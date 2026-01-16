using System.Net.Sockets;

namespace Xcsb;

public class XcbClientConfiguration
{
    public static XcbClientConfiguration Default => new XcbClientConfiguration();
    public const int StackAllocThreshold = 1024;
    public delegate void SendAction(Socket socket, scoped in ReadOnlySpan<byte> request);
    public SendAction? OnSendRequest { get; set; }
}