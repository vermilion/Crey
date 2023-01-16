using Crey.Contracts;

namespace Crey.Tests.Contracts;

public interface ITestContract : IMicroService
{
    Task<string> Say(string name);
}
