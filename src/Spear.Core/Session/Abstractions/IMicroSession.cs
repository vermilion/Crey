namespace Spear.Core.Session.Abstractions;

public interface IMicroSession
{
    object UserId { get; }
    string UserName { get; }
    string Role { get; }
}
