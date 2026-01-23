namespace Xcsb.Configuration;

public class XcsbClientConfiguration
{
    public static XcsbClientConfiguration Default => new XcsbClientConfiguration()
    {
    };
    public const int StackAllocThreshold = 1024;
    public ActionDelegates.SendAction? OnSendRequest { get; set; }
    public ActionDelegates.ReceivedAction? OnReceivedReply { get; set; }
    public bool ShouldCrashOnFailConnection { get; set; }
}