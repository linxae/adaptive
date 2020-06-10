using System;
using System.Collections.Generic;
using System.Text;

namespace AdaptiveProvider.Core
{
    public interface IProvisioningService
    {
        CloudResource Create(CloudResource resource);

        CloudResource Read(CloudResource resource);

        CloudResource Update(CloudResource resource);

        CloudResource Delete(CloudResource resource);
    }
}
