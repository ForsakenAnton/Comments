using Comments.Server.Data.Entities;
using Microsoft.AspNetCore.Identity;

namespace Comments.Server.Data;

public static class DbInitializer
{
    public static async Task InitializeAsync(
        CommentsDbContext context,
        UserManager<User> userManager,
        SignInManager<User> signInManager)
    {
        
    }
}
