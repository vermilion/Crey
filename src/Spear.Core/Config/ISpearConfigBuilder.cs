using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Spear.Core.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Spear.Core.Config
{
    public interface ISpearConfigBuilder : IConfigurationBuilder
    {
    }
}
