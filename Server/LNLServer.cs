using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LiteNetLibDebugApp;

public class LNLServerOptions : LNLConnectionOptions { }
public class LNLServer(ILogger log, IOptions<LNLServerOptions> options) : LNLConnection(log, options)
{
}
