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

internal class ClientService : BackgroundService
{
    private LNLClient _client;
    private ClientServiceOptions Options;
    private ILogger Log;
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

    public void Start()
    {
        var address = IPAddress.Parse(Options.RemoteAddress);
        endpoint = new IPEndPoint(address, Options.RemotePort);

        Log.LogInformation("ClientService Running , connecting to {remoteAddress}:{remotePort}", Options.RemoteAddress, Options.RemotePort);

        _client.Connect(endpoint);
    }

    public void Send(string message)
    {
        _client.Send(message);
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Start();
        return Task.CompletedTask;
    }
}
