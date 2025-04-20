// Ignore Spelling: Cors Sql env

using Contracts;
using LoggerService;
using Repository;
using Service.Contracts;
using Service;
using Microsoft.EntityFrameworkCore;
using Shared.Options;

namespace Comments.Server.Extensions;

public static class ServiceExtensions
{
    public static void ConfigureCors(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("CorsPolicy", builder =>
                builder
                    .WithOrigins("http://localhost:5173")
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials()
                    .WithExposedHeaders("X-Pagination"));
        });
    }

    public static void ConfigureIOptions(this IServiceCollection services, IWebHostEnvironment env)
    {
        services.Configure<FileStorageOptions>(options =>
        {
            options.WebRootPath = env.WebRootPath;
        });
    }

    public static void ConfigureSqlContext(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        string sqlConnection = configuration.GetConnectionString("sqlConnection")!;

        services.AddDbContext<RepositoryContext>(opts =>
            opts.UseSqlServer(sqlConnection));
    }

    public static void ConfigureLoggerService(this IServiceCollection services)
    {
        services.AddSingleton<ILoggerManager, LoggerManager>();
    }

    public static void ConfigureRepositoryManager(this IServiceCollection services)
    {
        services.AddScoped<IRepositoryManager, RepositoryManager>();
    }

    public static void ConfigureServiceManager(this IServiceCollection services)
    {
        services.AddScoped<IServiceManager, ServiceManager>();
    }
}
