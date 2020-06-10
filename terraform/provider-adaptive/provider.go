package main

import (
	"log"
	"os"
	"path/filepath"
	"strings"

	tfjson "github.com/hashicorp/terraform-json"
	"github.com/hashicorp/terraform-plugin-sdk/helper/schema"
	"github.com/zclconf/go-cty/cty"
)

var ProviderName = GetProviderName()

type AdaptiveProvider struct {
	Endpoint string
}

var SchemaCache map[string]*CloudResourceSchema = map[string]*CloudResourceSchema{}

func GetProviderName() string {
	name := strings.ToLower(filepath.Base(os.Args[0]))

	if strings.HasPrefix(name, "terraform-provider-") && strings.HasSuffix(name, ".exe") {
		return name[19 : len(name)-4]
	} else {
		return "adaptive"
	}
}

func Provider() *schema.Provider {
	log.Printf("[TRACE][ADAPTIVE] >> Initializing provider")

	api := prepareProviderAPI()

	provider := initializeProvider(ProviderName, api.providerSchema)

	log.Printf("[TRACE][ADAPTIVE] >> Provider ready.")
	return provider
}

func prepareProviderAPI() *providerAPI {
	log.Printf("[TRACE][ADAPTIVE] >> Initializing provider")

	api, err := initProviderAPI()

	if err != nil {
		log.Panicf("[ERROR][ADAPTIVE] >> Failed to initialize the configuration: %v", err)
	}

	err = api.getProviderSchema(ProviderName)

	if err != nil {
		log.Panicf("[ERROR][ADAPTIVE] >> Failed to get the provider configuration: %v", err)
	}

	return api
}

func initializeProvider(name string, schemas *tfjson.ProviderSchemas) *schema.Provider {
	log.Printf("[TRACE][ADAPTIVE] >> Configuring provider <" + name + "> schema")

	//initialize the resources schema cache
	//SchemaCache := map[string]*CloudResourceSchema{}

	//initialize the adaptive provider
	provider := &schema.Provider{
		Schema: map[string]*schema.Schema{},

		ResourcesMap: map[string]*schema.Resource{},

		ConfigureFunc: configureProvider,
	}

	//get the provider schema definition
	providerSchema := schemas.Schemas[name]

	if providerSchema == nil {
		log.Panicf("[ERROR][ADAPTIVE] Provider <" + name + "> not defined in the configuration.")
	}

	//get the provider configuration definition
	providerConfig := providerSchema.ConfigSchema
	log.Printf("[TRACE][ADAPTIVE] >> provider: <" + name + ">")

	//configure the provider attributes using the definition
	for pkey, pattr := range providerConfig.Block.Attributes {
		log.Printf("[TRACE][ADAPTIVE] >>    <" + pkey + "> [" + pattr.AttributeType.FriendlyName() + "]")

		provider.Schema[pkey] = &schema.Schema{
			Type:        getValueTypeFromType(pattr.AttributeType),
			Required:    pattr.Required,
			Optional:    pattr.Optional,
			DefaultFunc: schema.EnvDefaultFunc(pkey, nil),
			Description: pattr.Description,
		}
	}

	//get provider resources defintion
	resourcesConfig := providerSchema.ResourceSchemas

	//configure the provider resources schemas using the definition
	for rkey, resourceConfig := range resourcesConfig {
		log.Printf("[TRACE][ADAPTIVE] >> resource: <" + rkey + ">")

		resourceSchema := &CloudResourceSchema{
			Type:       rkey,
			Attributes: map[string]string{},
		}

		//configure resource schema
		resource := &schema.Resource{
			Create: createResource,
			Read:   readResource,
			Update: updateResource,
			Delete: deleteResource,

			Schema: map[string]*schema.Schema{},
		}

		//configure the mandatory and reserved resource type attribute
		resource.Schema[ResourceTypeKey] = &schema.Schema{
			Type:        schema.TypeString,
			Required:    false,
			Optional:    true,
			Description: ResourceTypeKey,
			Default:     rkey,
		}

		//configure resource attributes
		for akey, rattr := range resourceConfig.Block.Attributes {
			log.Printf("[TRACE][ADAPTIVE] >>    <" + akey + "> [" + rattr.AttributeType.FriendlyName() + "]")

			resource.Schema[akey] = &schema.Schema{
				Type:        getValueTypeFromType(rattr.AttributeType),
				Required:    rattr.Required,
				Computed:    rattr.Computed,
				Description: rattr.Description,
			}

			if resource.Schema[akey].Type == schema.TypeList {
				resource.Schema[akey].Elem = &schema.Schema{Type: schema.TypeString}
			}

			resourceSchema.Attributes[akey] = resource.Schema[akey].Type.String()
		}

		provider.ResourcesMap[rkey] = resource

		log.Printf("[TRACE][ADAPTIVE] >> Resource [%v] schema cache added.", rkey)
		SchemaCache[rkey] = resourceSchema
	}

	if err := schemas.Validate(); err != nil {
		log.Panicf("[ERROR][ADAPTIVE] >> Failed during provider schema validation: %v", err)
	}

	log.Printf("[TRACE][ADAPTIVE] >> Provider schema configuration completed. [%v] resources added.", len(SchemaCache))
	return provider
}

func configureProvider(d *schema.ResourceData) (interface{}, error) {
	log.Printf("[TRACE][ADAPTIVE] >> Preparing provider configuration")

	api := prepareProviderAPI()

	log.Printf("[TRACE][ADAPTIVE] >> Provider configuration completed.")

	return api, nil
}

func getValueTypeFromType(t cty.Type) schema.ValueType {

	if strings.HasPrefix(t.FriendlyName(), "list") {
		return schema.TypeList
	}

	switch t.FriendlyName() {
	case "bool":
		return schema.TypeBool
	case "number":
		return schema.TypeInt
	//case "number":
	//	return schema.TypeFloat
	case "string":
		return schema.TypeString
	case "list":
		return schema.TypeList
	case "map":
		return schema.TypeMap
	case "set":
		return schema.TypeSet
	/*case "object":
	return schema.typeObject*/
	default:
		return schema.TypeInvalid
	}

}
