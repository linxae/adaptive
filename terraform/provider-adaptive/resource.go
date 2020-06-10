package main

import (
	"bytes"
	"fmt"
	"log"

	"github.com/hashicorp/terraform-plugin-sdk/helper/schema"
)

const ResourceTypeKey = "adaptive_reserved_resource_type"

//const ResourceNameKey = "name"

func populateResourceData(rd *schema.ResourceData, cr *CloudResource) error {
	log.Printf("[TRACE][ADAPTIVE] >> Populate Resource Data")

	resourceType := rd.Get(ResourceTypeKey).(string)
	log.Printf("[TRACE][ADAPTIVE] >> Resource Data Type <%v>", resourceType)
	log.Printf("[TRACE][ADAPTIVE] >> Cloud Resource Type <%v>", cr.Type)

	if resourceType != cr.Type {
		return fmt.Errorf("[TRACE][ADAPTIVE] >> Type missmatch between Resource Data and Cloud Resource")
	}

	if SchemaCache == nil {
		return fmt.Errorf("[TRACE][ADAPTIVE] >> Resource schema cache is not initialized")
	}

	if _, exists := SchemaCache[resourceType]; !exists {
		return fmt.Errorf("[TRACE][ADAPTIVE] >> Resource schema cache doesn't contain definition for [%v]", resourceType)
	}

	rd.SetId(cr.Id)

	//iterate through the attributes and fill the resource data
	for akey, atype := range SchemaCache[resourceType].Attributes {
		// if cr.Data[akey].(type) == string {
		// 	log.Printf("[TRACE][ADAPTIVE] >>  Populating  [%v]<%v>:%v", akey, atype, cr.Data[akey])
		// } else {

		// }

		log.Printf("[TRACE][ADAPTIVE] >>  Populating  [%v]<%v>:%v", akey, atype, cr.Data[akey])

		// switch v := param.(type) {
		// default:
		// 	fmt.Printf("unexpected type %T", v)
		// case uint64:
		// 	e.code = Code(C.curl_wrapper_easy_setopt_long(e.curl, C.CURLoption(option), C.long(v)))
		// case string:
		// 	e.code = Code(C.curl_wrapper_easy_setopt_str(e.curl, C.CURLoption(option), C.CString(v)))
		// }

		rd.Set(akey, cr.Data[akey])
	}

	log.Printf("[TRACE][ADAPTIVE] >> Resource Data Populated.")

	return nil
}

// populateCloudResource: fills the a CloudResource object using ResourceData
func populateCloudResource(d *schema.ResourceData) (*CloudResource, error) {

	log.Printf("[TRACE][ADAPTIVE] >> Populate Cloud Resource")

	resource := &CloudResource{
		Type: d.Get(ResourceTypeKey).(string),
		Id:   d.Id(),
		Data: map[string]string{},
	}

	log.Printf("[TRACE][ADAPTIVE] >> Resource Type <%v>", resource.Type)

	if SchemaCache == nil {
		return nil, fmt.Errorf("[TRACE][ADAPTIVE] >> Resource schema cache is not initialized")
	}

	if _, exists := SchemaCache[resource.Type]; !exists {
		return nil, fmt.Errorf("[TRACE][ADAPTIVE] >> Resource schema cache doesn't contain definition for [%v]", resource.Type)
	}

	//iterate through the attributes and fill the resource data
	for akey, atype := range SchemaCache[resource.Type].Attributes {
		log.Printf("[TRACE][ADAPTIVE] >>  Populating  [%v]<%v>:%v", akey, atype, d.Get(akey))

		switch v := d.Get(akey).(type) {
		default:
			log.Printf("[TRACE][ADAPTIVE] >>  unexpected type %T", v)
		case string:
			resource.Data[akey] = d.Get(akey).(string)
		case []interface{}:
			var buffer bytes.Buffer
			for i, s := range d.Get(akey).([]interface{}) {
				fmt.Println(i, s)
				buffer.WriteString(s.(string))
				buffer.WriteString(";")
			}

			resource.Data[akey] = buffer.String()
		}

	}

	log.Printf("[TRACE][ADAPTIVE] >> Cloud Resource Populated.")

	return resource, nil
}

func createResource(rd *schema.ResourceData, api interface{}) error {
	log.Printf("[TRACE][ADAPTIVE] >> Creating resource")

	resource, err := populateCloudResource(rd)

	if err != nil {
		log.Printf("[TRACE][ADAPTIVE] >> Cloud Resource population failed!")
		return err
	}

	resource, err = api.(*providerAPI).createResource(resource)

	if err != nil {
		log.Printf("[TRACE][ADAPTIVE] >> Resource creation failed!")
		return err
	}

	if resource == nil {
		return fmt.Errorf("Created resource is not valid")
	}

	log.Printf("[TRACE][ADAPTIVE] >> Resource id: %v.", resource.Id)

	err = populateResourceData(rd, resource)

	if err != nil {
		log.Printf("[WARN][ADAPTIVE] >> Resource data population from Cloud Resource failed!")
		return err
	}

	err = readResource(rd, api)

	if err != nil {
		log.Printf("[WARN][ADAPTIVE] >> Resource not found.")
		return err
	}

	log.Printf("[TRACE][ADAPTIVE] >> Resource created.")
	return nil
}

func readResource(rd *schema.ResourceData, api interface{}) error {
	log.Printf("[TRACE][ADAPTIVE] >> Reading resource <%v>", rd.Id())

	resourceType := rd.Get(ResourceTypeKey).(string)
	resource, err := api.(*providerAPI).getResource(resourceType, rd.Id())

	if err != nil {
		return err
	}

	if resource != nil {
		err = populateResourceData(rd, resource)

		if err != nil {
			log.Printf("[WARN][ADAPTIVE] >> Resource data population from Cloud Resource failed!")
			return err
		}
		log.Printf("[TRACE][ADAPTIVE] >> Resource found.")
	} else {
		rd.SetId("") //setting the id to empty marking the resource as not found
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
