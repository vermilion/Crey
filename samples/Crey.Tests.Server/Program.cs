using Crey.Builder;
using Crey.Client;
using Crey.Codec.MessagePack;
using Crey.Extensions;
using Crey.Micro;
using Crey.Protocol.Tcp;
using Crey.Discovery;
using Crey.Discovery.Consul;
using Crey.Tests.Contracts;
using Crey.Tests.Server.Services;
using Crey.Discovery.StaticRouter;

namespace Crey.Tests.Server;

internal class Program
{
    private static async Task Main(string[] args)
    {
        var host = Host.CreateDefaultBuilder(args)
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
#if !DEBUG
                .AddStaticListDiscovery(x =>
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