using Crey.Builder;
using Crey.Client;
using Crey.Codec.MessagePack.Extensions;
using Crey.Extensions;
using Crey.Micro.Extensions;
using Crey.Protocol.Tcp.Extensions;
using Crey.ServiceDiscovery.Models;
using Crey.Discovery.Consul.Extensions;
using Crey.Tests.Contracts;
using Crey.Tests.Server.Services;
using Crey.ServiceDiscovery.StaticRouter.Extensions;

namespace Crey.Tests.Server;

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
                    //.AddStaticServiceDiscovery()
                    .AddConsulDiscovery()

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
        var proxyFactory = ClientBuilder.Create(builder =>
        {
            builder
                .AddTcpProtocol()
                .AddMessagePackCodec()
#if DEBUG
                .AddStaticServiceDiscovery(x =>
                {
                    x.Set<ITestContract>(new[] { new ServiceAddress("192.168.1.24", 5003) });
                })
#else
                .AddConsulDiscovery(x =>
                {
                    x.Server = "http://192.168.1.24:8500";
                })
#endif
                .AddMicroClient();
        })
        .CreateProxyFactory();

        var contract = proxyFactory.Create<ITestContract>();

        Console.WriteLine("Ready");

        while (true)
        {
            var cmd = Console.ReadLine();
            if (cmd == "exit") break;

            if (cmd?.StartsWith("one:") == true)
            {
                var command = cmd.Replace("one:", "");

                await contract.InvokeOneWay((x) => x.Say(command));
            }
            else
            {
                var result = contract.Say("Hello");
                Console.WriteLine(result.Result);
            }
        }
    }
}