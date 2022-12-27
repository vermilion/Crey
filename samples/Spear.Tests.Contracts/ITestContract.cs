using Spear.Core.Micro.Abstractions;
using Spear.Tests.Contracts.Dtos;

namespace Spear.Tests.Contracts;

public interface ITestContract : IMicroService
{
    Task Notice(string name);

    Task<string> Say(string name);

    Task<UserDto> User(UserInputDto input);
}
