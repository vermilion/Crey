using Crey.Builder;
using Crey.Clients;
using Crey.Discovery.Consul;
#if !DEBUG
using Crey.Discovery;
using Crey.Discovery.StaticList;
#endif
using Crey.Extensions;
using Crey.Tests.Contracts;
using Crey.Service;

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
                    .AddMicroClient(builder =>
                    {
                        builder.AddMiddleware<ClientMiddleware>();
                    })
                    .AddMicroService(builder =>
                    {
                        builder.AddContract<ITestContract, TestService>();

                        builder.AddMiddleware<ServiceMiddleware>();
                    });
            })
            .Build();

        await Task.Factory.StartNew(async () =>
        {
            await host.RunAsync();
        });

        Console.WriteLine("Ready");

        var proxyFactory = ClientBuilder.Create(host.Services).CreateProxyFactory();

        var contract = proxyFactory.Create<ITestContract>();

        while (true)
        {
            try
            {
                var cmd = Console.ReadLine();
                if (cmd == "exit") break;

                if (cmd?.StartsWith("one:") == true)
                {
                    var command = cmd.Replace("one:", "");

                    await contract.InvokeOneWay((x) => x.Say(command));
                }
                else if (cmd?.StartsWith("client:") == true)
                {
                    var command = cmd.Replace("client:", "");

                    var c = await GetStandaloneClient();

                    var result = c.Say("Hello from client");
                    Console.WriteLine(result.Result);
                }
                else
                {
                    var result = contract.Say("Hello");
                    Console.WriteLine(result.Result);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }

    private static Task<ITestContract> GetStandaloneClient()
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

        return Task.FromResult(contract);
    }
}