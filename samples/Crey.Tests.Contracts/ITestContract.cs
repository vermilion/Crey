using Crey.Contracts;

namespace Crey.Tests.Contracts;

public interface ITestContract : IMicroService
{
    Task PingOneWay(string name);
    Task<string> Ping(string name);
    Task<string> InvokeChain(string name);
    Task Throw();
}
