using System;
using System.Collections.Generic;

namespace AdaptiveProvider.Core.Configuration
{
    public class ResourceConfiguration
    {
        public string ResourceType { get; set; }
        public ServiceConfiguration ProvisiningAdapter { get; set; }
        public string ProvisiningHandler { get; set; }
        public IEnumerable<string> RequiredServices { get; set; }
        public static string Read => "Read"; 
        public static string Create => "Create"; 
        public static string Update => "Update"; 
        public static string Delete => "Delete";
    }
}
