using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Spear.Codec.MessagePack;
using Spear.Consul;
using Spear.Core;
using Spear.Core.Extensions;
using Spear.Core.Micro;
using Spear.Core.Micro.Services;
using Spear.Protocol.Tcp;
using Spear.ProxyGenerator;
using Spear.Tests.Contracts;
using Spear.Tests.Server.Services;

namespace Spear.Tests.Server
{
    internal class Program
    {
        private static IServiceProvider _provider;

        private static async Task Main(string[] args)
        {
            AppDomain.CurrentDomain.ProcessExit += async (sender, eventArgs) => await Shutdown();
            Console.CancelKeyPress += async (sender, eventArgs) =>
            {
                await Shutdown();
                eventArgs.Cancel = true;
            };

            var port = -1;
            if (args.Length > 0)
                int.TryParse(args[0], out port);

            var builder = new MicroBuilder();
            builder.Services.AddLogging(builder =>
            {
                builder.SetMinimumLevel(LogLevel.Information);
                builder.AddFilter("System", level => level >= LogLevel.Warning);
                builder.AddFilter("Microsoft", level => level >= LogLevel.Warning);
                builder.AddConsole();
            });

            builder
                .AddMicroService(builder =>
                {
                    builder
                        .AddTcpProtocol()
                        .AddMessagePackCodec()
                        .AddSession()
                        //.AddDefaultRouter()
                        .AddConsul("http://192.168.1.24:8500")
                        ;
                })
                .AddMicroClient(builder =>
                {
                    builder
                        .AddTcpProtocol()
                        .AddMessagePackCodec()
                        .AddSession()
                        //.AddDefaultRouter()
                        .AddConsul("http://192.168.1.24:8500")
                        ;
                });

            builder.Services.AddSingleton<ITestContract, TestService>();

            _provider = builder.Services.BuildServiceProvider();

            await _provider.UseMicroService(address =>
            {
                var m = "micro".Config<ServiceAddress>();
                if (m == null) return;
                address.Host = m.Host;
                address.Port = port > 80 ? port : m.Port;
                if (address.Port < 80)
                    address.Port = 5000;
                address.Weight = m.Weight;
            });

            await Task.Delay(10000);

            var proxy = _provider.GetRequiredService<IProxyFactory>();
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

        private static Task Shutdown()
        {
            return _provider.GetService<IMicroHost>()?.Stop();
        }
    }
}
