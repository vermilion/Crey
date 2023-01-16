namespace Crey.Transport;

public interface ITransportListener : IMessageListener
{
    Task Start(ServiceAddress serviceAddress);

    Task Stop();
}
