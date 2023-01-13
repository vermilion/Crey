using Crey.Builder;
using Crey.Extensions;
using Crey.Micro;
using Crey.Discovery;
using Crey.Discovery.Consul;
using Crey.Tests.Contracts;
using Crey.Tests.Server.Services;
using Crey.Discovery.StaticList;
using Crey.Client;

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
                    //.AddStaticListDiscovery()
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

        await TestSeparateClient();

        Console.WriteLine("Ready");

        var proxyFactory = ClientBuilder.Create(provider).CreateProxyFactory();

        var contract = proxyFactory.Create<ITestContract>();

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

    private static async Task TestSeparateClient()
    {
        // create standalone client
        var proxyFactory = ClientBuilder.Create(builder =>
        {
            builder
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

        var result = await contract.Say("Hello from Client");
        Console.WriteLine(result);
    }
}