using Contracts;
using Microsoft.Extensions.Logging;

namespace LoggerService;

public class LoggerManager : ILoggerManager
{
    public static readonly ILoggerFactory MyLoggerFactory = LoggerFactory.Create(builder =>
    {
        builder.AddConsole();
    });

    public static readonly ILogger<LoggerManager> logger =
        MyLoggerFactory.CreateLogger<LoggerManager>();

    public LoggerManager()
    {
    }

    public void LogDebug(string message) => logger.LogDebug(message);

    public void LogError(string message) => logger.LogError(message);

    public void LogInfo(string message) => logger.LogInformation(message);

    public void LogWarn(string message) => logger.LogWarning(message);
}