package main

import (
	"bytes"
	"fmt"
	"log"
	"strconv"

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

	var err error = nil

	//iterate through the attributes and fill the resource data
	for akey, atype := range SchemaCache[resource.Type].Attributes {
		log.Printf("[TRACE][ADAPTIVE] >>  Populating  [%v]<%v>:%v", akey, atype, d.Get(akey))

		switch v := d.Get(akey).(type) {
		default:
			resource.Data[akey], err = convertToString(d.Get(akey))

			if err != nil {
				return nil, fmt.Errorf("Type %T of field [%v] is not supported", v, akey)
			}

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

func convertToString(v interface{}) (string, error) {
	switch t := v.(type) {
	default:
		return "", fmt.Errorf("Type %T is not supported", t)
	case string:
		return v.(string), nil
	case bool:
		return strconv.FormatBool(v.(bool)), nil
	case int64:
		return strconv.FormatInt(v.(int64), 10), nil
	case float64:
		return strconv.FormatFloat(v.(float64), 'f', -1, 64), nil
	}
}

// func convertFromString(v string) (interface{}, error) {
// 	switch t := v.(type) {
// 	default:
// 		return "", fmt.Errorf("Type %T is not supported")
// 	case string:
// 		return v.(string), nil
// 	case bool:
// 		return v.(bool), nil
// 	case float64:
// 		return v.(float64), nil
// 	case float32:
// 		return v.(float32), nil
// 	}
// }

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
