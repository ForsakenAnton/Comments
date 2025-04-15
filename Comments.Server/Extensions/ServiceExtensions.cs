﻿// Ignore Spelling: Cors Sql

using Comments.Server.Data;
using Contracts;
using LoggerService;
using Microsoft.AspNetCore.Identity;
using Repository;
using Service.Contracts;
using Service;
using Microsoft.EntityFrameworkCore;

namespace Comments.Server.Extensions;

public static class ServiceExtensions
{
    public static void ConfigureCors(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("CorsPolicy", builder =>
                builder
                    .WithOrigins("https://scalar.com")
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials()
                    .WithExposedHeaders("X-Pagination"));
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
