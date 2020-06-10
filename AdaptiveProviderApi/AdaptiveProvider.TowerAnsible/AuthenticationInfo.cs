using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace AdaptiveProvider.TowerAnsible
{
    internal class AuthenticationInfo
    {
        [JsonPropertyName("detail")]
        public string Detail { get; set; }

        [JsonPropertyName("token")]
        public string Token { get; set; }

        [JsonPropertyName("expires")]
        public DateTime Expires { get; set; }
    }
}
