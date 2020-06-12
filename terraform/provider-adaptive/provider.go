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

		provider.Schema[pkey] = createSchemaByType(pattr.AttributeType)

		provider.Schema[pkey].Required = pattr.Required
		provider.Schema[pkey].Optional = pattr.Optional
		provider.Schema[pkey].DefaultFunc = schema.EnvDefaultFunc(pkey, nil)
		provider.Schema[pkey].Description = pattr.Description

		if pattr.Required == false && pattr.Optional == false && pattr.Computed == false {
			provider.Schema[pkey].Optional = true
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

			resource.Schema[akey] = createSchemaByType(rattr.AttributeType)

			resource.Schema[akey].Required = rattr.Required
			resource.Schema[akey].Optional = rattr.Optional
			resource.Schema[akey].Computed = rattr.Computed
			resource.Schema[akey].Description = rattr.Description

			if rattr.Required == false && rattr.Optional == false && rattr.Computed == false {
				resource.Schema[akey].Optional = true
			}

			//cache the attribute type to be used for the data conversion
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

//https://www.terraform.io/docs/configuration/types.html
//https://www.terraform.io/docs/extend/schemas/schema-types.html
func createSchemaByType(t cty.Type) *schema.Schema {

	typeSchema := &schema.Schema{}

	if t.IsPrimitiveType() {
		switch t {
		case cty.Bool:
			typeSchema.Type = schema.TypeBool
		case cty.Number:
			typeSchema.Type = schema.TypeFloat
		case cty.String:
			typeSchema.Type = schema.TypeString
		default:
			log.Panic("'%v' is not a supported primitive type", t.FriendlyName())
		}
	} else if t.IsCollectionType() {
		if t.IsListType() {
			typeSchema.Type = schema.TypeList
		} else if t.IsMapType() {
			typeSchema.Type = schema.TypeMap
		} else if t.IsSetType() {
			typeSchema.Type = schema.TypeSet
		} else {
			log.Panic("'%v' is not a supported collection type", t.FriendlyName())
		}

		typeSchema.Elem = createSchemaByType(t.ElementType())
	} else if t.IsObjectType() {
		log.Panic("'%v': object types are not supported ", t.FriendlyName())

	} else if t.IsTupleType() {
		log.Panic("'%v': tuple types are not supported ", t.FriendlyName())

	} else {
		log.Panic("'%v' is not a supported type", t.FriendlyName())
	}

	if typeSchema.Elem == nil {
		log.Printf("[TRACE][ADAPTIVE] >>    type: %v", typeSchema.Type.String())
	} else {
		log.Printf("[TRACE][ADAPTIVE] >>    type: %v[%v]", typeSchema.Type.String(), typeSchema.Elem.(*schema.Schema).Type.String())
	}

	return typeSchema
}
