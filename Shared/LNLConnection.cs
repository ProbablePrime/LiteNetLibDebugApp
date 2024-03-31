using LiteNetLib;
using LiteNetLib.Utils;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Sockets;

namespace LiteNetLibDebugApp;


/// <summary>
/// Global options for all LNL connections in this application
/// </summary>
public class LNLConnectionOptions
{
    /// <summary>
    /// What port should the LNL connection use for the Local End of the connection?
    /// </summary>
    public int LocalPort { get; set; } = 0;

    /// <summary>
    /// Should this connection be enabled?
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <inheritdoc cref="NetManager.DisconnectTimeout"/>
    public int DisconnectTimeout { get; set; } = 30000; // 30k

    /// <inheritdoc cref="NetManager.UseNativeSockets"/>
    public bool UseNativeSockets { get; set; } = true;

    /// <inheritdoc cref="NetManager.ChannelsCount"/>
    public byte ChannelCount { get; set; } = 1;

    /// <inheritdoc cref="NetManager.UpdateTime"/>
    public int UpdateTime { get; set; } = 5;
}
public class LNLConnection : INetEventListener
{
    protected readonly ILogger Log;
    protected readonly NetManager netManager;

    protected LNLConnectionOptions Options;

    public IEnumerable<NetPeer> Peers => netManager;

    public delegate void MessageEventHandler(NetPeer sender, string message);

    public event MessageEventHandler? MessageRecieved = null;
    public LNLConnection(ILogger log, IOptions<LNLConnectionOptions> ioptions)
    {
        Options = ioptions.Value;
        Log = log;

        netManager = new NetManager(this)
        {
            UnsyncedEvents = true,
            UnconnectedMessagesEnabled = true,

            UseNativeSockets = Options.UseNativeSockets,
            DisconnectTimeout = Options.DisconnectTimeout,
            ChannelsCount = Options.ChannelCount,
            UpdateTime = Options.UpdateTime,
        };

        Log.LogInformation("Starting LNL Listener on: {port}", Options.LocalPort);
        netManager.Start(Options.LocalPort);
    }
    public void OnConnectionRequest(ConnectionRequest request)
    {
        Log.LogInformation("Connection request from: {request}", request);
        request.AcceptIfKey(Constants.CONNECTION_KEY);
    }

    public void OnNetworkError(IPEndPoint endPoint, SocketError socketError)
    {
        Log.LogError("Network error at {endpoint}, with error: {error}", endPoint, socketError);
    }

    public void OnNetworkLatencyUpdate(NetPeer peer, int latency)
    {
        // Ignore
    }

    public void OnNetworkReceive(NetPeer peer, NetPacketReader reader, byte channelNumber, DeliveryMethod deliveryMethod)
    {
        var message = reader.GetString();
        MessageRecieved?.Invoke(peer, message);
    }

    public void OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType)
    {
        // Ignore
    }

    public void OnPeerConnected(NetPeer peer)
    {
        Log.LogInformation("{peer} connected!", peer);
    }

    public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
    {
        Log.LogInformation("{peer} disconnected!", peer);
    }

    // Just basic Text for now
    public void Send(int id, string line)
    {
        var peer = netManager.GetPeerById(id);
        if (peer == null)
            return;

        Send(peer, line);
    }

    public static void Send(NetPeer peer, string line)
    {
        NetDataWriter writer = new();
        writer.Put(line);

        peer.Send(writer, DeliveryMethod.ReliableOrdered);
    }
}
