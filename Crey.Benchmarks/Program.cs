﻿using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Crey.Client;
using Crey.Micro;
using Crey.Protocol.Tcp;
using Crey.Proxy;
using Crey.Discovery.Consul;
using Crey.Tests.Contracts;
using Crey.Discovery.StaticRouter;
using Crey.Discovery;

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
#if DEBUG
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
    }

    [Benchmark]
    public Task CreateContractAndCallMethod()
    {
        var contract = _proxyFactory.Create<ITestContract>();

        return contract.Say("Qwe");
    }
}