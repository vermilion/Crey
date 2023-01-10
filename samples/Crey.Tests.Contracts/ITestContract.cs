using Crey.Micro;

namespace Crey.Tests.Contracts;

public interface ITestContract : IMicroService
{
    Task<string> Say(string name);
}
