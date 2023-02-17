using Crey.Messages;
using Crey.Service;

namespace Crey.Tests.Server;

internal class ServiceMiddleware : IServiceMiddleware
{
    public Task Execute(MessageInvoke message, NextServiceDelegate next)
    {
        return next();
    }
}
