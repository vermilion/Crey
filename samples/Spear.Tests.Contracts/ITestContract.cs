using Spear.Core.Micro.Abstractions;

namespace Spear.Tests.Contracts;

public interface ITestContract : IMicroService
{
    Task<string> Say(string name);
}
