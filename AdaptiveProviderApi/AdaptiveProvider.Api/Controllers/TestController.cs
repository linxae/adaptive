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
    public class TestController : BaseController
    {
        public TestController(ILogger<ResourceController> logger, IResourceRepository resourceRepository, IOptions<ProviderConfigurationData> configurationData)
            : base(logger, resourceRepository, configurationData)
        {
        }

        [HttpGet("{command}/{id}")]
        public ActionResult Ping(string command, string id)
        {
            _logger.Log(LogLevel.Information, $"ping {command}/{id} received at {DateTime.Now}");

            var t = Type.GetType("AdaptiveProvider.PowerShell.PowerShellAdapter, AdaptiveProvider.PowerShell, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", true, true);

            return Ok();
        }
    }
}
