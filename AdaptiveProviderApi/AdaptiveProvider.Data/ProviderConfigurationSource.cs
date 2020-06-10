using AdaptiveProvider.Core.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace AdaptiveProvider.Data
{
    public class ProviderConfigurationSource : IProviderConfigurationSource
    {
        private static readonly char[] ServiceSeparators = { ',', ';' };
        public ProviderConfigurationSource(ProviderConfigurationData configurationData)
        {
            Configuration = new ProviderConfiguration();

            Configuration.AdaptersDirectory = configurationData.AdaptersDirectory;

            foreach (var p in configurationData.ProvidersSchema)
            {
                if (p.Value == null) continue;

                Configuration.Schemas.Add(p.Key, new SchemaConfiguration()
                {
                    ProviderName = p.Key,
                    ProviderSchema = p.Value,
                });
            }

            foreach (var s in configurationData.ServicesConfiguration)
            {
                if (s.Value == null) continue;

                Configuration.Services.Add(s.Key, new ServiceConfiguration()
                {
                    ServiceName = s.Key,
                    ServicePlugin = s.Value.ServicePlugin,
                    ConnectionString = s.Value.ConnectionString,
                });
            }

            foreach (var r in configurationData.ResourcesConfiguration)
            {
                if (r.Value == null) continue;

                if (!Configuration.Services.ContainsKey(r.Value.ProvisiningAdapter))
                {
                    throw new Exception($"Service '{r.Value.ProvisiningAdapter}' required as Provisioning Adapter for resource '{r.Key}' cannot be found in the ServicesConfiguration");
                }

                Configuration.Resources.Add(r.Key, new ResourceConfiguration() { 
                    ResourceType = r.Key,
                    ProvisiningAdapter = Configuration.Services[r.Value.ProvisiningAdapter],
                    ProvisiningHandler = r.Value.ProvisiningHandler,
                    RequiredServices = (r.Value.RequiredServices ?? string.Empty).Split(ServiceSeparators, StringSplitOptions.RemoveEmptyEntries),
                });
            }
            
        }

        public ProviderConfiguration Configuration { get; }
    }
}
