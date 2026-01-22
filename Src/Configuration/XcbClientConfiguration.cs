namespace Xcsb.Configuration;

public class XcbClientConfiguration
{
    public static XcbClientConfiguration Default => new XcbClientConfiguration()
    {
    };
    public const int StackAllocThreshold = 1024;
    public ActionDelegates.SendAction? OnSendRequest { get; set; }
    public ActionDelegates.ReceivedAction? OnReceivedReply { get; set; }
}