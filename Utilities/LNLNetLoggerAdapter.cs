using LiteNetLib;
using Microsoft.Extensions.Logging;

namespace LiteNetLibDebugApp;

public class LNLNetLoggerAdapter : INetLogger
{
    private readonly ILogger Log;
    private readonly Dictionary<NetLogLevel, LogLevel> mapper = new() {
        { NetLogLevel.Info, LogLevel.Information },
        {NetLogLevel.Error, LogLevel.Error },
        {NetLogLevel.Warning, LogLevel.Warning },
        {NetLogLevel.Trace, LogLevel.Trace },
    };

    private LogLevel MapLevel(NetLogLevel logLevel)
    {
        if (mapper.TryGetValue(logLevel, out LogLevel value))
            return value;

        return LogLevel.Information;
    }
    public void WriteNet(NetLogLevel level, string str, params object[] args)
    {
#pragma warning disable CA2254 // Template should be a static expression
        Log.Log(MapLevel(level), str, args);
#pragma warning restore CA2254 // Template should be a static expression
    }

    public LNLNetLoggerAdapter(ILoggerFactory loggerFactory)
    {
        Log = loggerFactory.CreateLogger("LNL NetLogger");

        Log.LogInformation("Relaying LNL Messages");

        // Register this with LNL Library.
        NetDebug.Logger = this;
    }
}
