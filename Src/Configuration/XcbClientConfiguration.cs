using Xcsb.Helpers;

namespace Xcsb.Configuration;

public class XcbClientConfiguration
{
    public static XcbClientConfiguration Default => new XcbClientConfiguration()
    {
        OnSendRequest = (socket, socketFlags, scoped in data) =>
        {
          socket.SendExact(data, socketFlags);  
        },
    };
    public const int StackAllocThreshold = 1024;
    public ActionDelegates.SendAction? OnSendRequest { get; set; }
    public ActionDelegates.ReceivedAction? OnReceivedReply { get; set; }
}