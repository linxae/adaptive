package main

import (
	"github.com/hashicorp/terraform-plugin-sdk/helper/schema"
)

func Provider() *schema.Provider {
	return &schema.Provider{
		ResourcesMap: map[string]*schema.Resource{
			"adaptive_server": resourceServer(),
		},
	}
}

/*
func Provider() *schema.Provider {
	log.Printf("[INFO] preparing 'adaptive' provider...")
	return &schema.Provider{
		Schema: map[string]*schema.Schema{
			"endpoint": {
				Type:        schema.TypeString,
				Required:    true,
				DefaultFunc: schema.EnvDefaultFunc("PC_ENDPOINT", nil),
				Description: "The HTTP endpoint for PC API operations.",
			},
		},

		ResourcesMap: map[string]*schema.Resource{
			"adaptive_server": resourceServer(),
		},

		ConfigureFunc: providerConfigure,
	}
}

func providerConfigure(d *schema.ResourceData) (interface{}, error) {
	config := Config{
		Endpoint: d.Get("endpoint").(string),
	}

	return config.Client()
}
*/
