using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace PrivateCloudApi
{
    public class ProvisionerConfiguration : IProvisionerConfiguration
    {
        public ProvisionerConfiguration(string configurationPath)
        {
            if (string.IsNullOrWhiteSpace(configurationPath))
            {
                throw new ArgumentException($"ConfigurationRepository {nameof(configurationPath)} cannot be empty or whitespace");
            }

            if (!File.Exists(configurationPath))
            {
                throw new ArgumentException($"ConfigurationRepository [configurationPath] cannot be found");
            }

            string configJson = File.ReadAllText(configurationPath);
            _configuration = JsonSerializer.Deserialize<Dictionary<string, Provisioner>>(configJson);

            ConfigurationPath = configurationPath;
            ConfigurationDirectory = Path.GetDirectoryName(configurationPath);
        }

        private readonly Dictionary<string, Provisioner> _configuration;

        public string ConfigurationPath { get; }

        public string ConfigurationDirectory { get; }

        public Provisioner this[string type] => _configuration[type];
    }
}
