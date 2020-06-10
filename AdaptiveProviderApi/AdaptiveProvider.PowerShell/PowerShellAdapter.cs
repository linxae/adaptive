using AdaptiveProvider.Core;
using AdaptiveProvider.Core.Configuration;
using System;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Text.Json;

namespace AdaptiveProvider.Powershell
{
    public class PowerShellAdapter : IProvisioningService
    {
        private string _scriptsDirectory;

        public PowerShellAdapter(string scriptsDirectory)
        {
            _scriptsDirectory = scriptsDirectory;
        }

        private string ScriptPath(ResourceConfiguration config)
        {
            var scriptName = config.ProvisiningHandler ?? $"{config.ResourceType}.ps1";
            return Path.Combine(_scriptsDirectory, scriptName);
        }

        private object RunProvisionerCommand(string command, CloudResource resource)
        {
            using (PowerShell ps = PowerShell.Create())
            {
                ps.AddScript(File.ReadAllText(ScriptPath(resource.Configuration)))
                    .AddParameter("Resource", resource);

                var results = ps.Invoke();

                if (ps.HadErrors)
                {
                    throw ps.Streams.Error.FirstOrDefault()?.Exception ?? new Exception("Uknown powershell failure");
                }

                results = ps.AddCommand(command)
                    .AddParameter("Resource", resource)
                    .Invoke();

                return results.LastOrDefault()?.BaseObject;
            }
        }

        public CloudResource Create(CloudResource resource)
        {
            var result = RunProvisionerCommand(ResourceConfiguration.Create, resource);
            var createdResource = result as CloudResource;

            if (createdResource == null)
            {
                throw new Exception("The returned result is not a cloud resource");
            }

            return createdResource;
        }

        public CloudResource Read(CloudResource resource)
        {
            var resultResource = RunProvisionerCommand(ResourceConfiguration.Read, resource);

            if (resultResource == null)
                return null;

            var resourceJson = resultResource as string;

            resource = JsonSerializer.Deserialize<CloudResource>(resourceJson);

            return resource;
        }

        public CloudResource Update(CloudResource resource)
        {
            var result = RunProvisionerCommand(ResourceConfiguration.Update, resource);
            var updatedResource = result as CloudResource;

            if (updatedResource == null)
            {
                throw new Exception("The returned result is not a cloud resource");
            }

            return updatedResource;
        }

        public CloudResource Delete(CloudResource resource)
        {
            var result = RunProvisionerCommand(ResourceConfiguration.Delete, resource);
            var createdResource = result as CloudResource;

            if (createdResource != null)
            {
                throw new Exception("The cloud resource could not be deleted");
            }

            return createdResource;
        }
    }
}
