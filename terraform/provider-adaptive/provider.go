package main

import (
	"log"

	tfjson "github.com/hashicorp/terraform-json"
	"github.com/hashicorp/terraform-plugin-sdk/helper/schema"
	"github.com/zclconf/go-cty/cty"
)

const ProviderName = "adaptive"

type AdaptiveProvider struct {
	Endpoint string
}

//
//
//
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

	provider := &schema.Provider{
		Schema: map[string]*schema.Schema{},

		ResourcesMap: map[string]*schema.Resource{},

		ConfigureFunc: configureProvider,
	}

	providerSchema := schemas.Schemas[name]

	if providerSchema == nil {
		log.Panicf("[ERROR][ADAPTIVE] Provider <" + name + "> not defined in the configuration.")
	}

	providerConfig := providerSchema.ConfigSchema
	log.Printf("[TRACE][ADAPTIVE] >> provider: <" + name + ">")

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

	resourcesConfig := providerSchema.ResourceSchemas

	for rkey, resourceConfig := range resourcesConfig {
		log.Printf("[TRACE][ADAPTIVE] >> resource: <" + rkey + ">")

		resource := &schema.Resource{
			Create: createResource,
			Read:   readResource,
			Update: updateResource,
			Delete: deleteResource,

			Schema: map[string]*schema.Schema{},
		}

		resource.Schema[ResourceNameKey] = &schema.Schema{
			Type:        schema.TypeString,
			Required:    false,
			Optional:    true,
			Description: ResourceNameKey,
			Default:     rkey,
		}

		for akey, rattr := range resourceConfig.Block.Attributes {
			log.Printf("[TRACE][ADAPTIVE] >>    <" + akey + "> [" + rattr.AttributeType.FriendlyName() + "]")

			resource.Schema[akey] = &schema.Schema{
				Type:        getValueTypeFromType(rattr.AttributeType),
				Required:    rattr.Required,
				Description: rattr.Description,
			}
		}

		provider.ResourcesMap[rkey] = resource
	}

	if err := schemas.Validate(); err != nil {
		log.Panicf("[ERROR][ADAPTIVE] >> Failed during provider schema validation: %v", err)
	}

	log.Printf("[TRACE][ADAPTIVE] >> Provider schema configuration completed.")
	return provider
}

func configureProvider(d *schema.ResourceData) (interface{}, error) {
	log.Printf("[TRACE][ADAPTIVE] >> Preparing provider configuration")

	api := prepareProviderAPI()

	log.Printf("[TRACE][ADAPTIVE] >> Provider configuration completed.")

	return api, nil
}

func getValueTypeFromType(t cty.Type) schema.ValueType {

	switch t.FriendlyName() {
	case "bool":
		return schema.TypeBool
	/*case "number":
	return schema.TypeInt*/
	case "number":
		return schema.TypeFloat
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
