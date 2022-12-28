using Spear.Codec.MessagePack.Extensions;
using Spear.Core.Builder;
using Spear.Discovery.Consul.Extensions;
using Spear.Micro.Extensions;
using Spear.Protocol.Tcp.Extensions;
using Spear.ProxyGenerator.Abstractions;
using Spear.Tests.Contracts;
using Spear.Tests.Server.Services;
using Spear.Core.Extensions;

namespace Spear.Tests.Server
{
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
                        //.AddStaticRouter()
                        .AddConsulDiscovery()

                        .AddMicroService(builder =>
                        {
                            builder.AddContract<ITestContract, TestService>();
                        })
                        .AddMicroClient(builder =>
                        {
                        });
                })
                .Build();

            await Task.Factory.StartNew(async () =>
            {
                await host.RunAsync();
            });

            var provider = host.Services;

            await Task.Delay(10000);

            var proxy = provider.GetRequiredService<IProxyFactory>();
            var contract = proxy.Create<ITestContract>();

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
}
