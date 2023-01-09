using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Crey.Client;
using Crey.Codec.MessagePack.Extensions;
using Crey.Micro.Extensions;
using Crey.Protocol.Tcp.Extensions;
using Crey.Proxy.Abstractions;
using Crey.Discovery.Consul.Extensions;
using Crey.Tests.Contracts;
using Crey.ServiceDiscovery.StaticRouter.Extensions;
using Crey.ServiceDiscovery.Models;

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
                .AddTcpProtocol()
                .AddMessagePackCodec()
#if DEBUG
                .AddStaticServiceDiscovery(x =>
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
    }

    [Benchmark]
    public Task CreateContractAndCallMethod()
    {
        var contract = _proxyFactory.Create<ITestContract>();

        return contract.Say("Qwe");
    }
}