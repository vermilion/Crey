using Crey.Messages;
using Crey.Service;

namespace Crey.Tests.Server;

public class TestMethodFilter : IServiceMethodFilter
{
    public Task Execute(MessageInvoke message, NextServiceDelegate next)
    {
        return next();
    }
}