// Ignore Spelling: Middleware app

using Comments.Server.Models;
using Comments.Server.Models.ErrorModels;
using Microsoft.AspNetCore.Diagnostics;

namespace Comments.Server.Extensions;

public static class ExceptionMiddlewareExtensions
{
    internal static void ConfigureExceptionHandler(
        this WebApplication app, 
        ILogger<Program> logger)
    {
        app.UseExceptionHandler(configure: appError =>
        {
            appError.Run(handler: async context =>
            {
                //context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.ContentType = "application/json";

                var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                if (contextFeature != null)
                {
                    context.Response.StatusCode = contextFeature.Error switch
                    {
                        BadRequestException => StatusCodes.Status400BadRequest,
                        _ => StatusCodes.Status500InternalServerError
                    };

                    logger.LogError($"Something went wrong: {contextFeature.Error}");

                    await context.Response.WriteAsync(new ErrorDetails()
                    {
                        StatusCode = context.Response.StatusCode,
                        Message = contextFeature.Error.Message,
                    }.ToString());
                }
            });
        });
    }
}
