namespace LiteNetLibDebugApp;

public class OptionBase
{

}
public sealed class AppOptions : OptionBase
{
    public bool ClientEnabled { get; set; } = true;
    public bool ServerEnabled { get; set; } = true;
}
