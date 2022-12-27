namespace Spear.Core.Session.Models;

public class SessionDto
{
    public object UserId { get; set; }
    public string UserName { get; set; }
    public string Role { get; set; }

    public SessionDto(object userId = null)
    {
        UserId = userId;
    }
}
