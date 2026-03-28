using LiteNetLib;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net;

namespace LiteNetLibDebugApp;

public sealed class ClientServiceOptions : LNLClientOptions
{
    public int RemotePort { get; set; } = 0;
    public string RemoteAddress { get; set; } = string.Empty;
}

public class ClientService : BackgroundService
{
    private readonly LNLClient _client;
    private readonly ClientServiceOptions Options;
    private readonly ILogger Log;
    private IPEndPoint? endpoint = null;

    public ClientService(ILoggerFactory logF, IOptions<ClientServiceOptions> options)
    {
        Log = logF.CreateLogger(nameof(ClientService));
        _client = new LNLClient(Log, options);
        _client.MessageRecieved += MessageRecieved;
        Options = options.Value;
    }

    private void MessageRecieved(NetPeer sender, string message)
    {
        Log.LogInformation("Recieved message from {sender} message {message}", sender, message);
    }

    public async Task<bool> Start()
    {
        var address = IPAddress.Parse(Options.RemoteAddress);
        endpoint = new IPEndPoint(address, Options.RemotePort);

        Log.LogInformation("ClientService Running, connecting to {remoteAddress}:{remotePort}", Options.RemoteAddress, Options.RemotePort);

        var tcs = new TaskCompletionSource<bool>();

        _client.PeerConnected += peer =>
        {
            tcs.TrySetResult(true);
        };

        _client.PeerDisconnected += (peer, disconnectInfo) =>
        {
            if (disconnectInfo.Reason == DisconnectReason.ConnectionFailed)
                tcs.TrySetResult(false);
        };

        _client.Connect(endpoint);

        var result = await tcs.Task;

        if (result)
            Log.LogInformation("Successfully connected to server");
        else
            Log.LogError("Failed to connect to server");

        return result;
    }

    public void Send(string message)
    {
        _client.Send(message);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var connected = await Start();
        if (!connected)
            return;

        await Task.Delay(Timeout.Infinite, stoppingToken);
    }
}