using Crey.CallContext;
using Crey.Clients;
using Crey.Proxy;
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

    public Task<string> InvokeFast()
    {
        return Task.FromResult("Hello world");
    }

    public async Task<string> Say(string message)
    {
        var contract = _proxyFactory.Create<ITestContract>();

        var s = await contract.InvokeFast();

        //await Task.Delay(3000);

        //var res = _callContextAccessor.GetValue<bool>(MicroConstants.LongRunning);
        //_logger.LogWarning($"Server says: {name}, longRunning: {res}");

        return await Task.FromResult($"pong: {message}");
    }
}
