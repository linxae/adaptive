using System;
using System.Collections.Generic;

namespace AdaptiveProvider.Core.Configuration
{
    public class ProviderConfiguration
    {
        public ProviderConfiguration()
        {
            Resources = new Dictionary<string, ResourceConfiguration>();
            Services = new Dictionary<string, ServiceConfiguration>();
            Schemas = new Dictionary<string, SchemaConfiguration>();
            Variables = new Dictionary<string, string>();
        }

        public string AdaptersDirectory { get; set; }
        public IDictionary<string, ResourceConfiguration> Resources  { get; }
        public IDictionary<string, ServiceConfiguration> Services { get; }
        public IDictionary<string, SchemaConfiguration> Schemas { get; }
        public Dictionary<string, string> Variables { get; }
    }
}
