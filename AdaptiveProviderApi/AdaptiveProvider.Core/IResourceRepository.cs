
namespace AdaptiveProvider.Core
{
    public interface IResourceRepository
    {
        string Add(string type, CloudResource resource);
        CloudResource Get(string type, string id);
    }
}