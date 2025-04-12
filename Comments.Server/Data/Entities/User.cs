using Microsoft.AspNetCore.Identity;

namespace Comments.Server.Data.Entities;

public class User : IdentityUser
{
    public string NickName { get; set; } = "";
}
