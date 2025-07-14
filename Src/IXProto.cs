using System.Numerics;
using System.Runtime.CompilerServices;
using Xcsb.Masks;
using Xcsb.Models;
using Xcsb.Models.Event;
using Xcsb.Models.Handshake;
using Xcsb.Models.Response;

namespace Xcsb;

public interface IXProto : IVoidProto, IVoidProtoChecked, IDisposable
{
    HandshakeSuccessResponseBody HandshakeSuccessResponseBody { get; }
    IXBufferProto BufferCLient { get; }

    /// <summary>
    /// Retrieves the next event from the X server's event queue, if available.
    /// </summary>
    /// <returns>
    /// An <see cref="XEvent"/> instance representing the next event in the queue,
    /// or <c>null</c> if there are no more events or if the connection should be closed.
    /// </returns>
    /// <remarks>
    /// This method processes and returns the next available event from the X server's event queue.
    /// A return value of <c>null</c> indicates that either no events are currently available,
    /// or that the X server connection is no longer valid and should be terminated.
    /// <para>
    /// To determine the specific type of event, inspect the <see cref="XEvent.EventType"/> property.
    /// </para>
    /// </remarks>
    /// <exception cref="XProtocolError">
    /// Thrown when a protocol-level error occurs while attempting to retrieve the event from the X server.
    /// </exception>

    XEvent? GetEvent();

    uint NewId();
    AllocColorReply AllocColor(uint colorMap, ushort red, ushort green, ushort blue);
    QueryPointerReply QueryPointer(uint window);

    GrabPointerReply GrabPointer(bool ownerEvents, uint grabWindow, ushort mask, GrabMode pointerMode,
        GrabMode keyboardMode, uint confineTo, uint cursor, uint timeStamp);

    InternAtomReply InternAtom(bool onlyIfExist, string atomName);
    GetPropertyReply GetProperty(bool delete, uint window, uint property, uint type, uint offset, uint length);
    void GetWindowAttributes();
    void GetGeometry();
    void QueryTree();
    void GetAtomName();
    void ListProperties();
    void GetSelectionOwner();
    void GrabKeyboard();
    void GetMotionEvents();
    void TranslateCoordinates();
    void GetInputFocus();
    void QueryKeymap();
    void QueryFont();
    void QueryTextExtents();
    void ListFonts();
    void ListFontsWithInfo();
    void GetFontPath();
    void GetImage();
    void ListInstalledColormaps();
    void AllocNamedColor();
    void AllocColorCells();
    void AllocColorPlanes();
    void QueryColors();
    void LookupColor();
    void QueryBestSize();
    void QueryExtension();
    void ListExtensions();
    void SetModifierMapping();
    void GetModifierMapping();
    void GetKeyboardMapping();
    void GetKeyboardControl();
    void SetPointerMapping();
    void GetPointerMapping();
    void GetPointerControl();
    void GetScreenSaver();
    void ListHosts();
}