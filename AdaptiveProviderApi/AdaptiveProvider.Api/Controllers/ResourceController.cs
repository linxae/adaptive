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
    [ApiController]
    [Route("privatecloud/[controller]")]
    public class ResourceController : BaseController
    {
        public ResourceController(ILogger<ResourceController> logger, IResourceRepository resourceRepository, IOptions<ProviderConfigurationData> configurationData)
            : base(logger, resourceRepository, configurationData)
        {
        }

        [HttpGet("{type}/{id}")]
        public ActionResult<CloudResource> Get(string type, string id)
        {
            _logger.Log(LogLevel.Information, "Get resource {0}:{1}", type, id);

            var r = _provisioningManager.ReadResource(type, id);

            if (r != null)
            {
                _logger.Log(LogLevel.Information, "Found {0}:{1}", type, r);
                return r;
            }
            else
            {
                _logger.Log(LogLevel.Warning, "Not found {0}:{1}", type, id);
                return NotFound();
            }
        }

        [HttpPost("{type}")]
        public ActionResult<CloudResource> Create(string type, CloudResource resource)
        {
            _logger.Log(LogLevel.Information, "Creating {0}:{1}...", type, resource);

            resource = _provisioningManager.CreateResource(resource);

            _logger.Log(LogLevel.Information, "Created {0}:{1}", type, resource);

            return CreatedAtAction(nameof(Get), new { type = resource.Type, id = resource.Id }, resource);
        }

        [HttpPatch("{type}")]
        public ActionResult<CloudResource> Update(string type, CloudResource resource)
        {
            _logger.Log(LogLevel.Information, "Updating resource: {0}:{1}...", type, resource);

            resource = _provisioningManager.UpdateResource(resource);

            _logger.Log(LogLevel.Information, "Updated {0}:{1}", type, resource);

            return new ObjectResult(resource);
        }

        [HttpDelete("{type}")]
        public ActionResult<CloudResource> Delete(string type, CloudResource resource)
        {
            _logger.Log(LogLevel.Information, "Creating {0}:{1}...", type, resource);

            resource = _provisioningManager.DestroyResource(resource);

            _logger.Log(LogLevel.Information, "Deleted {0}:{1}", type, resource);

            return CreatedAtAction(nameof(Get), new { type = resource.Type, id = resource.Id }, resource);
        }
    }
}
