**Crey - netstandard2.0 microservice framework**

![workflow](https://img.shields.io/github/actions/workflow/status/vermilion/Crey/build-and-publish.yml) ![GitHub code size in bytes](https://img.shields.io/github/languages/code-size/vermilion/Crey?style=flat-square)


### How to install
```code
PM> Install-Package Crey
PM> Install-Package Crey.Protocol.Tcp
PM> Install-Package Crey.Discovery.Consul
PM> Install-Package Crey.Codec.MessagePack
```

| Package Name           | NuGet                                                                                                                                          | Downloads                                                                                                                                                             |
| ---------------------- | ---------------------------------------------------------------------------------------------------------------------------------------------- | --------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Crey                   | [![nuget](https://img.shields.io/nuget/v/Crey.svg?style=flat-square)](https://www.nuget.org/packages/Crey)                                     | [![stats](https://img.shields.io/nuget/dt/Crey.svg?style=flat-square)](https://www.nuget.org/stats/packages/Crey?groupby=Version)                                     |
| Crey.Protocol.Tcp      | [![nuget](https://img.shields.io/nuget/v/Crey.Protocol.Tcp.svg?style=flat-square)](https://www.nuget.org/packages/Crey.Protocol.Tcp)           | [![stats](https://img.shields.io/nuget/dt/Crey.Protocol.Tcp.svg?style=flat-square)](https://www.nuget.org/stats/packages/Crey.Protocol.Tcp?groupby=Version)           |
| Crey.Discovery.Consul  | [![nuget](https://img.shields.io/nuget/v/Crey.Discovery.Consul.svg?style=flat-square)](https://www.nuget.org/packages/Crey.Discovery.Consul)   | [![stats](https://img.shields.io/nuget/dt/Crey.Discovery.Consul.svg?style=flat-square)](https://www.nuget.org/stats/packages/Crey.Discovery.Consul?groupby=Version)   |
| Crey.Codec.MessagePack | [![nuget](https://img.shields.io/nuget/v/Crey.Codec.MessagePack.svg?style=flat-square)](https://www.nuget.org/packages/Crey.Codec.MessagePack) | [![stats](https://img.shields.io/nuget/dt/Crey.Codec.MessagePack.svg?style=flat-square)](https://www.nuget.org/stats/packages/Crey.Codec.MessagePack?groupby=Version) |


- [Technologies](#technologies)
- [Usage samples](#usage-samples)
  - [Contract](#contract)
  - [Server](#server)
  - [Client](#client)
- [Roadmap](#roadmap)
- [Benchmark](#benchmark)
- [Licences](#licences)


## Technologies
- **DotNetty** (transport layer)
- **Consul** (service discovery)
- **MessagePack** (message serialization)
- **Castle.Core** (for building DynamicProxy)
- **Polly** (retry strategy)

## Usage samples

### Contract
``` c#
// contract is referenced by client and service
public interface ITestContract : IMicroService
{
    Task<string> Say(string name);
}
```
### Server
``` c#
// using .net core's GenericHost
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
            // choose either static discovery list or or Consul based one
            //.AddStaticServiceDiscovery()
            .AddConsulDiscovery()

            .AddMicroService(builder =>
            {
                // register contract mapping
                builder.AddContract<ITestContract, TestService>();
            })
            .AddMicroClient();
        })
        .Build();

await host.RunAsync();

// contract implementation
public class TestService : ITestContract
{
    public Task<string> Say(string message)
    {
        return Task.FromResult($"pong: {message}");
    }
}
```
**Configuration**
```json
{
  "micro": {
    "service": {
      "host": "192.168.1.24", // define host IP
      "port": 5003 // port to bind to
    },
    "discovery": {
      "consul": {
        "server": "http://192.168.1.24:8500", // consul address
        "token": ""
      }
    }
  }
}

```

### Client
``` c#
// create using existing IServiceProvider instance
var proxyFactory = ClientBuilder.Create(provider).CreateProxyFactory();

// OR without it (created internally)
var proxyFactory = ClientBuilder.Create(builder =>
{
    builder
        .AddTcpProtocol()
        .AddMessagePackCodec()
        // choose either static discovery list or or Consul based one
        //.AddStaticServiceDiscovery(x =>
        //{
        //    x.Set<ITestContract>(new[] { new ServiceAddress("192.168.1.24", 5003) });
        //})
        .AddConsulDiscovery(x =>
        {
            x.Server = "http://192.168.1.24:8500";
        })
        .AddMicroClient();
})
.CreateProxyFactory();

// creating contract proxy instance
var contract = proxyFactory.Create<ITestContract>();

// use it like common .net method
var res = await contract.Say("Hello world");
```

**Configuration**
```json
{
  "micro": {
    "discovery": {
      "consul": {
        "server": "http://192.168.1.24:8500", // consul address
        "token": ""
      }
    }
  }
}
```

## Roadmap
- Middleware
- Tests
- ContextValuesAccessor docs
- OneWay docs
- retry policy abstraction

## Benchmark

``` ini

BenchmarkDotNet=v0.13.3, OS=Windows 11 (10.0.25193.1000)
Intel Core i7-8550U CPU 1.80GHz (Kaby Lake R), 1 CPU, 8 logical and 4 physical cores
.NET SDK=7.0.101
  [Host]     : .NET 6.0.12 (6.0.1222.56807), X64 RyuJIT AVX2
  DefaultJob : .NET 6.0.12 (6.0.1222.56807), X64 RyuJIT AVX2


```
| Method                      |     Mean |    Error |   StdDev |    Gen0 |    Gen1 | Allocated |
| --------------------------- | -------: | -------: | -------: | ------: | ------: | --------: |
| CreateContractAndCallMethod | 113.2 ms | 18.02 ms | 53.13 ms | 93.7500 | 31.2500 | 491.28 KB |

## Licences

Licenced under [MIT](LICENSE)