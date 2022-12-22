using Spear.Core;
using Spear.Tests.Contracts.Dtos;

namespace Spear.Tests.Contracts
{
    public interface ITestContract : ISpearService
    {
        Task Notice(string name);

        Task<string> Get(string name);

        Task<UserDto> User(UserInputDto input);
    }
}
