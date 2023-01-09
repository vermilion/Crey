using Crey.Micro.Extensions;
using Crey.Micro.Constants;
using Crey.Tests.Contracts;
using Crey.Session.Abstractions;

namespace Crey.Tests.Server.Services;

public class TestService : ITestContract
{
    private readonly ILogger<TestService> _logger;
    private readonly ISessionValuesAccessor _callContextAccessor;

    public TestService(ILogger<TestService> logger, ISessionValuesAccessor callContextAccessor)
    {
        _logger = logger;
        _callContextAccessor = callContextAccessor;
    }

    public async Task<string> Say(string name)
    {
        //await Task.Delay(3000);

        //var res = _callContextAccessor.GetValue<bool>(MicroConstants.LongRunning);
        //_logger.LogWarning($"Server says: {name}, longRunning: {res}");

        return await Task.FromResult($"get name:{name}");
    }
}
