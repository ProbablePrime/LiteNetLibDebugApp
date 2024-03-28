using LiteNetLib;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LiteNetLibDebugApp;

public sealed class ServerServiceOptions : LNLServerOptions { }

internal class ServerService : BackgroundService
{
    private LNLServer _client;
    private ServerServiceOptions Options;
    private ILogger Log;

    public ServerService(ILoggerFactory logF, IOptions<ServerServiceOptions> options)
    {
        Log = logF.CreateLogger(nameof(ServerService));

        Log.LogInformation("ServerService Running on Port: {port}", options.Value.LocalPort);

        _client = new LNLServer(Log, options);
        _client.MessageRecieved += MessageRecieved;

        Options = options.Value;
    }

    private void MessageRecieved(NetPeer sender, string message)
    {
        Log.LogInformation("Recieved message from {sender}: {message}", sender, message);

        var msg = message;

        foreach (NetPeer peer in _client.Peers)
        {
            if (peer == sender)
                msg = "Echo: " + msg;
            Log.LogInformation("Sending recieved message to {peer} message: {message}", peer, msg);
            _client.Send(peer, msg);
        }
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        //no-op
        return Task.CompletedTask;
    }
}
