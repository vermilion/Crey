using Spear.Client;
using Spear.Codec.MessagePack.Extensions;
using Spear.Core.Builder;
using Spear.Core.Extensions;
using Spear.Core.ServiceDiscovery.Models;
using Spear.Core.ServiceDiscovery.StaticRouter.Extensions;
using Spear.Discovery.Consul.Extensions;
using Spear.Micro.Extensions;
using Spear.Protocol.Tcp.Extensions;
using Spear.Tests.Contracts;
using Spear.Tests.Server.Services;

namespace Spear.Tests.Server;

internal class Program
{
    private static async Task Main(string[] args)
    {
        var host = Host.CreateDefaultBuilder(args)
            .ConfigureLogging(x =>
            {
                x.SetMinimumLevel(LogLevel.Information);
                x.AddFilter("System", level => level >= LogLevel.Warning);
                x.AddFilter("Microsoft", level => level >= LogLevel.Warning);
                x.AddConsole();
            })
            .ConfigureServices((context, services) =>
            {
                var builder = new MicroBuilder(context.Configuration, services);

                builder
                    .AddTcpProtocol()
                    .AddMessagePackCodec()
                    .AddStaticServiceDiscovery()
                    //.AddConsulDiscovery()

                    .AddMicroService(builder =>
                    {
                        builder.AddContract<ITestContract, TestService>();
                    })
                    .AddMicroClient();
            })
            .Build();

        await Task.Factory.StartNew(async () =>
        {
            await host.RunAsync();
        });

        var provider = host.Services;

        await Task.Delay(10000);

        Console.WriteLine("Initialized");

        var factory = ClientBuilder.Create(provider)
            .CreateProxyFactory();

        var c = factory.Create<ITestContract>();
        var res = await c.Say("Hello existing");

        // create separate client
        var f2 = ClientBuilder.Create(builder =>
        {
            builder
                .AddTcpProtocol()
                .AddMessagePackCodec()
                .AddStaticServiceDiscovery(x =>
                {
                    x.Set<ITestContract>(new[] { new ServiceAddress("192.168.1.24", 5003) });
                })
                /*.AddConsulDiscovery(x =>
                {
                    x.Server = "http://192.168.1.24:8500";
                })*/
                .AddMicroClient();
        })
        .CreateProxyFactory();

        var contract = f2.Create<ITestContract>();

        Console.WriteLine("Ready");

        while (true)
        {
            var cmd = Console.ReadLine();
            if (cmd == "exit") break;

            if (cmd.StartsWith("one:"))
            {
                var command = cmd.Replace("one:", "");

                await SpearExtensions.InvokeOneWay<ITestContract>(provider, (x) => x.Say(command));
            }
            else
            {
                var result = contract.Say("Hello");
                Console.WriteLine(result.Result);
            }
        }
    }
}