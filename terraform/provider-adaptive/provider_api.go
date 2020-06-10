package main

import (
	"bytes"
	"encoding/json"
	"errors"
	"fmt"
	"log"
	"net/http"
	"net/url"
	"os"
	"path"

	"github.com/hashicorp/go-cleanhttp"
	tfjson "github.com/hashicorp/terraform-json"
)

const endpointVariableName = "TF_VAR_adaptive_config_endpoint"

const (
	statusRequested = 204
	statusAccepted  = 202
	statusCreated   = 201
	statusCompleted = 200
	statusFound     = 200
	statusNotFound  = 404
)

// providerAPI represents the resource provider API
type providerAPI struct {
	endpoint       *url.URL
	client         *http.Client
	providerSchema *tfjson.ProviderSchemas
}

// CloudResource represents the provider API exchange object
type CloudResource struct {
	Type string
	Id   string
	Data map[string]string
}

// CloudResourceSchema represents the cloud resource structure
type CloudResourceSchema struct {
	Type       string
	Attributes map[string]string
}

// initResourceAPI inits the ResourceApi
func initProviderAPI() (*providerAPI, error) {
	log.Printf("[TRACE][ADAPTIVE] >> Initializing Provider Api")

	ep := os.Getenv(endpointVariableName)
	u, err := url.Parse(ep)

	if err != nil {
		return nil, fmt.Errorf("Provider endpoint url not set properly by the environment variable <%v>. Error: %v", endpointVariableName, err)
	}

	api := &providerAPI{
		endpoint: u,
		client:   cleanhttp.DefaultClient(),
	}

	log.Printf("[TRACE][ADAPTIVE] >> Provider Api ready: <%v>", api.endpoint)

	return api, nil
}

func (api *providerAPI) providerURL(name string) string {
	u, err := url.Parse(api.endpoint.String())

	if err != nil {
		log.Panicf("[ERROR][ADAPTIVE] >> Failed creating url: %v", err)
	}

	u.Path = path.Join(u.Path, "provider", name)

	return u.String()
}

func (api *providerAPI) resourceURL(type_ string, id string) string {
	u, err := url.Parse(api.endpoint.String())

	if err != nil {
		log.Panicf("[ERROR][ADAPTIVE] >> Failed creating url: %v", err)
	}

	u.Path = path.Join(u.Path, "resource", type_, id)

	return u.String()
}

// getProviderSchema get the provider schema using the configuration API
func (api *providerAPI) getProviderSchema(name string) error {
	log.Printf("[TRACE][ADAPTIVE] >> Getting provider <%v> configuration from the server", name)

	// Attempt to read from an upstream API
	response, err := api.client.Get(api.providerURL(name))

	if err != nil {
		log.Printf("[WARN][ADAPTIVE] >> Could not connect to the provider configuration API: %v.", err)
		return err
	}

	if response != nil {
		if response.StatusCode != 200 {
			err = fmt.Errorf("Provider configuration returned status %d", response.StatusCode)
			log.Printf("[WARN][ADAPTIVE] >> %v.", err)
			return err
		}

		defer response.Body.Close()

		if err = json.NewDecoder(response.Body).Decode(&api.providerSchema); err != nil {
			log.Printf("[WARN][ADAPTIVE] >> Could not read the provider configuration: %v.", err)
			return err
		}

		log.Printf("[TRACE][ADAPTIVE] >> Provider configuration found.")
		return nil
	}

	return errors.New("[WARN][ADAPTIVE] GetProviderConfiguration: Could not evaluate the configuration API response")
}

// createResource create a resource
func (api *providerAPI) createResource(r *CloudResource) (*CloudResource, error) {
	log.Printf("[TRACE][ADAPTIVE] >> CreateResource")

	jsonData, err := json.Marshal(r)

	if err != nil {
		log.Printf("[TRACE][ADAPTIVE] >> Failed to serialize resource")
		return nil, err
	}

	// Attempt to read from an upstream API
	response, err := api.client.Post(api.resourceURL(r.Type, ""), "application/json", bytes.NewBuffer(jsonData))

	if err != nil {
		log.Printf("[WARN][ADAPTIVE] >> Could not connect to the Provider API: %v.", err)
		return nil, err
	}

	if response == nil {
		return nil, fmt.Errorf("[WARN][ADAPTIVE] >> Invalid response.")
	}

	if response.StatusCode == statusCreated {

		defer response.Body.Close()
		var resource *CloudResource

		if err = json.NewDecoder(response.Body).Decode(&resource); err != nil {
			log.Printf("[WARN][ADAPTIVE] >> Could not read resource: %v.", err)
			return nil, err
		}

		return resource, nil

	} else {
		err = fmt.Errorf("Provider API returned invalid status %d", response.StatusCode)
		log.Printf("[WARN][ADAPTIVE] >> %v.", err)
		return nil, err
	}
}

// getResource returns a resource configuration state
func (api *providerAPI) getResource(type_ string, id string) (*CloudResource, error) {
	log.Printf("[TRACE][ADAPTIVE] >> GetResource")

	// Get resource state
	response, err := api.client.Get(api.resourceURL(type_, id))

	if err != nil {
		log.Printf("[WARN][ADAPTIVE] >> Could not connect to the Provider API: %v.", err)
		return nil, err
	}

	if response == nil {
		err = fmt.Errorf("Invalid response ")
		log.Printf("[WARN][ADAPTIVE] >> Provider API returned: %v.", err)
		return nil, err
	}

	if response.StatusCode == statusFound {
		defer response.Body.Close()
		var resource *CloudResource
		if err = json.NewDecoder(response.Body).Decode(&resource); err != nil {
			log.Printf("[WARN][ADAPTIVE] >> Could not read resource: %v.", err)
			return nil, err
		}

		log.Printf("[TRACE][ADAPTIVE] >> Resource found.")
		return resource, nil

	} else if response.StatusCode == statusNotFound {
		log.Printf("[WARN][ADAPTIVE] >> Resource not found.")
		return nil, nil
	} else {
		err = fmt.Errorf("Provider API returned status %d", response.StatusCode)
		log.Printf("[WARN][ADAPTIVE] >> %v.", err)
		return nil, err
	}
}
