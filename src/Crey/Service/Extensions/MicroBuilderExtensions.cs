﻿using Crey.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Crey.Service;

public static class MicroBuilderExtensions
{
    /// <summary>
    /// Adds service-related services
    /// </summary>
    /// <param name="builder">Builder instance <see cref="IMicroBuilder"/></param>
    /// <param name="builderAction">Configure action</param>
    /// <returns>Fluent for <see cref="IMicroBuilder"/></returns>
    public static IMicroBuilder AddMicroService(this IMicroBuilder builder, Action<IMicroServerBuilder> builderAction)
    {
        var services = builder.Services;

        builderAction.Invoke(builder);

        services.AddBaseServices();

        services.AddServiceServices();
        services.AddSingleton<IServiceEntryFactory, ServiceEntryFactory>();

        services.AddScoped<ICallContextAccessor, CallContextAccessor>();

        services.AddHostedService<HostListenerBackroundService>();

        services.Configure<ServiceAddress>(builder.ConfigurationSection.GetSection("service"));
        services.AddTransient<IPostConfigureOptions<ServiceAddress>, PostConfigureServiceAddress>();

        return builder;
    }
}
