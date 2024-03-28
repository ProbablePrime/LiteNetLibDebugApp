using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LiteNetLibDebugApp;

public class LNLServerOptions : LNLConnectionOptions { }
public class LNLServer : LNLConnection
{
    public LNLServer(ILogger log, IOptions<LNLServerOptions> options) : base(log, options) { }
}
