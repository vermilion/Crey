﻿using Crey.Micro;
using Crey.Tests.Contracts;
using Crey.Session;

namespace Crey.Tests.Server.Services;

public class TestService : ITestContract
{
    private readonly ILogger<TestService> _logger;
    private readonly ICallContextAccessor _callContextAccessor;

    public TestService(ILogger<TestService> logger, ICallContextAccessor callContextAccessor)
    {
        _logger = logger;
        _callContextAccessor = callContextAccessor;
    }

    public async Task<string> Say(string message)
    {
        //await Task.Delay(3000);

        //var res = _callContextAccessor.GetValue<bool>(MicroConstants.LongRunning);
        //_logger.LogWarning($"Server says: {name}, longRunning: {res}");

        return await Task.FromResult($"pong: {message}");
    }
}
