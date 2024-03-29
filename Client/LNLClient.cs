using LiteNetLib;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net;

namespace LiteNetLibDebugApp;

public class LNLClientOptions: LNLConnectionOptions {}

public class LNLClient(ILogger log, IOptions<LNLClientOptions> options) : LNLConnection(log, options)
{
    private NetPeer? serverPeer;

    public bool Connect(IPEndPoint to)
    {
        Log.LogInformation("Connecting to: {to}", to);
        serverPeer = netManager.Connect(to, Constants.CONNECTION_KEY);
        return true;
    }

    public void Send(string message)
    {
        if (serverPeer == null)
            return;

        //serverPeer = netManager.Connect(endpoint, Constants.CONNECTION_KEY);

        Log.LogTrace("Server peer is: {state}", serverPeer.ConnectionState);

        Send(serverPeer, message);
    }
}
