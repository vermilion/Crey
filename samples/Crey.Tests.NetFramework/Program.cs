using Crey.ClientSide;
using Crey.Discovery.Localhost;
using Crey.Tests.Contracts;

namespace Crey.Tests.NetFramework
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var client = await GetStandaloneClient();

            var r2 = await client.Ping("Hello from client");
            Console.WriteLine(r2);

            Console.ReadLine();
        }

        private static Task<ITestContract> GetStandaloneClient()
        {
            // create standalone client
            var proxyFactory = ClientBuilder.Create(builder =>
            {
                //builder.Services.AddLogging(x => x.AddConsole());

                builder
                    .AddLocalhostDiscovery()
                    .AddMicroClient();
            })
            .GetProxyFactory();

            var contract = proxyFactory.Proxy<ITestContract>();

            return Task.FromResult(contract);
        }
    }
}
