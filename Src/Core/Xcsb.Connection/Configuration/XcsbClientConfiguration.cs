using System.Buffers;

namespace Xcsb.Connection.Configuration;

public class XcsbClientConfiguration
{
    public static XcsbClientConfiguration Default => new();
    public const int StackAllocThreshold = 1024;
    public ActionDelegates.SendAction? OnSendRequest { get; set; }
    public ActionDelegates.ReceivedAction? OnReceivedReply { get; set; }
    public bool ShouldCrashOnFailConnection { get; set; }
    public ArrayPool<byte> BufferPool { get; set; } = ArrayPool<byte>.Shared;
}