namespace Xcsb.Connection.Configuration;

public class ActionDelegates
{
    public delegate void SendAction(scoped in ReadOnlySpan<byte> request);

    public delegate void ReceivedAction(scoped in ReadOnlySpan<byte> response);
}