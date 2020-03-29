package main

import (
	"fmt"
	"log"

	"github.com/hashicorp/terraform-plugin-sdk/helper/schema"
)

const ResourceNameKey = "adaptive_reserved_resource_name"

/*
func configureResource(rtype string, config *ConfigurationApi) *schema.Resource {

	log.Printf("[TRACE][ADAPTIVE] >> Configuring resource <" + rtype + ">")

	resource := &schema.Resource{
		Create: resourceServerCreate,
		Read:   resourceServerRead,
		Update: resourceServerUpdate,
		Delete: resourceServerDelete,

		Schema: map[string]*schema.Schema{},
	}

	//Get the resource configuration from the ConfigurationApi
	resource.Schema["address"] = &schema.Schema{
		Type:     schema.TypeString,
		Required: true,
	}

	log.Printf("[TRACE][ADAPTIVE] >> Resource <" + rtype + "> ready.")
	return resource
}
*/

func createResource(d *schema.ResourceData, api interface{}) error {
	log.Printf("[TRACE][ADAPTIVE] >> Creating resource")

	resource := &CloudResource{
		Type_: d.Get(ResourceNameKey).(string),
		Data: map[string]string{
			"dns": d.Get("dns").(string),
		},
	}

	resource, err := api.(*providerAPI).createResource(resource)

	if err != nil {
		log.Printf("[TRACE][ADAPTIVE] >> Resource creation failed.")
		return err
	}

	if resource == nil {
		return fmt.Errorf("Created resource is not valid")
	}

	log.Printf("[TRACE][ADAPTIVE] >> Resource id: %v.", resource.Id)

	d.SetId(resource.Id)

	err = readResource(d, api)

	if err != nil {
		log.Printf("[WARN][ADAPTIVE] >> Resource not found.")
		return err
	}

	log.Printf("[TRACE][ADAPTIVE] >> Resource created.")
	return nil
}

func readResource(d *schema.ResourceData, api interface{}) error {
	log.Printf("[TRACE][ADAPTIVE] >> Reading resource <%v>", d.Id())

	type_ := d.Get(ResourceNameKey).(string)
	resource, err := api.(*providerAPI).getResource(type_, d.Id())

	if err != nil {
		return err
	}

	if resource != nil {
		d.SetId(resource.Id)
		d.Set("dns", resource.Data["dns"])
		log.Printf("[TRACE][ADAPTIVE] >> Resource found.")
	} else {
		d.SetId("")
		log.Printf("[TRACE][ADAPTIVE] >> Resource not found.")
	}

	return nil
}

func updateResource(d *schema.ResourceData, m interface{}) error {
	return readResource(d, m)
}

func deleteResource(d *schema.ResourceData, m interface{}) error {
	return nil
}
