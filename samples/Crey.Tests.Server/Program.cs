using Crey.Builder;
using Crey.ClientSide;
using Crey.Discovery.Consul;
#if !DEBUG
using Crey.Discovery;
using Crey.Discovery.StaticList;
#endif
using Crey.Extensions;
using Crey.Tests.Contracts;
using Crey.Service;
using Crey.Exceptions;
using System.Diagnostics;
using Crey.Discovery;

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

        Console.WriteLine("Service started");

        var proxyFactory = ClientBuilder.Create(host.Services).GetProxyFactory();

        var contract = proxyFactory.Proxy<ITestContract>();

        while (true)
        {
            try
            {
                var cmd = Console.ReadLine();
                if (cmd == "exit") break;

                if (cmd?.StartsWith("sw") == true)
                {
                    var finder = host.Services.GetRequiredService<IServiceFinder>();

                    var sw = new Stopwatch();
                    sw.Start();
                    await finder.QueryServices(typeof(ITestContract));
                    sw.Stop();
                    Console.WriteLine($"Elapsed: {sw.ElapsedMilliseconds}ms");
                    continue;
                }

                if (cmd?.StartsWith("one:") == true)
                {
                    var command = cmd.Replace("one:", "");

                    await contract.InvokeOneWay((x) => x.Ping(command));
                    continue;
                }


                if (cmd?.StartsWith("client:") == true)
                {
                    var command = cmd.Replace("client:", "");

                    var c = await GetStandaloneClient();

                    var r2 = await c.Ping("Hello from client");
                    Console.WriteLine(r2);
                    continue;
                }

                if (cmd?.StartsWith("chain:") == true)
                {
                    var command = cmd.Replace("chain:", "");

                    var r3 = await contract.InvokeChain(command);
                    Console.WriteLine(r3);
                    continue;
                }

                if (cmd?.StartsWith("throw") == true)
                {
                    var command = cmd.Replace("client:", "");

                    await contract.Throw();
                    continue;
                }

                var result = await contract.Ping("Hello");
                Console.WriteLine(result);
            }
            catch (FaultException<NotImplementedException> ex)
            {
                Console.WriteLine($"Caught {nameof(NotImplementedException)}: {ex.Message}");
            }
            catch (AggregateException ex)
            {
                Console.WriteLine($"Aggregate: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
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
                    x.Server = "http://localhost:8500";
                })
#endif
                .AddMicroClient();
        })
        .GetProxyFactory();

        var contract = proxyFactory.Proxy<ITestContract>();

        return Task.FromResult(contract);
    }
}