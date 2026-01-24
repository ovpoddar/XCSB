using Xcsb.Extension.Generic.Event.Infrastructure.ResponceProto;
using Xcsb.Extension.Generic.Event.Infrastructure.VoidProto;
using Xcsb.Models;

namespace Xcsb.Extension.Generic.Event.Infrastructure;

public interface IXProto : IResponseProto, IVoidProto, IVoidProtoChecked, IVoidProtoUnchecked
{
    IXBufferProto BufferClient { get; }

    /// <summary>
    /// Retrieves the next <see cref="XEvent"/> from the X server's event queue.
    /// </summary>
    /// <returns>
    /// An <see cref="XEvent"/> instance representing the next event received from the X server.  
    /// If the server has closed the connection or no more events are available, a sentinel
    /// <c>LastEvent</c> instance is returned instead.
    /// </returns>
    /// <remarks>
    /// Consumers should inspect the <see cref="XEvent.EventType"/> property to determine the  
    /// specific type of event received. A <c>LastEvent</c> instance indicates the end of the event  
    /// stream, which typically means the X server connection has been closed.
    /// </remarks>
    XEvent GetEvent();
    bool IsEventAvailable();
    void WaitForEvent();
}