using AdaptiveProvider.Core.Configuration;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace AdaptiveProvider.Core
{
    public class CloudResource
    {
        public CloudResource()
        {
            Data = new Dictionary<string, string>();
        }
        public string Type { get; set; }
        public string Id { get; set; }

        public Dictionary<string, string> Data { get; set; }

        [JsonIgnore]
        public ResourceConfiguration Configuration { get; set; }

        [JsonIgnore]
        public Dictionary<string, object> RequiredServices { get; set; }

        [JsonIgnore]
        public Dictionary<string, string> ConfigurationVariables { get; set; }
    }
}
