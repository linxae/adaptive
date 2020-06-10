using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdaptiveProvider.Core;
using AdaptiveProvider.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace PrivateCloudApi.Controllers
{
    public abstract class BaseController : ControllerBase
    {
        protected readonly ILogger<ResourceController> _logger;
        protected readonly IResourceRepository _resourceRepository;
        protected readonly IProvisioningManager _provisioningManager;
        protected readonly ProviderConfigurationSource _configurationSource;

        protected BaseController(ILogger<ResourceController> logger, IResourceRepository resourceRepository, IOptions<ProviderConfigurationData> configurationData)
        {
            _logger = logger;
            _configurationSource = new ProviderConfigurationSource(configurationData.Value);
            _provisioningManager = new ProvisioningManager(_configurationSource.Configuration, logger);
            _resourceRepository = resourceRepository;
        }
    }
}
