using Crey.CallContext;
using Crey.Proxy;
using Crey.Service;
using Crey.Tests.Contracts;

namespace Crey.Tests.Server;

public class TestService : ITestContract
{
    private readonly ILogger<TestService> _logger;
    private readonly ICallContextAccessor _callContextAccessor;
    private readonly IProxyFactory _proxyFactory;

    public TestService(ILogger<TestService> logger, ICallContextAccessor callContextAccessor, IProxyFactory proxyFactory)
    {
        _logger = logger;
        _callContextAccessor = callContextAccessor;
        _proxyFactory = proxyFactory;
    }

    [MethodFilter<OneWayMethodFilter>]
    public async Task PingOneWay(string message)
    {
        await Task.Delay(3000);

        throw new NotImplementedException("test exception");
    }

    public async Task<string> Ping(string message)
    {
        //await Task.Delay(3000);

        //var res = _callContextAccessor.GetValue<bool>(MicroConstants.LongRunning);
        //_logger.LogWarning($"Server says: {name}, longRunning: {res}");

        return await Task.FromResult($"pong: {message}");
    }

    public async Task<string> InvokeChain(string name)
    {
        var contract = _proxyFactory.Proxy<ITestContract>();

        var s = await contract.Ping(name);

        return $"{s} -> {nameof(InvokeChain)} -> {nameof(Ping)}";
    }

    public Task Throw()
    {
        throw new NotImplementedException("test exception");
    }
}
