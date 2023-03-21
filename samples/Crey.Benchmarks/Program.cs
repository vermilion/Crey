using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Crey.ClientSide;
using Crey.Discovery;
using Crey.Discovery.StaticList;
using Crey.Discovery.Consul;
using Crey.Proxy;
using Crey.Tests.Contracts;
using Crey.Core;

namespace Crey.Benchmarks;

internal class Program
{
    static void Main(string[] args)
    {
        var summary = BenchmarkRunner.Run<ClientServerBenchmark>();
    }
}

[MemoryDiagnoser]
public class ClientServerBenchmark
{
    private IProxyFactory _proxyFactory;

    [GlobalSetup]
    public void GlobalSetup()
    {
        // create separate client
        _proxyFactory = ClientBuilder.Create(builder =>
        {
            builder
#if DEBUG
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
    }

    [Benchmark]
    public Task CreateContractAndCallMethod()
    {
        var contract = _proxyFactory.Proxy<ITestContract>();

        return contract.Ping("Qwe");
    }
}