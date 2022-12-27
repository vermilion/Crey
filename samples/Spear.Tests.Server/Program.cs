using Spear.Codec.MessagePack;
using Spear.Consul;
using Spear.Core;
using Spear.Core.Builder;
using Spear.Protocol.Tcp;
using Spear.ProxyGenerator;
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
                        //.AddDefaultRouter()
                        .AddConsul("http://192.168.1.24:8500")

                        .AddMicroService(builder =>
                        {
                        })
                        .AddMicroClient(builder =>
                        {
                        });

                    services.AddScoped<ITestContract, TestService>();
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
