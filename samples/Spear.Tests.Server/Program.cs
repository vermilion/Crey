using Spear.Codec.MessagePack.Extensions;
using Spear.Core.Builder;
using Spear.Core.Builder.Extensions;
using Spear.Core.Session.Extensions;
using Spear.Discovery.Consul.Extensions;
using Spear.Protocol.Tcp.Extensions;
using Spear.ProxyGenerator.Abstractions;
using Spear.Tests.Contracts;
using Spear.Tests.Server.Services;

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
                        .AddSession()
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

            host.RunAsync();

            var provider = host.Services;

            await Task.Delay(10000);

            var proxy = provider.GetRequiredService<IProxyFactory>();
            var contract = proxy.Create<ITestContract>();

            try
            {
                var result = contract.Say("Hello");
                Console.WriteLine(result.Result);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            while (true)
            {
                var cmd = Console.ReadLine();
                if (cmd == "exit") break;

                var result = contract.Say(cmd);
                Console.WriteLine(result.Result);
            }
        }
    }
}
