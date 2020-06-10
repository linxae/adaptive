using System;
using System.Collections.Generic;
using System.Text;

namespace AdaptiveProvider.Core.Configuration
{
    public class ServiceConfiguration
    {
        public string ServiceName { get; set; }
        public string ServicePlugin { get; set; }
        public string ConnectionString { get; set; }
    }
}
