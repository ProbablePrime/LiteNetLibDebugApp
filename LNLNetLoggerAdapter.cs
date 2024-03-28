using LiteNetLib;
using Microsoft.Extensions.Logging;

namespace LiteNetLibDebugApp;

public class LNLNetLoggerAdapter : INetLogger
{
    private readonly ILogger Log;
    private Dictionary<NetLogLevel, LogLevel> mapper = new() {
        { NetLogLevel.Info, LogLevel.Information },
        {NetLogLevel.Error, LogLevel.Error },
        {NetLogLevel.Warning, LogLevel.Warning },
        // Trace is problematic in LogLevel land, redirect trace to Debug for the moment
        {NetLogLevel.Trace, LogLevel.Debug },
    };

    private LogLevel MapLevel(NetLogLevel logLevel)
    {
        if (mapper.ContainsKey(logLevel))
            return mapper[logLevel];

        return LogLevel.Information;
    }
    public void WriteNet(NetLogLevel level, string str, params object[] args)
    {
        Log.Log(MapLevel(level), str, args);
    }

    public LNLNetLoggerAdapter(ILoggerFactory loggerFactory)
    {
        Log = loggerFactory.CreateLogger("LNL NetLogger");

        Log.LogInformation("Relaying LNL Messages");

        // Register this with LNL Library.
        NetDebug.Logger = this;
    }
}
