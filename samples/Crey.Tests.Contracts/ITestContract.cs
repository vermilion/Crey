using Crey.Micro.Abstractions;

namespace Crey.Tests.Contracts;

public interface ITestContract : IMicroService
{
    Task<string> Say(string name);
}
