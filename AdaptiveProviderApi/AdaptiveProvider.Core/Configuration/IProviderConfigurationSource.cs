using System;
using System.Collections.Generic;
using System.Text;

namespace AdaptiveProvider.Core.Configuration
{
    public interface IProviderConfigurationSource
    {
        ProviderConfiguration Configuration { get; }
    }
}
