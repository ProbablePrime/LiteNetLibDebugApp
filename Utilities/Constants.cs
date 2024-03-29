
namespace LiteNetLibDebugApp
{
    public class Constants
    {
        public const string CONNECTION_KEY = "LNLTesting";

        public static IDictionary<string, string>? SWITCH_MAPPINGS = new Dictionary<string, string>()
        {
            {"--server", "AppOptions:ServerEnabled" },
            {"--client", "AppOptions:ClientEnabled" },
        };
    }
}
