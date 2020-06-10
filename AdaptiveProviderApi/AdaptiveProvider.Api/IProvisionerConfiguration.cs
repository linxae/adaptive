namespace PrivateCloudApi
{
    public interface IProvisionerConfiguration
    {
        Provisioner this[string type] { get; }

        string ConfigurationPath { get; }

        string ConfigurationDirectory { get; }
    }
}