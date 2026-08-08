namespace Xcsb.Connection.Configuration;

public static class ActionDelegates
{
    public delegate void SendAction(scoped ReadOnlySpan<byte> request);

    public delegate void ReceivedAction(scoped ReadOnlySpan<byte> response);

    public delegate string ErrorMessageAction(Span<byte> data);
}