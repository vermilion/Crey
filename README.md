**Crey - netstandard2.0 microservice framework**

![workflow](https://img.shields.io/github/actions/workflow/status/vermilion/Crey/build-and-publish.yml) ![GitHub code size in bytes](https://img.shields.io/github/languages/code-size/vermilion/Crey?style=flat-square) [![nuget](https://img.shields.io/nuget/v/Crey.svg?style=flat-square)](https://www.nuget.org/packages/Crey) [![stats](https://img.shields.io/nuget/dt/Crey.svg?style=flat-square)](https://www.nuget.org/stats/packages/Crey?groupby=Version)  

### How to install

```code
PM> Install-Package Crey
```

- [Technologies](#technologies)
- [Quick Start](#quick-start)
  - [Contract](#contract)
  - [Server](#server)
  - [Client](#client)
- [Extensions](#extensions)
  - [OneWay](#oneway---invoke-tasks-in-fire-and-forget-manner)
  - [ICallContextAccessor](#icallcontextaccessor---getting-the-call-context-from-anywhere)
  - [Client / Server Middleware](#middlewares)
- [Service Discovery](#servicediscovery)
  - [Consul](#consul)
  - [Static List](#static-list)
- [Roadmap](#roadmap)
- [Benchmark](#benchmark)

## Technologies
- **DotNetty** (transport layer)
- **Consul** / **Static list** (service discovery)
- **MessagePack** (message serialization)
- **Castle.Core** (for building DynamicProxy)
- **Polly** (retry strategy)

## Quick Start

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
    .ConfigureServices((context, services) =>
    {
        var builder = new MicroBuilder(context.Configuration, services);

        builder
            // choose either static discovery list or or Consul based one
            //.AddStaticListDiscovery()
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
  "Micro": {
    "Service": {
      "Host": "192.168.1.24", // define host IP
      "Port": 5003 // port to bind to
    },
    "Discovery": {
      "Consul": {
        "Server": "http://localhost:8500", // consul address
        "Token": ""
      }
    }
  }
}

```

### Client
``` c#
// create using existing IServiceProvider instance
var proxyFactory = ClientBuilder.Create(provider).CreateProxyFactory();

// OR without it (IServiceProvider is created internally)
var proxyFactory = ClientBuilder.Create(builder =>
{
    builder
        // choose either static discovery list or or Consul based one
        //.AddStaticListDiscovery(x =>
        //{
        //    x.Set<ITestContract>(new[] { new ServiceAddress("192.168.1.24", 5003) });
        //})
        .AddConsulDiscovery(x =>
        {
            x.Server = "http://localhost:8500";
        })
        .AddMicroClient();
})
.CreateProxyFactory();

// creating contract proxy instance
var contract = proxyFactory.Create<ITestContract>();

// use it like common .net method (RPC)
var res = await contract.Say("Hello world");
```

**Configuration**
```json
{
  "Micro": {
    "Discovery": {
      "Consul": {
        "Server": "http://localhost:8500", // consul address
        "Token": ""
      }
    }
  }
}
```

## Extensions

### OneWay - Invoke tasks in fire-and-forget manner

```csharp
// creating contract proxy instance
var contract = proxyFactory.Create<ITestContract>();

// use Extension on contract instance
await contract.InvokeOneWay((x) => x.Say("Hello OneWay"));
```

### ICallContextAccessor - Getting the call context from anywhere

To access context anywhere in code, inject an instance of `ICallContextAccessor` in your constructor

```csharp
public class TestService
{
    public TestService(ICallContextAccessor callContextAccessor)
    {
    }
}
```

### Middlewares

Cross-cutting concepts can be implemented using Middlewares `IClientMiddleware` and `IServiceMiddleware`

First, implement these interfaces. Then register these in client or server builders

```csharp
builder.AddMiddleware<TMiddleware>();
```

## Service Discovery

**Following providers are available out of the box:**

___Configured under `discovery` tag in configuration___

### Consul

Consul provider can be configured with these values
- Tags
- Metadata
- Check options
```json
"Consul": {
  "Server": "http://localhost:8500",
  "Token": "",
  "Service": {
  "Tags": [
    "DEV"
  ],
  "Meta": {
    "Environment": "Development"
  },
  "Check": {
    "DeregisterCriticalServiceAfterDays": 0,
    "Timeout": 5,
    "Interval": 1
  }
}
```

### Static List

Static List provider can be configured with these values

```json
"Static": {
  "Crey.Tests.Contracts_v1": [
    "Host": "localhost",
    "Port": 5003,
    "Weight": 1 // optional, used in weighted random algorithm
  ]
}
```

Where `Key` = `$"{AssemblyNamespace}_v{AssemblyVersion.Major}"`

## Roadmap
- Tests
- Check internal services override possibility
- Kestrel hosting with `ConnectionHandler`

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