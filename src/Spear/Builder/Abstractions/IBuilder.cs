using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Spear.Core.Builder.Abstractions;

public interface IBuilder
{
    IConfiguration Configuration { get; }
    IServiceCollection Services { get; }
}
