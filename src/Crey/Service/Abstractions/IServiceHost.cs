namespace Crey.Service;

public interface IServiceHost
{
    Task Start(ServiceAddress serviceAddress);

    Task Stop();
}
