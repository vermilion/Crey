using Crey.Clients;
using Crey.Messages;

namespace Crey.Tests.Server;

internal class ClientMiddleware : IClientMiddleware
{
    public Task<MessageResult> Execute(MessageInvoke message, ClientHandlerDelegate next)
    {
        return next();
    }
}
