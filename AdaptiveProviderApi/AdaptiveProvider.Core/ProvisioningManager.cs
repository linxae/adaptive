using AdaptiveProvider.Core.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace AdaptiveProvider.Core
{
    public class ProvisioningManager : IProvisioningManager
    {
        private readonly IDictionary<string, object> _provisioningServices;
        private readonly ProviderConfiguration _provisionerConfiguration;
        private readonly ILogger _logger;

        public ProvisioningManager(ProviderConfiguration provisionerConfiguration, ILogger logger)
        {
            _provisionerConfiguration = provisionerConfiguration;
            _logger = logger;
            _provisioningServices = InitializeServices();

        }

        private IDictionary<string, object> InitializeServices()
        {
            var provisioningServices = new Dictionary<string, object>();

            foreach (var serviceConfig in _provisionerConfiguration.Services)
            {
                try
                {
                    var plugin = Type.GetType(serviceConfig.Value.ServicePlugin, throwOnError: true, ignoreCase: true);
                    var service = Activator.CreateInstance(plugin, serviceConfig.Value.ConnectionString);
                    provisioningServices.Add(serviceConfig.Key, service);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning($"Service plugin '{serviceConfig.Value.ServicePlugin}' initialization failed: {ex.Message}");

                    if (ex.InnerException != null)
                    {
                        _logger.LogWarning($"More details>> : {ex.InnerException.Message}");
                    }
                }
            }

            return provisioningServices;
        }

        private IProvisioningService ConfigureResourceServices(CloudResource resource)
        {
            if (!_provisionerConfiguration.Resources.ContainsKey(resource.Type))
            {
                throw new Exception($"Resouce configuration not found for {resource.Type}");
            }

            resource.Configuration = _provisionerConfiguration.Resources[resource.Type];

            if (resource.Configuration.ProvisiningAdapter == null)
            {
                throw new Exception($"Resouce '{resource.Type}' configuration missing provisioning service information");
            }

            if (!_provisioningServices.ContainsKey(resource.Configuration.ProvisiningAdapter.ServiceName))
            {
                throw new Exception($"Provisioining service '{resource.Configuration.ProvisiningAdapter.ServiceName}' not found for resource '{resource.Type}'");
            }

            var provisioningService = _provisioningServices[resource.Configuration.ProvisiningAdapter.ServiceName] as IProvisioningService;

            if (provisioningService == null)
            {
                throw new Exception($"Provisioining service '{resource.Configuration.ProvisiningAdapter.ServiceName}' is not of type '{nameof(IProvisioningService)}'");
            }

            var requiredServices = new Dictionary<string, object>();

            foreach (var serviceName in resource.Configuration.RequiredServices)
            {
                if (!_provisioningServices.ContainsKey(serviceName))
                {
                    throw new Exception($"Required service '{serviceName}' is not defined");
                }

                var requiredService = _provisioningServices[serviceName];

                if (requiredService == null)
                {
                    throw new Exception($"Required service '{serviceName}' is not initialized");
                }

                requiredServices.Add(serviceName, requiredService);
            }

            resource.RequiredServices = requiredServices;
            resource.ConfigurationVariables = _provisionerConfiguration.Variables;

            return provisioningService;
        }

        public CloudResource CreateResource(CloudResource resource)
        {
            var provisioningService = ConfigureResourceServices(resource);

            var result = provisioningService.Create(resource);
            return result;
        }

        public CloudResource ReadResource(string type, string id)
        {
            var resource = new CloudResource() { Type = type, Id = id };
            var provisioningService = ConfigureResourceServices(resource);

            var result = provisioningService.Read(resource);
            return result;
        }

        public CloudResource UpdateResource(CloudResource resource)
        {
            var provisioningService = ConfigureResourceServices(resource);

            var result = provisioningService.Update(resource);
            return result;
        }

        public CloudResource DestroyResource(CloudResource resource)
        {
            var provisioningService = ConfigureResourceServices(resource);
            var result = provisioningService.Delete(resource);
            return result;
        }

    }
}
