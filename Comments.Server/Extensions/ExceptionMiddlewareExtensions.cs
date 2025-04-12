﻿// Ignore Spelling: Middleware app

using Comments.Server.Models;
using Microsoft.AspNetCore.Diagnostics;
using System.Net;

namespace Comments.Server.Extensions;

public static class ExceptionMiddlewareExtensions
{
    internal static void ConfigureExceptionHandler(
        this WebApplication app, 
        ILogger<Program> logger)
    {
        app.UseExceptionHandler(appError =>
        {
            appError.Run(async context =>
            {
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.ContentType = "application/json";

                var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                if (contextFeature != null)
                {
                    logger.LogError($"Something went wrong: {contextFeature.Error}");

                    await context.Response.WriteAsync(new ErrorDetails()
                    {
                        StatusCode = context.Response.StatusCode,
                        Message = "Internal Server Error.",
                    }.ToString());
                }
            });
        });
    }
}
