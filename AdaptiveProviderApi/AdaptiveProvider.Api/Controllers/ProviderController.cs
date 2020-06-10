using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdaptiveProvider.Core;
using AdaptiveProvider.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace PrivateCloudApi.Controllers
{
    [ApiController]
    [Route("privatecloud/[controller]")]
    public class ProviderController : BaseController
    {
        public ProviderController(ILogger<ResourceController> logger, IResourceRepository resourceRepository, IOptions<ProviderConfigurationData> configurationData)
            : base(logger, resourceRepository, configurationData)
        {
        }

        [HttpGet("{name}")]
        public ActionResult Get(string name)
        {
            _logger.Log(LogLevel.Information, "Get resource {0}", name);

            if (!_configurationSource.Configuration.Schemas.ContainsKey(name))
            {
                return NotFound(new { Message= $"Provider schema '{name}' could not be found in the configuration" });
            }

            if (!System.IO.File.Exists(_configurationSource.Configuration.Schemas[name].ProviderSchema))
            {
                return NotFound(new { Message = $"Provider schema '{_configurationSource.Configuration.Schemas[name].ProviderSchema}' could not be found" });
            }

            return File(System.IO.File.OpenRead(_configurationSource.Configuration.Schemas[name].ProviderSchema), "application/json");
        }
    }
}
