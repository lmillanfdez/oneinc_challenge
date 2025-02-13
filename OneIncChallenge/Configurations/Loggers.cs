using Serilog;

namespace OneIncChallenge.Configurations;

internal static class Loggers
{
    internal static void ConfiguringSerilog()
    {
        Log.Logger = new LoggerConfiguration()
                        .WriteTo.File(Path.Combine(Directory.GetCurrentDirectory(), "Logs", $"{DateTime.Now.Year}-{DateTime.Now.Month}-{DateTime.Now.Day}.txt"),
                            rollingInterval: RollingInterval.Infinite,
                            outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff} {Message}{NewLine}{Exception}")
                        .CreateLogger();
    }
}