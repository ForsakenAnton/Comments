// Ignore Spelling: Cors

using Comments.Server.Data;
using Comments.Server.Data.Entities;
using Microsoft.AspNetCore.Identity;

namespace Comments.Server.Extensions;

public static class ServiceExtensions
{
    public static void ConfigureCors(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("CorsPolicy", builder =>
                builder
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader());
        });
    }

    public static void ConfigureIdentity(this IServiceCollection services)
    {
        var builder = services.AddIdentity<User, IdentityRole>(o =>
        {
            o.User.RequireUniqueEmail = true;
            // just for quick testing
            o.Password.RequiredLength = 1;
            o.Password.RequireDigit = false;
            o.Password.RequireLowercase = false;
            o.Password.RequireUppercase = false;
            o.Password.RequireNonAlphanumeric = false;
        })
        .AddEntityFrameworkStores<CommentsDbContext>()
        .AddDefaultTokenProviders();
    }
}
