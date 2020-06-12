using System;
using System.Collections.Generic;
using System.Text;

namespace AdaptiveProvider.Data
{
    public class ProviderConfigurationData
    {
        public ProviderConfigurationData()
        {
            ResourcesConfiguration = new Dictionary<string, ResourceConfigurationData>();
        }

        public string AdaptersDirectory { get; set; }
        public Dictionary<string, ResourceConfigurationData> ResourcesConfiguration { get; set; }
        public Dictionary<string, ServiceConfigurationData> ServicesConfiguration { get; set; }
        public Dictionary<string, string> ProvidersSchema { get; set; }
        public Dictionary<string, string> ConfigurationVariables { get; set; }
    }
}
