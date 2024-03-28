using LiteNetLib;
using LiteNetLib.Utils;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Sockets;

namespace LiteNetLibDebugApp;

public class MessageEventArgs {
    public readonly int PeerId;
    public readonly string Message;

    public MessageEventArgs(int peerId, string message)
    {
        PeerId = peerId;
        Message = message;
    }
}

public class LNLConnectionOptions
{
    public int LocalPort { get; set; } = 0;
    public bool Enabled { get; set; } = true;
}
public class LNLConnection : INetEventListener
{
    protected readonly ILogger Log;
    protected readonly NetManager netManager;

    protected readonly Dictionary<int, NetPeer> PeerMap = new Dictionary<int, NetPeer>();

    protected LNLConnectionOptions Options;
    
    public IEnumerable<NetPeer> Peers => PeerMap.Values;

    public delegate void MessageEventHandler(NetPeer sender, string message);

    public event MessageEventHandler? MessageRecieved = null;
    public LNLConnection(ILogger log, IOptions<LNLConnectionOptions> ioptions)
    {
        Options = ioptions.Value;
        Log = log;

        netManager = new NetManager(this);
        netManager.DisconnectTimeout = 30000;
        netManager.UnsyncedEvents = true;

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
        if (!PeerMap.ContainsKey(peer.Id))
            PeerMap.Add(peer.Id, peer);
    }

    public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
    {
        Log.LogInformation("{peer} disconnected!", peer);
        if (PeerMap.ContainsKey(peer.Id))
            PeerMap.Remove(peer.Id);
    }

    // Just basic Text for now
    public void Send(int id, string line)
    {
        var found = PeerMap.TryGetValue(id, out var peer);
        if (!found || peer == null)
            return;


        Send(peer, line);
    }

    public void Send(NetPeer peer, string line)
    {
        NetDataWriter writer = new NetDataWriter();
        writer.Put(line);

        peer.Send(writer, DeliveryMethod.ReliableOrdered);
    }
}
