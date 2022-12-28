using Psi.Micro.Abstractions;

namespace Psi.Tests.Contracts;

public interface ITestContract : IMicroService
{
    Task<string> Say(string name);
}
