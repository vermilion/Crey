using Psi.Micro.Extensions;
using Psi.Micro.Constants;
using Psi.Micro.Abstractions;
using Psi.Tests.Contracts;

namespace Psi.Tests.Server.Services;

public class TestService : ITestContract
{
    private readonly ILogger<TestService> _logger;
    private readonly IMicroSession _session;

    public TestService(ILogger<TestService> logger, IMicroSession session)
    {
        _logger = logger;
        _session = session;
    }

    public async Task<string> Say(string name)
    {
        await Task.Delay(3000);

        var res = _session.GetValue<bool>(MicroConstants.LongRunning);
        _logger.LogWarning($"Server says: {name}, longRunning: {res}");

        return await Task.FromResult($"get name:{name}");
    }
}
