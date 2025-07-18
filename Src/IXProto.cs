using Xcsb.Models;
using Xcsb.Models.Event;
using Xcsb.Models.Handshake;
using Xcsb.Models.Response;

namespace Xcsb;

public interface IXProto : IResponseProto, IVoidProto, IVoidProtoChecked, IDisposable
{
    HandshakeSuccessResponseBody HandshakeSuccessResponseBody { get; }
    IXBufferProto BufferClient { get; }

    /// <summary>
    ///     Retrieves the next event from the X server's event queue, if available.
    /// </summary>
    /// <returns>
    ///     An <see cref="XEvent" /> instance representing the next event in the queue,
    ///     or <c>null</c> if there are no more events or if the connection should be closed.
    /// </returns>
    /// <remarks>
    ///     This method processes and returns the next available event from the X server's event queue.
    ///     A return value of <c>null</c> indicates that either no events are currently available,
    ///     or that the X server connection is no longer valid and should be terminated.
    ///     <para>
    ///         To determine the specific type of event, inspect the <see cref="XEvent.EventType" /> property.
    ///     </para>
    /// </remarks>
    /// <exception cref="XProtocolError">
    ///     Thrown when a protocol-level error occurs while attempting to retrieve the event from the X server.
    /// </exception>
    XEvent? GetEvent();
    Task<XEvent?> GetEventAsync();
    bool IsEventAvailable();
    void WaitForEvent();
    uint NewId();
    
    
    
}