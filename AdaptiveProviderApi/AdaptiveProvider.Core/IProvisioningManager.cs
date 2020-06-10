using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdaptiveProvider.Core
{
    public interface IProvisioningManager
    {
        CloudResource ReadResource(string type, string id);

        CloudResource CreateResource(CloudResource resource);

        CloudResource UpdateResource(CloudResource resource);

        CloudResource DestroyResource(CloudResource resource);
    }
}
