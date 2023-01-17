using Crey.Contracts;

namespace Crey.Tests.Contracts;

public interface ITestContract : IMicroService
{
    Task<string> InvokeFast();

    Task<string> Say(string name);
}
