using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using Xcsb.Connection;
using Xcsb.Connection.Handlers;
using Xcsb.Connection.Helpers;
using Xcsb.Connection.Infrastructure.Exceptions;
using Xcsb.Connection.Models;
using Xcsb.Connection.Models.Handshake;
using Xcsb.Connection.Response;
using Xcsb.Connection.Response.Contract;
using Xcsb.Extension.BigRequests;
using Xcsb.Generators;
using Xcsb.Handlers.Direct;
using Xcsb.Infrastructure;
using Xcsb.Infrastructure.Exceptions;
using Xcsb.Infrastructure.VoidProto;
using Xcsb.Masks;
using Xcsb.Models;
using Xcsb.Models.String;
using Xcsb.Models.TypeInfo;
using Xcsb.Requests;
using Xcsb.Requests.BigExtension;
using Xcsb.Response.Errors;
using Xcsb.Response.Event;
using Xcsb.Response.Replies;
using Xcsb.Response.Replies.Internals;

#if !NETSTANDARD
using System.Numerics;
#endif

namespace Xcsb.Implementation;

#if !NETSTANDARD
[SkipLocalsInit]
#endif
// https://xorg.freedesktop.org/archive/X11R7.7/doc/xproto/x11protocol.pdf
[CheckedImplementation(typeof(IVoidProto))]
[UncheckedImplementation(typeof(IVoidProto))]
[BaseImplementation(typeof(IVoidProto))]
internal sealed partial class XProto : IXProto
{
    private XBufferProto? _xBufferProto;
    private static uint _bigRequestLength = 262140;
    private readonly ISocketAccessor _socketAccessor;
    private readonly IXExtensionInternal _extensionInternal;

    public IXBufferProto BufferClient => _xBufferProto ??= new XBufferProto(_socketAccessor);

    public XProto(IXConnectionInternal connection)
    {
        if (connection.HandshakeStatus is not HandshakeStatus.Success ||
            connection.HandshakeSuccessResponseBody is null)
            throw new UnauthorizedAccessException(connection.FailReason);

        if (connection.Extension is not IXExtensionInternal extensionInternal)
            throw new InvalidOperationException();

        _extensionInternal = extensionInternal;
        Resister(_extensionInternal);
        _socketAccessor = connection.Accessor;
    }

    private static void Resister(IXExtensionInternal extension)
    {
        // error
        extension.RegisterError<AccessError>(0, ErrorCode.Access);
        extension.RegisterError<AllocError>(0, ErrorCode.Alloc);
        extension.RegisterError<AtomError>(0, ErrorCode.Atom);
        extension.RegisterError<ColormapError>(0, ErrorCode.Colormap);
        extension.RegisterError<CursorError>(0, ErrorCode.Cursor);
        extension.RegisterError<DrawableError>(0, ErrorCode.Drawable);
        extension.RegisterError<FontError>(0, ErrorCode.Font);
        extension.RegisterError<GcContextError>(0, ErrorCode.GcContext);
        extension.RegisterError<IDChoiceError>(0, ErrorCode.IdChoice);
        extension.RegisterError<ImplementationError>(0, ErrorCode.Implementation);
        extension.RegisterError<LengthError>(0, ErrorCode.Length);
        extension.RegisterError<MatchError>(0, ErrorCode.Match);
        extension.RegisterError<NameError>(0, ErrorCode.Name);
        extension.RegisterError<PixmapError>(0, ErrorCode.Pixmap);
        extension.RegisterError<RequestError>(0, ErrorCode.Request);
        extension.RegisterError<ValueError>(0, ErrorCode.Value);
        extension.RegisterError<WindowError>(0, ErrorCode.Window);

        // event
        extension.RegisterX1Event<ButtonPressEvent>(EventType.ButtonPress);
        extension.RegisterX1Event<ButtonReleaseEvent>(EventType.ButtonRelease);
        extension.RegisterX1Event<CirculateNotifyEvent>(EventType.CirculateNotify);
        extension.RegisterX1Event<CirculateRequestEvent>(EventType.CirculateRequest);
        extension.RegisterX1Event<ClientMessageEvent>(EventType.ClientMessage);
        extension.RegisterX1Event<ColorMapNotifyEvent>(EventType.ColormapNotify);
        extension.RegisterX1Event<ConfigureNotifyEvent>(EventType.ConfigureNotify);
        extension.RegisterX1Event<ConfigureRequestEvent>(EventType.ConfigureRequest);
        extension.RegisterX1Event<DestroyNotifyEvent>(EventType.DestroyNotify);
        extension.RegisterX1Event<EnterNotifyEvent>(EventType.EnterNotify);
        extension.RegisterX1Event<ExposeEvent>(EventType.Expose);
        extension.RegisterX1Event<FocusInEvent>(EventType.FocusIn);
        extension.RegisterX1Event<FocusOutEvent>(EventType.FocusOut);
        extension.RegisterX1Event<GraphicsExposeEvent>(EventType.GraphicsExpose);
        extension.RegisterX1Event<GravityNotifyEvent>(EventType.GravityNotify);
        extension.RegisterX1Event<KeymapNotifyEvent>(EventType.KeymapNotify);
        extension.RegisterX1Event<KeyPressEvent>(EventType.KeyPress);
        extension.RegisterX1Event<KeyReleaseEvent>(EventType.KeyRelease);
        extension.RegisterX1Event<LeaveNotifyEvent>(EventType.LeaveNotify);
        extension.RegisterX1Event<MapNotifyEvent>(EventType.MapNotify);
        extension.RegisterX1Event<MappingNotifyEvent>(EventType.MappingNotify);
        extension.RegisterX1Event<MapRequestEvent>(EventType.MapRequest);
        extension.RegisterX1Event<MotionNotifyEvent>(EventType.MotionNotify);
        extension.RegisterX1Event<NoExposeEvent>(EventType.NoExpose);
        extension.RegisterX1Event<PropertyNotifyEvent>(EventType.PropertyNotify);
        extension.RegisterX1Event<ReParentNotifyEvent>(EventType.ReParentNotify);
        extension.RegisterX1Event<ResizeRequestEvent>(EventType.ResizeRequest);
        extension.RegisterX1Event<SelectionClearEvent>(EventType.SelectionClear);
        extension.RegisterX1Event<SelectionNotifyEvent>(EventType.SelectionNotify);
        extension.RegisterX1Event<SelectionRequestEvent>(EventType.SelectionRequest);
        extension.RegisterX1Event<UnMapNotifyEvent>(EventType.UnMapNotify);
        extension.RegisterX1Event<VisibilityNotifyEvent>(EventType.VisibilityNotify);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WaitForEvent() =>
        _socketAccessor.WaitForEventArrival();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsEventAvailable() =>
        _socketAccessor.HasEventToProcesses();

    public ReplyLease<AllocColorReply> AllocColor(uint colorMap, ushort red, ushort green, ushort blue)
    {
        var cookie = AllocColorBase(colorMap, red, green, blue);
        var (result, mapping) =
            this._socketAccessor.SocketIn.ReceivedResponseSpan<AllocColorReply, AllocColorReply>(cookie.Id);

        if (mapping.ResponseType == XResponseType.Reply)
            return new ReplyLease<AllocColorReply>(result, this._socketAccessor.SocketIn.BufferPool);

        var error = result.ToStruct<XResponse>();
        this._socketAccessor.SocketIn.BufferPool.Return(result);
        throw new XEventException(new GenericError(error, mapping.ErrorMessageAction!));
    }

    public ReplyLease<AllocColorCellsReply> AllocColorCells(bool contiguous, uint colorMap, ushort colors,
        ushort planes)
    {
        var cookie = AllocColorCellsBase(contiguous, colorMap, colors, planes);
        var (result, mapping) = this._socketAccessor.SocketIn
            .ReceivedResponseSpan<AllocColorCellsReply, AllocColorCellsResponse>(cookie.Id);

        if (mapping.ResponseType == XResponseType.Reply)
            return new ReplyLease<AllocColorCellsReply>(result, this._socketAccessor.SocketIn.BufferPool);

        var error = result.ToStruct<XResponse>();
        this._socketAccessor.SocketIn.BufferPool.Return(result);
        throw new XEventException(new GenericError(error, mapping.ErrorMessageAction!));
    }

    public ReplyLease<AllocColorPlanesReply> AllocColorPlanes(bool contiguous, uint colorMap, ushort colors,
        ushort reds, ushort greens, ushort blues)
    {
        var cookie = AllocColorPlanesBase(contiguous, colorMap, colors, reds, greens, blues);
        var (result, mapping) = this._socketAccessor.SocketIn
            .ReceivedResponseSpan<AllocColorPlanesReply, AllocColorPlanesResponse>(cookie.Id);
        if (mapping.ResponseType == XResponseType.Reply)
            return new ReplyLease<AllocColorPlanesReply>(result, this._socketAccessor.SocketIn.BufferPool);

        var error = result.ToStruct<XResponse>();
        this._socketAccessor.SocketIn.BufferPool.Return(result);
        throw new XEventException(new GenericError(error, mapping.ErrorMessageAction!));
    }

    public ReplyLease<ListHostsReply> ListHosts()
    {
        var cookie = ListHostsBase();
        var (result, mapping) = this._socketAccessor.SocketIn
            .ReceivedResponseSpan<ListHostsReply, ListHostsResponse>(cookie.Id);
        if (mapping.ResponseType == XResponseType.Reply)
            return new ReplyLease<ListHostsReply>(result, this._socketAccessor.SocketIn.BufferPool);

        var error = result.ToStruct<XResponse>();
        this._socketAccessor.SocketIn.BufferPool.Return(result);
        throw new XEventException(new GenericError(error, mapping.ErrorMessageAction!));
    }

    public ReplyLease<AllocNamedColorReply> AllocNamedColor(uint colorMap, ReadOnlySpan<byte> name)
    {
        var cookie = AllocNamedColorBase(colorMap, name);
        var (result, mapping) =
            this._socketAccessor.SocketIn.ReceivedResponseSpan<AllocNamedColorReply, AllocNamedColorReply>(cookie.Id);
        if (mapping.ResponseType == XResponseType.Reply)
            return new ReplyLease<AllocNamedColorReply>(result, this._socketAccessor.SocketIn.BufferPool);

        var error = result.ToStruct<XResponse>();
        this._socketAccessor.SocketIn.BufferPool.Return(result);
        throw new XEventException(new GenericError(error, mapping.ErrorMessageAction!));
    }

    public ReplyLease<GetAtomNameReply> GetAtomName(ATOM atom)
    {
        var cookie = GetAtomNameBase(atom);
        var (result, mapping) =
            this._socketAccessor.SocketIn.ReceivedResponseSpan<GetAtomNameReply, GetAtomNameResponse>(cookie.Id);
        if (mapping.ResponseType == XResponseType.Reply)
            return new ReplyLease<GetAtomNameReply>(result, this._socketAccessor.SocketIn.BufferPool);

        var error = result.ToStruct<XResponse>();
        this._socketAccessor.SocketIn.BufferPool.Return(result);
        throw new XEventException(new GenericError(error, mapping.ErrorMessageAction!));
    }

    public ReplyLease<InternAtomReply> InternAtom(bool onlyIfExist, string atomName)
    {
        var cookie = InternAtomBase(onlyIfExist, atomName);
        var (result, mapping) =
            this._socketAccessor.SocketIn.ReceivedResponseSpan<InternAtomReply, InternAtomReply>(cookie.Id);
        if (mapping.ResponseType == XResponseType.Reply)
            return new ReplyLease<InternAtomReply>(result, this._socketAccessor.SocketIn.BufferPool);

        var error = result.ToStruct<XResponse>();
        this._socketAccessor.SocketIn.BufferPool.Return(result);
        throw new XEventException(new GenericError(error, mapping.ErrorMessageAction!));
    }

    public ReplyLease<GetFontPathReply> GetFontPath()
    {
        var cookie = GetFontPathBase();
        var (result, mapping) =
            this._socketAccessor.SocketIn.ReceivedResponseSpan<GetFontPathReply, GetFontPathResponse>(cookie.Id);
        if (mapping.ResponseType == XResponseType.Reply)
            return new ReplyLease<GetFontPathReply>(result, this._socketAccessor.SocketIn.BufferPool);

        var error = result.ToStruct<XResponse>();
        this._socketAccessor.SocketIn.BufferPool.Return(result);
        throw new XEventException(new GenericError(error, mapping.ErrorMessageAction!));
    }

    public ReplyLease<GetGeometryReply> GetGeometry(uint drawable)
    {
        var cookie = GetGeometryBase(drawable);
        var (result, mapping) =
            this._socketAccessor.SocketIn.ReceivedResponseSpan<GetGeometryReply, GetGeometryReply>(cookie.Id);
        if (mapping.ResponseType == XResponseType.Reply)
            return new ReplyLease<GetGeometryReply>(result, this._socketAccessor.SocketIn.BufferPool);

        var error = result.ToStruct<XResponse>();
        this._socketAccessor.SocketIn.BufferPool.Return(result);
        throw new XEventException(new GenericError(error, mapping.ErrorMessageAction!));
    }

    public ReplyLease<GetImageReply> GetImage(ImageFormat format, uint drawable, ushort x, ushort y, ushort width,
        ushort height, uint planeMask)
    {
        var cookie = GetImageBase(format, drawable, x, y, width, height, planeMask);
        var (result, mapping) =
            this._socketAccessor.SocketIn.ReceivedResponseSpan<GetImageReply, GetImageResponse>(cookie.Id);
        if (mapping.ResponseType == XResponseType.Reply)
            return new ReplyLease<GetImageReply>(result, this._socketAccessor.SocketIn.BufferPool);

        var error = result.ToStruct<XResponse>();
        this._socketAccessor.SocketIn.BufferPool.Return(result);
        throw new XEventException(new GenericError(error, mapping.ErrorMessageAction!));
    }

    public ReplyLease<GetInputFocusReply> GetInputFocus()
    {
        var cookie = GetInputFocusBase();
        var (result, mapping) =
            this._socketAccessor.SocketIn.ReceivedResponseSpan<GetInputFocusReply, GetInputFocusReply>(cookie.Id);
        if (mapping.ResponseType == XResponseType.Reply)
            return new ReplyLease<GetInputFocusReply>(result, this._socketAccessor.SocketIn.BufferPool);

        var error = result.ToStruct<XResponse>();
        this._socketAccessor.SocketIn.BufferPool.Return(result);
        throw new XEventException(new GenericError(error, mapping.ErrorMessageAction!));
    }

    public ReplyLease<GetKeyboardControlReply> GetKeyboardControl()
    {
        var cookie = GetKeyboardControlBase();
        var (result, mapping) = this._socketAccessor.SocketIn
            .ReceivedResponseSpan<GetKeyboardControlReply, GetKeyboardControlResponse>(cookie.Id);
        if (mapping.ResponseType == XResponseType.Reply)
            return new ReplyLease<GetKeyboardControlReply>(result, this._socketAccessor.SocketIn.BufferPool);

        var error = result.ToStruct<XResponse>();
        this._socketAccessor.SocketIn.BufferPool.Return(result);
        throw new XEventException(new GenericError(error, mapping.ErrorMessageAction!));
    }

    public ReplyLease<GetKeyboardMappingReply> GetKeyboardMapping(byte firstKeycode, byte count)
    {
        var cookie = GetKeyboardMappingBase(firstKeycode, count);
        var (result, mapping) = this._socketAccessor.SocketIn
            .ReceivedResponseSpan<GetKeyboardMappingReply, GetKeyboardMappingResponse>(cookie.Id);
        if (mapping.ResponseType == XResponseType.Reply)
            return new ReplyLease<GetKeyboardMappingReply>(result, this._socketAccessor.SocketIn.BufferPool);

        var error = result.ToStruct<XResponse>();
        this._socketAccessor.SocketIn.BufferPool.Return(result);
        throw new XEventException(new GenericError(error, mapping.ErrorMessageAction!));
    }

    public ReplyLease<GetModifierMappingReply> GetModifierMapping()
    {
        var cookie = GetModifierMappingBase();
        var (result, mapping) = this._socketAccessor.SocketIn
            .ReceivedResponseSpan<GetModifierMappingReply, GetModifierMappingResponse>(cookie.Id);
        if (mapping.ResponseType == XResponseType.Reply)
            return new ReplyLease<GetModifierMappingReply>(result, this._socketAccessor.SocketIn.BufferPool);

        var error = result.ToStruct<XResponse>();
        this._socketAccessor.SocketIn.BufferPool.Return(result);
        throw new XEventException(new GenericError(error, mapping.ErrorMessageAction!));
    }

    public ReplyLease<GetMotionEventsReply> GetMotionEvents(uint window, uint startTime, uint endTime)
    {
        var cookie = GetMotionEventsBase(window, startTime, endTime);
        var (result, mapping) = this._socketAccessor.SocketIn
            .ReceivedResponseSpan<GetMotionEventsReply, GetMotionEventsResponse>(cookie.Id);
        if (mapping.ResponseType == XResponseType.Reply)
            return new ReplyLease<GetMotionEventsReply>(result, this._socketAccessor.SocketIn.BufferPool);

        var error = result.ToStruct<XResponse>();
        this._socketAccessor.SocketIn.BufferPool.Return(result);
        throw new XEventException(new GenericError(error, mapping.ErrorMessageAction!));
    }

    public ReplyLease<GetPointerControlReply> GetPointerControl()
    {
        var cookie = GetPointerControlBase();
        var (result, mapping) = this._socketAccessor.SocketIn
            .ReceivedResponseSpan<GetPointerControlReply, GetPointerControlReply>(cookie.Id);
        if (mapping.ResponseType == XResponseType.Reply)
            return new ReplyLease<GetPointerControlReply>(result, this._socketAccessor.SocketIn.BufferPool);

        var error = result.ToStruct<XResponse>();
        this._socketAccessor.SocketIn.BufferPool.Return(result);
        throw new XEventException(new GenericError(error, mapping.ErrorMessageAction!));
    }

    public ReplyLease<GetPointerMappingReply> GetPointerMapping()
    {
        var cookie = GetPointerMappingBase();
        var (result, mapping) = this._socketAccessor.SocketIn
            .ReceivedResponseSpan<GetPointerMappingReply, GetPointerMappingResponse>(cookie.Id);
        if (mapping.ResponseType == XResponseType.Reply)
            return new ReplyLease<GetPointerMappingReply>(result, this._socketAccessor.SocketIn.BufferPool);

        var error = result.ToStruct<XResponse>();
        this._socketAccessor.SocketIn.BufferPool.Return(result);
        throw new XEventException(new GenericError(error, mapping.ErrorMessageAction!));
    }

    public ReplyLease<GetPropertyReply> GetProperty(bool delete, uint window, ATOM property, ATOM type, uint offset,
        uint length)
    {
        var cookie = GetPropertyBase(delete, window, property, type, offset, length);
        var (result, mapping) =
            this._socketAccessor.SocketIn.ReceivedResponseSpan<GetPropertyReply, GetPropertyResponse>(cookie.Id);
        if (mapping.ResponseType == XResponseType.Reply)
            return new ReplyLease<GetPropertyReply>(result, this._socketAccessor.SocketIn.BufferPool);

        var error = result.ToStruct<XResponse>();
        this._socketAccessor.SocketIn.BufferPool.Return(result);
        throw new XEventException(new GenericError(error, mapping.ErrorMessageAction!));
    }

    public ReplyLease<GetScreenSaverReply> GetScreenSaver()
    {
        var cookie = GetScreenSaverBase();
        var (result, mapping) =
            this._socketAccessor.SocketIn.ReceivedResponseSpan<GetScreenSaverReply, GetScreenSaverReply>(cookie.Id);
        if (mapping.ResponseType == XResponseType.Reply)
            return new ReplyLease<GetScreenSaverReply>(result, this._socketAccessor.SocketIn.BufferPool);

        var error = result.ToStruct<XResponse>();
        this._socketAccessor.SocketIn.BufferPool.Return(result);
        throw new XEventException(new GenericError(error, mapping.ErrorMessageAction!));
    }

    public ReplyLease<GetSelectionOwnerReply> GetSelectionOwner(ATOM atom)
    {
        var cookie = GetSelectionOwnerBase(atom);
        var (result, mapping) = this._socketAccessor.SocketIn
            .ReceivedResponseSpan<GetSelectionOwnerReply, GetSelectionOwnerReply>(cookie.Id);
        if (mapping.ResponseType == XResponseType.Reply)
            return new ReplyLease<GetSelectionOwnerReply>(result, this._socketAccessor.SocketIn.BufferPool);

        var error = result.ToStruct<XResponse>();
        this._socketAccessor.SocketIn.BufferPool.Return(result);
        throw new XEventException(new GenericError(error, mapping.ErrorMessageAction!));
    }

    public ReplyLease<GetWindowAttributesReply> GetWindowAttributes(uint window)
    {
        var cookie = GetWindowAttributesBase(window);
        var (result, mapping) = this._socketAccessor.SocketIn
            .ReceivedResponseSpan<GetWindowAttributesReply, GetWindowAttributesReply>(cookie.Id);
        if (mapping.ResponseType == XResponseType.Reply)
            return new ReplyLease<GetWindowAttributesReply>(result, this._socketAccessor.SocketIn.BufferPool);

        var error = result.ToStruct<XResponse>();
        this._socketAccessor.SocketIn.BufferPool.Return(result);
        throw new XEventException(new GenericError(error, mapping.ErrorMessageAction!));
    }

    public ReplyLease<ListFontsReply> ListFonts(ReadOnlySpan<byte> pattern, int maxNames)
    {
        var cookie = ListFontsBase(pattern, maxNames);
        var (result, mapping) =
            this._socketAccessor.SocketIn.ReceivedResponseSpan<ListFontsReply, ListFontsResponse>(cookie.Id);
        if (mapping.ResponseType == XResponseType.Reply)
            return new ReplyLease<ListFontsReply>(result, this._socketAccessor.SocketIn.BufferPool);

        var error = result.ToStruct<XResponse>();
        this._socketAccessor.SocketIn.BufferPool.Return(result);
        throw new XEventException(new GenericError(error, mapping.ErrorMessageAction!));
    }

    public ListFontsWithInfoReply[] ListFontsWithInfo(ReadOnlySpan<byte> pattern, int maxNames)
    {
        var cookie = ListFontsWithInfoBase(pattern, maxNames);
        var (result, error) = this._socketAccessor.ReceivedResponseArray(cookie.Id, maxNames);
        if (error.HasValue)
            throw new Exception();

        return result;
    }

    public ReplyLease<ListInstalledColormapsReply> ListInstalledColormaps(uint window)
    {
        var cookie = ListInstalledColormapsBase(window);
        var (result, mapping) = this._socketAccessor.SocketIn
            .ReceivedResponseSpan<ListInstalledColormapsReply, ListInstalledColormapsResponse>(cookie.Id);
        if (mapping.ResponseType == XResponseType.Reply)
            return new ReplyLease<ListInstalledColormapsReply>(result, this._socketAccessor.SocketIn.BufferPool);

        var error = result.ToStruct<XResponse>();
        this._socketAccessor.SocketIn.BufferPool.Return(result);
        throw new XEventException(new GenericError(error, mapping.ErrorMessageAction!));
    }

    public ReplyLease<ListPropertiesReply> ListProperties(uint window)
    {
        var cookie = ListPropertiesBase(window);
        var (result, mapping) =
            this._socketAccessor.SocketIn.ReceivedResponseSpan<ListPropertiesReply, ListPropertiesResponse>(cookie.Id);
        if (mapping.ResponseType == XResponseType.Reply)
            return new ReplyLease<ListPropertiesReply>(result, this._socketAccessor.SocketIn.BufferPool);

        var error = result.ToStruct<XResponse>();
        this._socketAccessor.SocketIn.BufferPool.Return(result);
        throw new XEventException(new GenericError(error, mapping.ErrorMessageAction!));
    }

    public ReplyLease<LookupColorReply> LookupColor(uint colorMap, ReadOnlySpan<byte> name)
    {
        var cookie = LookupColorBase(colorMap, name);
        var (result, mapping) =
            this._socketAccessor.SocketIn.ReceivedResponseSpan<LookupColorReply, LookupColorReply>(cookie.Id);
        if (mapping.ResponseType == XResponseType.Reply)
            return new ReplyLease<LookupColorReply>(result, this._socketAccessor.SocketIn.BufferPool);

        var error = result.ToStruct<XResponse>();
        this._socketAccessor.SocketIn.BufferPool.Return(result);
        throw new XEventException(new GenericError(error, mapping.ErrorMessageAction!));
    }

    public ReplyLease<QueryBestSizeReply> QueryBestSize(QueryShapeOf shape, uint drawable, ushort width, ushort height)
    {
        var cookie = QueryBestSizeBase(shape, drawable, width, height);
        var (result, mapping) =
            this._socketAccessor.SocketIn.ReceivedResponseSpan<QueryBestSizeReply, QueryBestSizeReply>(cookie.Id);
        if (mapping.ResponseType == XResponseType.Reply)
            return new ReplyLease<QueryBestSizeReply>(result, this._socketAccessor.SocketIn.BufferPool);

        var error = result.ToStruct<XResponse>();
        this._socketAccessor.SocketIn.BufferPool.Return(result);
        throw new XEventException(new GenericError(error, mapping.ErrorMessageAction!));
    }

    public ReplyLease<QueryColorsReply> QueryColors(uint colorMap, ReadOnlySpan<uint> pixels)
    {
        var cookie = QueryColorsBase(colorMap, pixels);
        var (result, mapping) =
            this._socketAccessor.SocketIn.ReceivedResponseSpan<QueryColorsReply, QueryColorsResponse>(cookie.Id);
        if (mapping.ResponseType == XResponseType.Reply)
            return new ReplyLease<QueryColorsReply>(result, this._socketAccessor.SocketIn.BufferPool);

        var error = result.ToStruct<XResponse>();
        this._socketAccessor.SocketIn.BufferPool.Return(result);
        throw new XEventException(new GenericError(error, mapping.ErrorMessageAction!));
    }

    public ReplyLease<QueryFontReply> QueryFont(uint fontId)
    {
        var cookie = QueryFontBase(fontId);
        var (result, mapping) =
            this._socketAccessor.SocketIn.ReceivedResponseSpan<QueryFontReply, QueryFontResponse>(cookie.Id);
        if (mapping.ResponseType == XResponseType.Reply)
            return new ReplyLease<QueryFontReply>(result, this._socketAccessor.SocketIn.BufferPool);

        var error = result.ToStruct<XResponse>();
        this._socketAccessor.SocketIn.BufferPool.Return(result);
        throw new XEventException(new GenericError(error, mapping.ErrorMessageAction!));
    }

    public ReplyLease<QueryKeymapReply> QueryKeymap()
    {
        var cookie = QueryKeymapBase();
        var (result, mapping) =
            this._socketAccessor.SocketIn.ReceivedResponseSpan<QueryKeymapReply, QueryKeymapResponse>(cookie.Id);
        if (mapping.ResponseType == XResponseType.Reply)
            return new ReplyLease<QueryKeymapReply>(result, this._socketAccessor.SocketIn.BufferPool);

        var error = result.ToStruct<XResponse>();
        this._socketAccessor.SocketIn.BufferPool.Return(result);
        throw new XEventException(new GenericError(error, mapping.ErrorMessageAction!));
    }

    public ReplyLease<QueryPointerReply> QueryPointer(uint window)
    {
        var cookie = QueryPointerBase(window);
        var (result, mapping) =
            this._socketAccessor.SocketIn.ReceivedResponseSpan<QueryPointerReply, QueryPointerReply>(cookie.Id);
        if (mapping.ResponseType == XResponseType.Reply)
            return new ReplyLease<QueryPointerReply>(result, this._socketAccessor.SocketIn.BufferPool);

        var error = result.ToStruct<XResponse>();
        this._socketAccessor.SocketIn.BufferPool.Return(result);
        throw new XEventException(new GenericError(error, mapping.ErrorMessageAction!));
    }

    public ReplyLease<QueryTextExtentsReply> QueryTextExtents(uint font, ReadOnlySpan<char> stringForQuery)
    {
        var cookie = QueryTextExtentsBase(font, stringForQuery);
        var (result, mapping) =
            this._socketAccessor.SocketIn.ReceivedResponseSpan<QueryTextExtentsReply, QueryTextExtentsReply>(cookie.Id);
        if (mapping.ResponseType == XResponseType.Reply)
            return new ReplyLease<QueryTextExtentsReply>(result, this._socketAccessor.SocketIn.BufferPool);

        var error = result.ToStruct<XResponse>();
        this._socketAccessor.SocketIn.BufferPool.Return(result);
        throw new XEventException(new GenericError(error, mapping.ErrorMessageAction!));
    }

    public ReplyLease<QueryTreeReply> QueryTree(uint window)
    {
        var cookie = QueryTreeBase(window);
        var (result, mapping) =
            this._socketAccessor.SocketIn.ReceivedResponseSpan<QueryTreeReply, QueryTreeResponse>(cookie.Id);
        if (mapping.ResponseType == XResponseType.Reply)
            return new ReplyLease<QueryTreeReply>(result, this._socketAccessor.SocketIn.BufferPool);

        var error = result.ToStruct<XResponse>();
        this._socketAccessor.SocketIn.BufferPool.Return(result);
        throw new XEventException(new GenericError(error, mapping.ErrorMessageAction!));
    }

    public ReplyLease<GrabKeyboardReply> GrabKeyboard(bool ownerEvents, uint grabWindow, uint timeStamp,
        GrabMode pointerMode,
        GrabMode keyboardMode)
    {
        var cookie = GrabKeyboardBase(ownerEvents, grabWindow, timeStamp, pointerMode, keyboardMode);
        var (result, mapping) =
            this._socketAccessor.SocketIn.ReceivedResponseSpan<GrabKeyboardReply, GrabKeyboardReply>(cookie.Id);
        if (mapping.ResponseType == XResponseType.Reply)
            return new ReplyLease<GrabKeyboardReply>(result, this._socketAccessor.SocketIn.BufferPool);

        var error = result.ToStruct<XResponse>();
        this._socketAccessor.SocketIn.BufferPool.Return(result);
        throw new XEventException(new GenericError(error, mapping.ErrorMessageAction!));
    }

    public ReplyLease<GrabPointerReply> GrabPointer(bool ownerEvents, uint grabWindow, ushort mask,
        GrabMode pointerMode,
        GrabMode keyboardMode, uint confineTo, uint cursor, uint timeStamp)
    {
        var cookie = GrabPointerBase(ownerEvents, grabWindow, mask, pointerMode, keyboardMode, confineTo, cursor,
            timeStamp);
        var (result, mapping) =
            this._socketAccessor.SocketIn.ReceivedResponseSpan<GrabPointerReply, GrabPointerReply>(cookie.Id);
        if (mapping.ResponseType == XResponseType.Reply)
            return new ReplyLease<GrabPointerReply>(result, this._socketAccessor.SocketIn.BufferPool);

        var error = result.ToStruct<XResponse>();
        this._socketAccessor.SocketIn.BufferPool.Return(result);
        throw new XEventException(new GenericError(error, mapping.ErrorMessageAction!));
    }

    public ReplyLease<SetModifierMappingReply> SetModifierMapping(ReadOnlySpan<ulong> keycodes)
    {
        var cookie = SetModifierMappingBase(keycodes);
        var (result, mapping) = this._socketAccessor.SocketIn
            .ReceivedResponseSpan<SetModifierMappingReply, SetModifierMappingReply>(cookie.Id);
        if (mapping.ResponseType == XResponseType.Reply)
            return new ReplyLease<SetModifierMappingReply>(result, this._socketAccessor.SocketIn.BufferPool);

        var error = result.ToStruct<XResponse>();
        this._socketAccessor.SocketIn.BufferPool.Return(result);
        throw new XEventException(new GenericError(error, mapping.ErrorMessageAction!));
    }

    public ReplyLease<SetPointerMappingReply> SetPointerMapping(ReadOnlySpan<byte> maps)
    {
        var cookie = SetPointerMappingBase(maps);
        var (result, mapping) = this._socketAccessor.SocketIn
            .ReceivedResponseSpan<SetPointerMappingReply, SetPointerMappingReply>(cookie.Id);
        if (mapping.ResponseType == XResponseType.Reply)
            return new ReplyLease<SetPointerMappingReply>(result, this._socketAccessor.SocketIn.BufferPool);

        var error = result.ToStruct<XResponse>();
        this._socketAccessor.SocketIn.BufferPool.Return(result);
        throw new XEventException(new GenericError(error, mapping.ErrorMessageAction!));
    }

    public ReplyLease<TranslateCoordinatesReply> TranslateCoordinates(uint srcWindow, uint destinationWindow,
        ushort srcX,
        ushort srcY)
    {
        var cookie = TranslateCoordinatesBase(srcWindow, destinationWindow, srcX, srcY);
        var (result, mapping) = this._socketAccessor.SocketIn
            .ReceivedResponseSpan<TranslateCoordinatesReply, TranslateCoordinatesReply>(cookie.Id);
        if (mapping.ResponseType == XResponseType.Reply)
            return new ReplyLease<TranslateCoordinatesReply>(result, this._socketAccessor.SocketIn.BufferPool);

        var error = result.ToStruct<XResponse>();
        this._socketAccessor.SocketIn.BufferPool.Return(result);
        throw new XEventException(new GenericError(error, mapping.ErrorMessageAction!));
    }


    public async ValueTask<ReplyLease<AllocColorReply>> AllocColorAsync(uint colorMap, ushort red, ushort green,
        ushort blue,
        CancellationToken token = default)
    {
        var cookie = AllocColorBase(colorMap, red, green, blue);
        var (result, mapping) = await this._socketAccessor.SocketIn
            .ReceivedResponseSpanAsync<AllocColorReply, AllocColorReply>(cookie.Id, token).ConfigureAwait(false);
        if (mapping.ResponseType == XResponseType.Reply)
            return new ReplyLease<AllocColorReply>(result, this._socketAccessor.SocketIn.BufferPool);

        var error = result.ToStruct<XResponse>();
        this._socketAccessor.SocketIn.BufferPool.Return(result);
        throw new XEventException(new GenericError(error, mapping.ErrorMessageAction!));
    }

    public async ValueTask<ReplyLease<QueryPointerReply>> QueryPointerAsync(uint window,
        CancellationToken token = default)
    {
        var cookie = QueryPointerBase(window);
        var (result, mapping) = await this._socketAccessor.SocketIn
            .ReceivedResponseSpanAsync<QueryPointerReply, QueryPointerReply>(cookie.Id, token)
            .ConfigureAwait(false);

        if (mapping.ResponseType == XResponseType.Reply)
            return new ReplyLease<QueryPointerReply>(result, this._socketAccessor.SocketIn.BufferPool);

        var error = result.ToStruct<XResponse>();
        this._socketAccessor.SocketIn.BufferPool.Return(result);
        throw new XEventException(new GenericError(error, mapping.ErrorMessageAction!));
    }

    public async ValueTask<ReplyLease<GrabPointerReply>> GrabPointerAsync(bool ownerEvents, uint grabWindow,
        ushort mask,
        GrabMode pointerMode, GrabMode keyboardMode, uint confineTo, uint cursor, uint timeStamp,
        CancellationToken token = default)
    {
        var cookie = GrabPointerBase(ownerEvents, grabWindow, mask, pointerMode, keyboardMode, confineTo, cursor,
            timeStamp);
        var (result, mapping) = await this._socketAccessor.SocketIn
            .ReceivedResponseSpanAsync<GrabPointerReply, GrabPointerReply>(cookie.Id, token).ConfigureAwait(false);
        if (mapping.ResponseType == XResponseType.Reply)
            return new ReplyLease<GrabPointerReply>(result, this._socketAccessor.SocketIn.BufferPool);

        var error = result.ToStruct<XResponse>();
        this._socketAccessor.SocketIn.BufferPool.Return(result);
        throw new XEventException(new GenericError(error, mapping.ErrorMessageAction!));
    }

    public async ValueTask<ReplyLease<InternAtomReply>> InternAtomAsync(bool onlyIfExist, string atomName,
        CancellationToken token = default)
    {
        var cookie = InternAtomBase(onlyIfExist, atomName);
        var (result, mapping) = await this._socketAccessor.SocketIn
            .ReceivedResponseSpanAsync<InternAtomReply, InternAtomReply>(cookie.Id, token)
            .ConfigureAwait(false);

        if (mapping.ResponseType == XResponseType.Reply)
            return new ReplyLease<InternAtomReply>(result, this._socketAccessor.SocketIn.BufferPool);

        var error = result.ToStruct<XResponse>();
        this._socketAccessor.SocketIn.BufferPool.Return(result);
        throw new XEventException(new GenericError(error, mapping.ErrorMessageAction!));
    }

    public async ValueTask<ReplyLease<GetPropertyReply>> GetPropertyAsync(bool delete, uint window, ATOM property,
        ATOM type, uint offset, uint length, CancellationToken token = default)
    {
        var cookie = GetPropertyBase(delete, window, property, type, offset, length);
        var (result, mapping) = await this._socketAccessor.SocketIn
            .ReceivedResponseSpanAsync<GetPropertyReply, GetPropertyResponse>(cookie.Id, token)
            .ConfigureAwait(false);
        if (mapping.ResponseType == XResponseType.Reply)
            return new ReplyLease<GetPropertyReply>(result, this._socketAccessor.SocketIn.BufferPool);

        var error = result.ToStruct<XResponse>();
        this._socketAccessor.SocketIn.BufferPool.Return(result);
        throw new XEventException(new GenericError(error, mapping.ErrorMessageAction!));
    }

    public async ValueTask<ReplyLease<GetWindowAttributesReply>> GetWindowAttributesAsync(uint window,
        CancellationToken token = default)
    {
        var cookie = GetWindowAttributesBase(window);
        var (result, mapping) = await this._socketAccessor.SocketIn
            .ReceivedResponseSpanAsync<GetWindowAttributesReply, GetWindowAttributesReply>(cookie.Id, token)
            .ConfigureAwait(false);
        if (mapping.ResponseType == XResponseType.Reply)
            return new ReplyLease<GetWindowAttributesReply>(result, this._socketAccessor.SocketIn.BufferPool);

        var error = result.ToStruct<XResponse>();
        this._socketAccessor.SocketIn.BufferPool.Return(result);
        throw new XEventException(new GenericError(error, mapping.ErrorMessageAction!));
    }

    public async ValueTask<ReplyLease<GetGeometryReply>> GetGeometryAsync(uint drawable,
        CancellationToken token = default)
    {
        var cookie = GetGeometryBase(drawable);
        var (result, mapping) = await this._socketAccessor.SocketIn
            .ReceivedResponseSpanAsync<GetGeometryReply, GetGeometryReply>(cookie.Id, token)
            .ConfigureAwait(false);
        if (mapping.ResponseType == XResponseType.Reply)
            return new ReplyLease<GetGeometryReply>(result, this._socketAccessor.SocketIn.BufferPool);

        var error = result.ToStruct<XResponse>();
        this._socketAccessor.SocketIn.BufferPool.Return(result);
        throw new XEventException(new GenericError(error, mapping.ErrorMessageAction!));
    }

    public async ValueTask<ReplyLease<QueryTreeReply>> QueryTreeAsync(uint window, CancellationToken token = default)
    {
        var cookie = QueryTreeBase(window);
        var (result, mapping) = await this._socketAccessor.SocketIn
            .ReceivedResponseSpanAsync<QueryTreeReply, QueryTreeResponse>(cookie.Id, token)
            .ConfigureAwait(false);
        if (mapping.ResponseType == XResponseType.Reply)
            return new ReplyLease<QueryTreeReply>(result, this._socketAccessor.SocketIn.BufferPool);

        var error = result.ToStruct<XResponse>();
        this._socketAccessor.SocketIn.BufferPool.Return(result);
        throw new XEventException(new GenericError(error, mapping.ErrorMessageAction!));
    }

    public async ValueTask<ReplyLease<GetAtomNameReply>> GetAtomNameAsync(ATOM atom, CancellationToken token = default)
    {
        var cookie = GetAtomNameBase(atom);
        var (result, mapping) = await this._socketAccessor.SocketIn
            .ReceivedResponseSpanAsync<GetAtomNameReply, GetAtomNameResponse>(cookie.Id, token)
            .ConfigureAwait(false);
        if (mapping.ResponseType == XResponseType.Reply)
            return new ReplyLease<GetAtomNameReply>(result, this._socketAccessor.SocketIn.BufferPool);

        var error = result.ToStruct<XResponse>();
        this._socketAccessor.SocketIn.BufferPool.Return(result);
        throw new XEventException(new GenericError(error, mapping.ErrorMessageAction!));
    }

    public async ValueTask<ReplyLease<ListPropertiesReply>> ListPropertiesAsync(uint window,
        CancellationToken token = default)
    {
        var cookie = ListPropertiesBase(window);
        var (result, mapping) = await this._socketAccessor.SocketIn
            .ReceivedResponseSpanAsync<ListPropertiesReply, ListPropertiesResponse>(cookie.Id, token)
            .ConfigureAwait(false);
        if (mapping.ResponseType == XResponseType.Reply)
            return new ReplyLease<ListPropertiesReply>(result, this._socketAccessor.SocketIn.BufferPool);

        var error = result.ToStruct<XResponse>();
        this._socketAccessor.SocketIn.BufferPool.Return(result);
        throw new XEventException(new GenericError(error, mapping.ErrorMessageAction!));
    }

    public async ValueTask<ReplyLease<GetSelectionOwnerReply>> GetSelectionOwnerAsync(ATOM atom,
        CancellationToken token = default)
    {
        var cookie = GetSelectionOwnerBase(atom);
        var (result, mapping) = await this._socketAccessor.SocketIn
            .ReceivedResponseSpanAsync<GetSelectionOwnerReply, GetSelectionOwnerReply>(cookie.Id, token)
            .ConfigureAwait(false);
        if (mapping.ResponseType == XResponseType.Reply)
            return new ReplyLease<GetSelectionOwnerReply>(result, this._socketAccessor.SocketIn.BufferPool);

        var error = result.ToStruct<XResponse>();
        this._socketAccessor.SocketIn.BufferPool.Return(result);
        throw new XEventException(new GenericError(error, mapping.ErrorMessageAction!));
    }

    public async ValueTask<ReplyLease<GrabKeyboardReply>> GrabKeyboardAsync(bool ownerEvents, uint grabWindow,
        uint timeStamp,
        GrabMode pointerMode, GrabMode keyboardMode, CancellationToken token = default)
    {
        var cookie = GrabKeyboardBase(ownerEvents, grabWindow, timeStamp, pointerMode, keyboardMode);
        var (result, mapping) = await this._socketAccessor.SocketIn
            .ReceivedResponseSpanAsync<GrabKeyboardReply, GrabKeyboardReply>(cookie.Id, token)
            .ConfigureAwait(false);
        if (mapping.ResponseType == XResponseType.Reply)
            return new ReplyLease<GrabKeyboardReply>(result, this._socketAccessor.SocketIn.BufferPool);

        var error = result.ToStruct<XResponse>();
        this._socketAccessor.SocketIn.BufferPool.Return(result);
        throw new XEventException(new GenericError(error, mapping.ErrorMessageAction!));
    }

    public async ValueTask<ReplyLease<GetMotionEventsReply>> GetMotionEventsAsync(uint window, uint startTime,
        uint endTime,
        CancellationToken token = default)
    {
        var cookie = GetMotionEventsBase(window, startTime, endTime);
        var (result, mapping) = await this._socketAccessor.SocketIn
            .ReceivedResponseSpanAsync<GetMotionEventsReply, GetMotionEventsResponse>(cookie.Id, token)
            .ConfigureAwait(false);
        if (mapping.ResponseType == XResponseType.Reply)
            return new ReplyLease<GetMotionEventsReply>(result, this._socketAccessor.SocketIn.BufferPool);

        var error = result.ToStruct<XResponse>();
        this._socketAccessor.SocketIn.BufferPool.Return(result);
        throw new XEventException(new GenericError(error, mapping.ErrorMessageAction!));
    }

    public async ValueTask<ReplyLease<TranslateCoordinatesReply>> TranslateCoordinatesAsync(uint srcWindow,
        uint destinationWindow,
        ushort srcX, ushort srcY, CancellationToken token = default)
    {
        var cookie = TranslateCoordinatesBase(srcWindow, destinationWindow, srcX, srcY);
        var (result, mapping) = await this._socketAccessor.SocketIn
            .ReceivedResponseSpanAsync<TranslateCoordinatesReply, TranslateCoordinatesReply>(cookie.Id, token)
            .ConfigureAwait(false);
        if (mapping.ResponseType == XResponseType.Reply)
            return new ReplyLease<TranslateCoordinatesReply>(result, this._socketAccessor.SocketIn.BufferPool);

        var error = result.ToStruct<XResponse>();
        this._socketAccessor.SocketIn.BufferPool.Return(result);
        throw new XEventException(new GenericError(error, mapping.ErrorMessageAction!));
    }

    public async ValueTask<ReplyLease<GetInputFocusReply>> GetInputFocusAsync(CancellationToken token = default)
    {
        var cookie = GetInputFocusBase();
        var (result, mapping) = await this._socketAccessor.SocketIn
            .ReceivedResponseSpanAsync<GetInputFocusReply, GetInputFocusReply>(cookie.Id, token)
            .ConfigureAwait(false);
        if (mapping.ResponseType == XResponseType.Reply)
            return new ReplyLease<GetInputFocusReply>(result, this._socketAccessor.SocketIn.BufferPool);

        var error = result.ToStruct<XResponse>();
        this._socketAccessor.SocketIn.BufferPool.Return(result);
        throw new XEventException(new GenericError(error, mapping.ErrorMessageAction!));
    }

    public async ValueTask<ReplyLease<QueryKeymapReply>> QueryKeymapAsync(CancellationToken token = default)
    {
        var cookie = QueryKeymapBase();
        var (result, mapping) = await this._socketAccessor.SocketIn
            .ReceivedResponseSpanAsync<QueryKeymapReply, QueryKeymapResponse>(cookie.Id, token)
            .ConfigureAwait(false);
        if (mapping.ResponseType == XResponseType.Reply)
            return new ReplyLease<QueryKeymapReply>(result, this._socketAccessor.SocketIn.BufferPool);

        var error = result.ToStruct<XResponse>();
        this._socketAccessor.SocketIn.BufferPool.Return(result);
        throw new XEventException(new GenericError(error, mapping.ErrorMessageAction!));
    }

    public async ValueTask<ReplyLease<QueryFontReply>> QueryFontAsync(uint fontId, CancellationToken token = default)
    {
        var cookie = QueryFontBase(fontId);
        var (result, mapping) = await this._socketAccessor.SocketIn
            .ReceivedResponseSpanAsync<QueryFontReply, QueryFontResponse>(cookie.Id, token)
            .ConfigureAwait(false);
        if (mapping.ResponseType == XResponseType.Reply)
            return new ReplyLease<QueryFontReply>(result, this._socketAccessor.SocketIn.BufferPool);

        var error = result.ToStruct<XResponse>();
        this._socketAccessor.SocketIn.BufferPool.Return(result);
        throw new XEventException(new GenericError(error, mapping.ErrorMessageAction!));
    }

    public async ValueTask<ReplyLease<QueryTextExtentsReply>> QueryTextExtentsAsync(uint font, string stringForQuery,
        CancellationToken token = default)
    {
        var cookie = QueryTextExtentsBase(font, stringForQuery);
        var (result, mapping) = await this._socketAccessor.SocketIn
            .ReceivedResponseSpanAsync<QueryTextExtentsReply, QueryTextExtentsReply>(cookie.Id, token)
            .ConfigureAwait(false);
        if (mapping.ResponseType == XResponseType.Reply)
            return new ReplyLease<QueryTextExtentsReply>(result, this._socketAccessor.SocketIn.BufferPool);

        var error = result.ToStruct<XResponse>();
        this._socketAccessor.SocketIn.BufferPool.Return(result);
        throw new XEventException(new GenericError(error, mapping.ErrorMessageAction!));
    }

    public async ValueTask<ReplyLease<ListFontsReply>> ListFontsAsync(ReadOnlyMemory<byte> pattern, int maxNames,
        CancellationToken token = default)
    {
        var cookie = ListFontsBase(pattern.Span, maxNames);
        var (result, mapping) = await this._socketAccessor.SocketIn
            .ReceivedResponseSpanAsync<ListFontsReply, ListFontsResponse>(cookie.Id, token)
            .ConfigureAwait(false);
        if (mapping.ResponseType == XResponseType.Reply)
            return new ReplyLease<ListFontsReply>(result, this._socketAccessor.SocketIn.BufferPool);

        var error = result.ToStruct<XResponse>();
        this._socketAccessor.SocketIn.BufferPool.Return(result);
        throw new XEventException(new GenericError(error, mapping.ErrorMessageAction!));
    }

    // public async ValueTask<ReplyLease<ListFontsWithInfoReply>[]> ListFontsWithInfoAsync(ReadOnlyMemory<byte> pattan, int maxNames,
    //     CancellationToken token = default)
    // {
    //     var cookie = ListFontsWithInfoBase(pattan.Span, maxNames);
    //     var (result, mapping) = await this._socketAccessor.ReceivedResponseArrayAsync(cookie.Id, maxNames, token).ConfigureAwait(false);
    //     return error.HasValue
    //         ? throw new XEventException(error.Value)
    //         : result;
    // }

    public async ValueTask<ReplyLease<GetFontPathReply>> GetFontPathAsync(CancellationToken token = default)
    {
        var cookie = GetFontPathBase();
        var (result, mapping) = await this._socketAccessor.SocketIn
            .ReceivedResponseSpanAsync<GetFontPathReply, GetFontPathResponse>(cookie.Id, token)
            .ConfigureAwait(false);
        if (mapping.ResponseType == XResponseType.Reply)
            return new ReplyLease<GetFontPathReply>(result, this._socketAccessor.SocketIn.BufferPool);

        var error = result.ToStruct<XResponse>();
        this._socketAccessor.SocketIn.BufferPool.Return(result);
        throw new XEventException(new GenericError(error, mapping.ErrorMessageAction!));
    }

    public async ValueTask<ReplyLease<GetImageReply>> GetImageAsync(ImageFormat format, uint drawable, ushort x,
        ushort y, ushort width,
        ushort height, uint planeMask, CancellationToken token = default)
    {
        var cookie = GetImageBase(format, drawable, x, y, width, height, planeMask);
        var (result, mapping) = await this._socketAccessor.SocketIn
            .ReceivedResponseSpanAsync<GetImageReply, GetImageResponse>(cookie.Id, token)
            .ConfigureAwait(false);
        if (mapping.ResponseType == XResponseType.Reply)
            return new ReplyLease<GetImageReply>(result, this._socketAccessor.SocketIn.BufferPool);

        var error = result.ToStruct<XResponse>();
        this._socketAccessor.SocketIn.BufferPool.Return(result);
        throw new XEventException(new GenericError(error, mapping.ErrorMessageAction!));
    }

    public async ValueTask<ReplyLease<ListInstalledColormapsReply>> ListInstalledColormapsAsync(uint window,
        CancellationToken token = default)
    {
        var cookie = ListInstalledColormapsBase(window);
        var (result, mapping) = await
            this._socketAccessor.SocketIn
                .ReceivedResponseSpanAsync<ListInstalledColormapsReply, ListInstalledColormapsResponse>(cookie.Id,
                    token)
                .ConfigureAwait(false);
        if (mapping.ResponseType == XResponseType.Reply)
            return new ReplyLease<ListInstalledColormapsReply>(result, this._socketAccessor.SocketIn.BufferPool);

        var error = result.ToStruct<XResponse>();
        this._socketAccessor.SocketIn.BufferPool.Return(result);
        throw new XEventException(new GenericError(error, mapping.ErrorMessageAction!));
    }

    public async ValueTask<ReplyLease<AllocNamedColorReply>> AllocNamedColorAsync(uint colorMap,
        ReadOnlyMemory<byte> name,
        CancellationToken token = default)
    {
        var cookie = AllocNamedColorBase(colorMap, name.Span);
        var (result, mapping) = await this._socketAccessor.SocketIn
            .ReceivedResponseSpanAsync<AllocNamedColorReply, AllocNamedColorReply>(cookie.Id, token)
            .ConfigureAwait(false);
        if (mapping.ResponseType == XResponseType.Reply)
            return new ReplyLease<AllocNamedColorReply>(result, this._socketAccessor.SocketIn.BufferPool);

        var error = result.ToStruct<XResponse>();
        this._socketAccessor.SocketIn.BufferPool.Return(result);
        throw new XEventException(new GenericError(error, mapping.ErrorMessageAction!));
    }

    public async ValueTask<ReplyLease<AllocColorCellsReply>> AllocColorCellsAsync(bool contiguous, uint colorMap,
        ushort colors,
        ushort planes,
        CancellationToken token = default)
    {
        var cookie = AllocColorCellsBase(contiguous, colorMap, colors, planes);
        var (result, mapping) = await this._socketAccessor.SocketIn
            .ReceivedResponseSpanAsync<AllocColorCellsReply, AllocColorCellsResponse>(cookie.Id, token)
            .ConfigureAwait(false);
        if (mapping.ResponseType == XResponseType.Reply)
            return new ReplyLease<AllocColorCellsReply>(result, this._socketAccessor.SocketIn.BufferPool);

        var error = result.ToStruct<XResponse>();
        this._socketAccessor.SocketIn.BufferPool.Return(result);
        throw new XEventException(new GenericError(error, mapping.ErrorMessageAction!));
    }

    public async ValueTask<ReplyLease<AllocColorPlanesReply>> AllocColorPlanesAsync(bool contiguous, uint colorMap,
        ushort colors,
        ushort reds,
        ushort greens, ushort blues,
        CancellationToken token = default)
    {
        var cookie = AllocColorPlanesBase(contiguous, colorMap, colors, reds, greens, blues);
        var (result, mapping) = await this._socketAccessor.SocketIn
            .ReceivedResponseSpanAsync<AllocColorPlanesReply, AllocColorPlanesResponse>(cookie.Id, token)
            .ConfigureAwait(false);
        if (mapping.ResponseType == XResponseType.Reply)
            return new ReplyLease<AllocColorPlanesReply>(result, this._socketAccessor.SocketIn.BufferPool);

        var error = result.ToStruct<XResponse>();
        this._socketAccessor.SocketIn.BufferPool.Return(result);
        throw new XEventException(new GenericError(error, mapping.ErrorMessageAction!));
    }

    public async ValueTask<ReplyLease<QueryColorsReply>> QueryColorsAsync(uint colorMap, ReadOnlyMemory<uint> pixels,
        CancellationToken token = default)
    {
        var cookie = QueryColorsBase(colorMap, pixels.Span);
        var (result, mapping) = await this._socketAccessor.SocketIn
            .ReceivedResponseSpanAsync<QueryColorsReply, QueryColorsResponse>(cookie.Id, token)
            .ConfigureAwait(false);
        if (mapping.ResponseType == XResponseType.Reply)
            return new ReplyLease<QueryColorsReply>(result, this._socketAccessor.SocketIn.BufferPool);

        var error = result.ToStruct<XResponse>();
        this._socketAccessor.SocketIn.BufferPool.Return(result);
        throw new XEventException(new GenericError(error, mapping.ErrorMessageAction!));
    }

    public async ValueTask<ReplyLease<LookupColorReply>> LookupColorAsync(uint colorMap, ReadOnlyMemory<byte> name,
        CancellationToken token = default)
    {
        var cookie = LookupColorBase(colorMap, name.Span);
        var (result, mapping) = await this._socketAccessor.SocketIn
            .ReceivedResponseSpanAsync<LookupColorReply, LookupColorReply>(cookie.Id, token)
            .ConfigureAwait(false);
        if (mapping.ResponseType == XResponseType.Reply)
            return new ReplyLease<LookupColorReply>(result, this._socketAccessor.SocketIn.BufferPool);

        var error = result.ToStruct<XResponse>();
        this._socketAccessor.SocketIn.BufferPool.Return(result);
        throw new XEventException(new GenericError(error, mapping.ErrorMessageAction!));
    }

    public async ValueTask<ReplyLease<QueryBestSizeReply>> QueryBestSizeAsync(QueryShapeOf shape, uint drawable,
        ushort width,
        ushort height,
        CancellationToken token = default)
    {
        var cookie = QueryBestSizeBase(shape, drawable, width, height);
        var (result, mapping) = await this._socketAccessor.SocketIn
            .ReceivedResponseSpanAsync<QueryBestSizeReply, QueryBestSizeReply>(cookie.Id, token)
            .ConfigureAwait(false);
        if (mapping.ResponseType == XResponseType.Reply)
            return new ReplyLease<QueryBestSizeReply>(result, this._socketAccessor.SocketIn.BufferPool);

        var error = result.ToStruct<XResponse>();
        this._socketAccessor.SocketIn.BufferPool.Return(result);
        throw new XEventException(new GenericError(error, mapping.ErrorMessageAction!));
    }

    public async ValueTask<ReplyLease<SetModifierMappingReply>> SetModifierMappingAsync(ReadOnlyMemory<ulong> keycodes,
        CancellationToken token = default)
    {
        var cookie = SetModifierMappingBase(keycodes.Span);
        var (result, mapping) = await this._socketAccessor.SocketIn
            .ReceivedResponseSpanAsync<SetModifierMappingReply, SetModifierMappingReply>(cookie.Id, token)
            .ConfigureAwait(false);
        if (mapping.ResponseType == XResponseType.Reply)
            return new ReplyLease<SetModifierMappingReply>(result, this._socketAccessor.SocketIn.BufferPool);

        var error = result.ToStruct<XResponse>();
        this._socketAccessor.SocketIn.BufferPool.Return(result);
        throw new XEventException(new GenericError(error, mapping.ErrorMessageAction!));
    }

    public async ValueTask<ReplyLease<GetModifierMappingReply>> GetModifierMappingAsync(
        CancellationToken token = default)
    {
        var cookie = GetModifierMappingBase();
        var (result, mapping) = await this._socketAccessor.SocketIn
            .ReceivedResponseSpanAsync<GetModifierMappingReply, GetModifierMappingResponse>(cookie.Id, token)
            .ConfigureAwait(false);
        if (mapping.ResponseType == XResponseType.Reply)
            return new ReplyLease<GetModifierMappingReply>(result, this._socketAccessor.SocketIn.BufferPool);

        var error = result.ToStruct<XResponse>();
        this._socketAccessor.SocketIn.BufferPool.Return(result);
        throw new XEventException(new GenericError(error, mapping.ErrorMessageAction!));
    }

    public async ValueTask<ReplyLease<GetKeyboardMappingReply>> GetKeyboardMappingAsync(byte firstKeycode, byte count,
        CancellationToken token = default)
    {
        var cookie = GetKeyboardMappingBase(firstKeycode, count);
        var (result, mapping) = await this._socketAccessor.SocketIn
            .ReceivedResponseSpanAsync<GetKeyboardMappingReply, GetKeyboardMappingResponse>(cookie.Id, token)
            .ConfigureAwait(false);
        if (mapping.ResponseType == XResponseType.Reply)
            return new ReplyLease<GetKeyboardMappingReply>(result, this._socketAccessor.SocketIn.BufferPool);

        var error = result.ToStruct<XResponse>();
        this._socketAccessor.SocketIn.BufferPool.Return(result);
        throw new XEventException(new GenericError(error, mapping.ErrorMessageAction!));
    }

    public async ValueTask<ReplyLease<GetKeyboardControlReply>> GetKeyboardControlAsync(
        CancellationToken token = default)
    {
        var cookie = GetKeyboardControlBase();
        var (result, mapping) = await this._socketAccessor.SocketIn
            .ReceivedResponseSpanAsync<GetKeyboardControlReply, GetKeyboardControlResponse>(cookie.Id, token)
            .ConfigureAwait(false);
        if (mapping.ResponseType == XResponseType.Reply)
            return new ReplyLease<GetKeyboardControlReply>(result, this._socketAccessor.SocketIn.BufferPool);

        var error = result.ToStruct<XResponse>();
        this._socketAccessor.SocketIn.BufferPool.Return(result);
        throw new XEventException(new GenericError(error, mapping.ErrorMessageAction!));
    }

    public async ValueTask<ReplyLease<SetPointerMappingReply>> SetPointerMappingAsync(ReadOnlyMemory<byte> maps,
        CancellationToken token = default)
    {
        var cookie = SetPointerMappingBase(maps.Span);
        var (result, mapping) = await this._socketAccessor.SocketIn
            .ReceivedResponseSpanAsync<SetPointerMappingReply, SetPointerMappingReply>(cookie.Id, token)
            .ConfigureAwait(false);
        if (mapping.ResponseType == XResponseType.Reply)
            return new ReplyLease<SetPointerMappingReply>(result, this._socketAccessor.SocketIn.BufferPool);

        var error = result.ToStruct<XResponse>();
        this._socketAccessor.SocketIn.BufferPool.Return(result);
        throw new XEventException(new GenericError(error, mapping.ErrorMessageAction!));
    }

    public async ValueTask<ReplyLease<GetPointerMappingReply>> GetPointerMappingAsync(CancellationToken token = default)
    {
        var cookie = GetPointerMappingBase();
        var (result, mapping) = await this._socketAccessor.SocketIn
            .ReceivedResponseSpanAsync<GetPointerMappingReply, GetPointerMappingResponse>(cookie.Id, token)
            .ConfigureAwait(false);
        if (mapping.ResponseType == XResponseType.Reply)
            return new ReplyLease<GetPointerMappingReply>(result, this._socketAccessor.SocketIn.BufferPool);

        var error = result.ToStruct<XResponse>();
        this._socketAccessor.SocketIn.BufferPool.Return(result);
        throw new XEventException(new GenericError(error, mapping.ErrorMessageAction!));
    }

    public async ValueTask<ReplyLease<GetPointerControlReply>> GetPointerControlAsync(CancellationToken token = default)
    {
        var cookie = GetPointerControlBase();
        var (result, mapping) = await this._socketAccessor.SocketIn
            .ReceivedResponseSpanAsync<GetPointerControlReply, GetPointerControlReply>(cookie.Id, token)
            .ConfigureAwait(false);
        if (mapping.ResponseType == XResponseType.Reply)
            return new ReplyLease<GetPointerControlReply>(result, this._socketAccessor.SocketIn.BufferPool);

        var error = result.ToStruct<XResponse>();
        this._socketAccessor.SocketIn.BufferPool.Return(result);
        throw new XEventException(new GenericError(error, mapping.ErrorMessageAction!));
    }

    public async ValueTask<ReplyLease<GetScreenSaverReply>> GetScreenSaverAsync(CancellationToken token = default)
    {
        var cookie = GetScreenSaverBase();
        var (result, mapping) = await this._socketAccessor.SocketIn
            .ReceivedResponseSpanAsync<GetScreenSaverReply, GetScreenSaverReply>(cookie.Id, token)
            .ConfigureAwait(false);
        if (mapping.ResponseType == XResponseType.Reply)
            return new ReplyLease<GetScreenSaverReply>(result, this._socketAccessor.SocketIn.BufferPool);

        var error = result.ToStruct<XResponse>();
        this._socketAccessor.SocketIn.BufferPool.Return(result);
        throw new XEventException(new GenericError(error, mapping.ErrorMessageAction!));
    }

    public async ValueTask<ReplyLease<ListHostsReply>> ListHostsAsync(CancellationToken token = default)
    {
        var cookie = ListHostsBase();
        var (result, mapping) = await this._socketAccessor.SocketIn
            .ReceivedResponseSpanAsync<ListHostsReply, ListHostsResponse>(cookie.Id, token)
            .ConfigureAwait(false);
        if (mapping.ResponseType == XResponseType.Reply)
            return new ReplyLease<ListHostsReply>(result, this._socketAccessor.SocketIn.BufferPool);

        var error = result.ToStruct<XResponse>();
        this._socketAccessor.SocketIn.BufferPool.Return(result);
        throw new XEventException(new GenericError(error, mapping.ErrorMessageAction!));
    }

    public ReplyLease GetEvent() => this._socketAccessor.ReceivedEvent();

    public async ValueTask<ReplyLease> GetEventAsync(CancellationToken token = default) =>
        await this._socketAccessor
            .ReceivedEventAsync(token)
            .ConfigureAwait(false);

    public GenericError? CheckResponseProtoResult(ResponseProto response) =>
        this._socketAccessor.SocketIn.GetVoidRequestResponse<GenericError>(response);

    public void VerifyResponseProtoResult(ResponseProto response)
    {
        var error = this._socketAccessor.SocketIn.GetVoidRequestResponse<GenericError>(response);
        if (error.HasValue) throw new XEventException(error.Value);
    }

    private ResponseProto ChangeWindowAttributesBase(uint window, ValueMask mask, ReadOnlySpan<uint> args)
    {
        if (mask.CountFlags() != args.Length)
            throw new InsufficientDataException(mask.CountFlags(), args.Length, nameof(mask), nameof(args));

        var request = new ChangeWindowAttributesType(window, mask, args.Length);
        var bigRequest = new ChangeWindowAttributesBigType(window, mask, args.Length);
        SendWithBigRequestIfNeed(
            ref request,
            12,
            request.Length * 4,
            ref bigRequest,
            16,
            bigRequest.Length * 4,
            MemoryMarshal.Cast<uint, byte>(args)
        );

        return new ResponseProto(_socketAccessor.SocketOut.Sequence, true);
    }

    private ResponseProto DestroyWindowBase(uint window)
    {
        var request = new DestroyWindowType(window);
        _socketAccessor.SocketOut.Send(ref request);
        return new ResponseProto(_socketAccessor.SocketOut.Sequence);
    }

    private ResponseProto AllowEventsBase(EventsMode mode, uint time)
    {
        var request = new AllowEventsType(mode, time);
        _socketAccessor.SocketOut.Send(ref request);
        return new ResponseProto(_socketAccessor.SocketOut.Sequence);
    }

    private ResponseProto BellBase(sbyte percent)
    {
        if (percent is > 100 or < -100)
            throw new ArgumentOutOfRangeException(nameof(percent), "value must be between -100 to 100");

        var request = new BellType(percent);
        _socketAccessor.SocketOut.Send(ref request);
        return new ResponseProto(_socketAccessor.SocketOut.Sequence);
    }

    private ResponseProto ChangeActivePointerGrabBase(uint cursor, uint time, ushort mask)
    {
        var request = new ChangeActivePointerGrabType(cursor, time, mask);
        _socketAccessor.SocketOut.Send(ref request);
        return new ResponseProto(_socketAccessor.SocketOut.Sequence);
    }

    private ResponseProto ChangeGcBase(uint gc, GcMask mask, ReadOnlySpan<uint> args)
    {
        if (mask.CountFlags() != args.Length)
            throw new InsufficientDataException(mask.CountFlags(), args.Length, nameof(mask), nameof(args));

        var request = new ChangeGcType(gc, mask, args.Length);
        var bigRequest = new ChangeGcBigType(gc, mask, args.Length);
        SendWithBigRequestIfNeed(
            ref request,
            12,
            request.Length * 4,
            ref bigRequest,
            16,
            bigRequest.Length * 4,
            MemoryMarshal.Cast<uint, byte>(args));

        return new ResponseProto(_socketAccessor.SocketOut.Sequence, true);
    }

    private ResponseProto ChangeHostsBase(HostMode mode, Family family, ReadOnlySpan<byte> address)
    {
        var request = new ChangeHostsType(mode, family, address.Length);
        var bigRequest = new ChangeHostsBigType(mode, family, address.Length);
        SendWithBigRequestIfNeed(
            ref request,
            8,
            request.Length * 4,
            ref bigRequest,
            12,
            bigRequest.Length * 4,
            address
        );
        return new ResponseProto(_socketAccessor.SocketOut.Sequence, true);
    }

    private ResponseProto ChangeKeyboardControlBase(KeyboardControlMask mask, ReadOnlySpan<uint> args)
    {
        var request = new ChangeKeyboardControlType(mask, args.Length);
        var bigRequest = new ChangeKeyboardControlBigType(mask, args.Length);
        SendWithBigRequestIfNeed(
            ref request,
            8,
            request.Length * 4,
            ref bigRequest,
            12,
            bigRequest.Length * 4,
            MemoryMarshal.Cast<uint, byte>(args)
        );
        return new ResponseProto(_socketAccessor.SocketOut.Sequence, true);
    }

    private ResponseProto ChangeKeyboardMappingBase(byte keycodeCount, byte firstKeycode, byte keysymsPerKeycode,
        ReadOnlySpan<uint> keysym)
    {
        var request = new ChangeKeyboardMappingType(keycodeCount, firstKeycode, keysymsPerKeycode);
        var bigRequest = new ChangeKeyboardMappingBigType(keycodeCount, firstKeycode, keysymsPerKeycode);
        SendWithBigRequestIfNeed(
            ref request,
            8,
            request.Length * 4,
            ref bigRequest,
            12,
            bigRequest.Length * 4,
            MemoryMarshal.Cast<uint, byte>(keysym)
        );
        return new ResponseProto(_socketAccessor.SocketOut.Sequence, true);
    }

    private ResponseProto ChangePointerControlBase(Acceleration? acceleration, ushort? threshold)
    {
        var request = new ChangePointerControlType(acceleration?.Numerator ?? 0, acceleration?.Denominator ?? 0,
            threshold ?? 0, (byte)(acceleration is null ? 0 : 1), (byte)(threshold.HasValue ? 1 : 0));
        _socketAccessor.SocketOut.Send(ref request);
        return new ResponseProto(_socketAccessor.SocketOut.Sequence);
    }

    private ResponseProto ChangePropertyBase<T>(PropertyMode mode, uint window, ATOM property, ATOM type,
        ReadOnlySpan<T> args)
        where T : struct
#if !NETSTANDARD
        , INumber<T>
#endif
    {
        var size = Marshal.SizeOf<T>();
        if (size is not 1 and not 2 and not 4)
            throw new ArgumentException("type must be byte, sbyte, short, ushort, int, uint");
        var request = new ChangePropertyType(mode, window, property, type, args.Length, size);
        var bigRequest = new ChangePropertyBigType(mode, window, property, type, args.Length, size);
        SendWithBigRequestIfNeed(
            ref request,
            24,
            request.Length * 4,
            ref bigRequest,
            28,
            bigRequest.Length * 4,
            MemoryMarshal.Cast<T, byte>(args)
        );

        return new ResponseProto(_socketAccessor.SocketOut.Sequence, true);
    }

    private ResponseProto ChangeSaveSetBase(ChangeSaveSetMode changeSaveSetMode, uint window)
    {
        var request = new ChangeSaveSetType(changeSaveSetMode, window);
        _socketAccessor.SocketOut.Send(ref request);
        return new ResponseProto(_socketAccessor.SocketOut.Sequence);
    }

    private ResponseProto CirculateWindowBase(Circulate circulate, uint window)
    {
        var request = new CirculateWindowType(circulate, window);
        _socketAccessor.SocketOut.Send(ref request);
        return new ResponseProto(_socketAccessor.SocketOut.Sequence);
    }

    private ResponseProto ClearAreaBase(bool exposures, uint window, short x, short y, ushort width, ushort height)
    {
        var request = new ClearAreaType(exposures, window, x, y, width, height);
        _socketAccessor.SocketOut.Send(ref request);
        return new ResponseProto(_socketAccessor.SocketOut.Sequence);
    }

    private ResponseProto CloseFontBase(uint fontId)
    {
        var request = new CloseFontType(fontId);
        _socketAccessor.SocketOut.Send(ref request);
        return new ResponseProto(_socketAccessor.SocketOut.Sequence);
    }

    private ResponseProto ConfigureWindowBase(uint window, ConfigureValueMask mask, ReadOnlySpan<uint> args)
    {
        if (mask.CountFlags() != args.Length)
            throw new InsufficientDataException(mask.CountFlags(), args.Length, nameof(mask), nameof(args));

        var request = new ConfigureWindowType(window, mask, args.Length);
        var bigRequest = new ConfigureWindowBigType(window, mask, args.Length);
        SendWithBigRequestIfNeed(
            ref request,
            12,
            request.Length * 4,
            ref bigRequest,
            16,
            bigRequest.Length * 4,
            MemoryMarshal.Cast<uint, byte>(args)
        );
        return new ResponseProto(_socketAccessor.SocketOut.Sequence, true);
    }

    private ResponseProto ConvertSelectionBase(uint requestor, ATOM selection, ATOM target, ATOM property,
        uint timestamp)
    {
        var request = new ConvertSelectionType(requestor, selection, target, property, timestamp);
        _socketAccessor.SocketOut.Send(ref request);
        return new ResponseProto(_socketAccessor.SocketOut.Sequence);
    }

    private ResponseProto CopyAreaBase(uint srcDrawable, uint destinationDrawable, uint gc, ushort srcX, ushort srcY,
        ushort destinationX, ushort destinationY, ushort width, ushort height)
    {
        var request = new CopyAreaType(srcDrawable, destinationDrawable, gc, srcX, srcY, destinationX, destinationY,
            width, height);
        _socketAccessor.SocketOut.Send(ref request);
        return new ResponseProto(_socketAccessor.SocketOut.Sequence);
    }

    private ResponseProto CopyColormapAndFreeBase(uint colormapId, uint srcColormapId)
    {
        var request = new CopyColormapAndFreeType(colormapId, srcColormapId);
        _socketAccessor.SocketOut.Send(ref request);
        return new ResponseProto(_socketAccessor.SocketOut.Sequence);
    }

    private ResponseProto CopyGcBase(uint srcGc, uint dstGc, GcMask mask)
    {
        var request = new CopyGcType(srcGc, dstGc, mask);
        _socketAccessor.SocketOut.Send(ref request);
        return new ResponseProto(_socketAccessor.SocketOut.Sequence);
    }

    private ResponseProto CopyPlaneBase(uint srcDrawable, uint destinationDrawable, uint gc, ushort srcX, ushort srcY,
        ushort destinationX, ushort destinationY, ushort width, ushort height, uint bitPlane)
    {
        var request = new CopyPlaneType(srcDrawable, destinationDrawable, gc, srcX, srcY, destinationX, destinationY,
            width, height, bitPlane);
        _socketAccessor.SocketOut.Send(ref request);
        return new ResponseProto(_socketAccessor.SocketOut.Sequence);
    }

    private ResponseProto CreateColormapBase(ColormapAlloc alloc, uint colormapId, uint window, uint visual)
    {
        var request = new CreateColormapType(alloc, colormapId, window, visual);
        _socketAccessor.SocketOut.Send(ref request);
        return new ResponseProto(_socketAccessor.SocketOut.Sequence);
    }

    private ResponseProto CreateCursorBase(uint cursorId, uint source, uint mask, ushort foreRed, ushort foreGreen,
        ushort foreBlue, ushort backRed, ushort backGreen, ushort backBlue, ushort x, ushort y)
    {
        var request = new CreateCursorType(cursorId, source, mask, foreRed, foreGreen, foreBlue, backRed, backGreen,
            backBlue, x, y);
        _socketAccessor.SocketOut.Send(ref request);
        return new ResponseProto(_socketAccessor.SocketOut.Sequence);
    }

    private ResponseProto CreateGcBase(uint gc, uint drawable, GcMask mask, ReadOnlySpan<uint> args)
    {
        if (mask.CountFlags() != args.Length)
            throw new InsufficientDataException(mask.CountFlags(), args.Length, nameof(mask), nameof(args));

        var request = new CreateGcType(gc, drawable, mask, args.Length);
        var bigRequest = new CreateGcBigType(gc, drawable, mask, args.Length);
        SendWithBigRequestIfNeed(
            ref request,
            16,
            request.Length * 4,
            ref bigRequest,
            20,
            bigRequest.Length * 4,
            MemoryMarshal.Cast<uint, byte>(args));
        return new ResponseProto(_socketAccessor.SocketOut.Sequence, true);
    }

    private ResponseProto CreateGlyphCursorBase(uint cursorId, uint sourceFont, uint fontMask, char sourceChar,
        ushort charMask, ushort foreRed, ushort foreGreen, ushort foreBlue, ushort backRed, ushort backGreen,
        ushort backBlue)
    {
        var request = new CreateGlyphCursorType(cursorId, sourceFont, fontMask, sourceChar, charMask, foreRed,
            foreGreen, foreBlue, backRed, backGreen, backBlue);
        _socketAccessor.SocketOut.Send(ref request);
        return new ResponseProto(_socketAccessor.SocketOut.Sequence);
    }

    private ResponseProto CreatePixmapBase(byte depth, uint pixmapId, uint drawable, ushort width, ushort height)
    {
        var request = new CreatePixmapType(depth, pixmapId, drawable, width, height);
        _socketAccessor.SocketOut.Send(ref request);
        return new ResponseProto(_socketAccessor.SocketOut.Sequence);
    }

    private ResponseProto CreateWindowBase(byte depth, uint window, uint parent, short x, short y, ushort width,
        ushort height, ushort borderWidth, ClassType classType, uint rootVisualId, ValueMask mask,
        ReadOnlySpan<uint> args)
    {
        var request = new CreateWindowType(depth, window, parent, x, y, width, height, borderWidth, classType,
            rootVisualId, mask, args.Length);
        var requiredBuffer = request.Length * 4;
        if (requiredBuffer < _bigRequestLength)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
            scratchBuffer.WriteRequest(
                ref request,
                32,
                MemoryMarshal.Cast<uint, byte>(args));
            _socketAccessor.SocketOut.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            var workingBuffer = scratchBuffer[..requiredBuffer];
            workingBuffer.WriteRequest(
                ref request,
                32,
                MemoryMarshal.Cast<uint, byte>(args));
            _socketAccessor.SocketOut.SendExact(workingBuffer[..requiredBuffer]);
        }

        return new ResponseProto(_socketAccessor.SocketOut.Sequence);
    }

    private ResponseProto DeletePropertyBase(uint window, ATOM atom)
    {
        var request = new DeletePropertyType(window, atom);
        _socketAccessor.SocketOut.Send(ref request);
        return new ResponseProto(_socketAccessor.SocketOut.Sequence);
    }

    private ResponseProto DestroySubwindowsBase(uint window)
    {
        var request = new DestroySubWindowsType(window);
        _socketAccessor.SocketOut.Send(ref request);
        return new ResponseProto(_socketAccessor.SocketOut.Sequence);
    }

    private ResponseProto FillPolyBase(uint drawable, uint gc, PolyShape shape, CoordinateMode coordinate,
        ReadOnlySpan<Point> points)
    {
        var request = new FillPolyType(drawable, gc, shape, coordinate, points.Length);
        var bigRequest = new FillPolyBigType(drawable, gc, shape, coordinate, points.Length);
        SendWithBigRequestIfNeed(
            ref request,
            16,
            request.Length * 4,
            ref bigRequest,
            20,
            bigRequest.Length * 4,
            MemoryMarshal.Cast<Point, byte>(points));
        return new ResponseProto(_socketAccessor.SocketOut.Sequence);
    }

    private ResponseProto ForceScreenSaverBase(ForceScreenSaverMode mode)
    {
        var request = new ForceScreenSaverType(mode);
        _socketAccessor.SocketOut.Send(ref request);
        return new ResponseProto(_socketAccessor.SocketOut.Sequence);
    }

    private ResponseProto FreeColormapBase(uint colormapId)
    {
        var request = new FreeColormapType(colormapId);
        _socketAccessor.SocketOut.Send(ref request);
        return new ResponseProto(_socketAccessor.SocketOut.Sequence);
    }

    private ResponseProto FreeColorsBase(uint colormapId, uint planeMask, ReadOnlySpan<uint> pixels)
    {
        var request = new FreeColorsType(colormapId, planeMask, pixels.Length);
        var bigRequest = new FreeColorsBigType(colormapId, planeMask, pixels.Length);
        SendWithBigRequestIfNeed(
            ref request,
            12,
            request.Length * 4,
            ref bigRequest,
            16,
            bigRequest.Length * 4,
            MemoryMarshal.Cast<uint, byte>(pixels));
        return new ResponseProto(_socketAccessor.SocketOut.Sequence);
    }

    private ResponseProto FreeCursorBase(uint cursorId)
    {
        var request = new FreeCursorType(cursorId);
        _socketAccessor.SocketOut.Send(ref request);
        return new ResponseProto(_socketAccessor.SocketOut.Sequence);
    }

    private ResponseProto FreeGcBase(uint gc)
    {
        var request = new FreeGcType(gc);
        _socketAccessor.SocketOut.Send(ref request);
        return new ResponseProto(_socketAccessor.SocketOut.Sequence);
    }

    private ResponseProto FreePixmapBase(uint pixmapId)
    {
        var request = new FreePixmapType(pixmapId);
        _socketAccessor.SocketOut.Send(ref request);
        return new ResponseProto(_socketAccessor.SocketOut.Sequence);
    }

    private ResponseProto GrabButtonBase(bool ownerEvents, uint grabWindow, ushort mask, GrabMode pointerMode,
        GrabMode keyboardMode, uint confineTo, uint cursor, Button button, ModifierMask modifiers)
    {
        var request = new GrabButtonType(ownerEvents, grabWindow, mask, pointerMode, keyboardMode, confineTo, cursor,
            button, modifiers);
        _socketAccessor.SocketOut.Send(ref request);
        return new ResponseProto(_socketAccessor.SocketOut.Sequence);
    }

    private ResponseProto GrabKeyBase(bool exposures, uint grabWindow, ModifierMask mask, byte keycode,
        GrabMode pointerMode, GrabMode keyboardMode)
    {
        var request = new GrabKeyType(exposures, grabWindow, mask, keycode, pointerMode, keyboardMode);
        _socketAccessor.SocketOut.Send(ref request);
        return new ResponseProto(_socketAccessor.SocketOut.Sequence);
    }

    private ResponseProto GrabServerBase()
    {
        var request = new GrabServerType();
        _socketAccessor.SocketOut.Send(ref request);
        return new ResponseProto(_socketAccessor.SocketOut.Sequence);
    }

    private ResponseProto ImageText16Base(uint drawable, uint gc, short x, short y, ReadOnlySpan<char> text)
    {
        var request = new ImageText16Type(drawable, gc, x, y, text.Length);
        var requiredBuffer = request.Length * 4;
        if (requiredBuffer < _bigRequestLength)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];

#if NETSTANDARD
            MemoryMarshal.Write(scratchBuffer[0..16], ref request);
#else
            MemoryMarshal.Write(scratchBuffer[..16], in request);
#endif
            Encoding.BigEndianUnicode.GetBytes(text, scratchBuffer[16..(text.Length * 2 + 16)]);
            scratchBuffer[(text.Length * 2 + 16)..requiredBuffer].Clear();
            _socketAccessor.SocketOut.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);

#if NETSTANDARD
            MemoryMarshal.Write(scratchBuffer[0..16], ref request);
#else
            MemoryMarshal.Write(scratchBuffer[..16], in request);
#endif
            Encoding.BigEndianUnicode.GetBytes(text, scratchBuffer[16..(text.Length * 2 + 16)]);
            scratchBuffer[(text.Length * 2 + 16)..requiredBuffer].Clear();
            _socketAccessor.SocketOut.SendExact(scratchBuffer[..requiredBuffer]);
        }

        return new ResponseProto(_socketAccessor.SocketOut.Sequence);
    }

    private ResponseProto ImageText8Base(uint drawable, uint gc, short x, short y, ReadOnlySpan<byte> text)
    {
        var request = new ImageText8Type(drawable, gc, x, y, text.Length);
        var bigRequest = new ImageText8BigType(drawable, gc, x, y, text.Length);
        SendWithBigRequestIfNeed(
            ref request,
            16,
            request.Length * 4,
            ref bigRequest,
            20,
            bigRequest.Length * 4,
            text);
        return new ResponseProto(_socketAccessor.SocketOut.Sequence);
    }

    private ResponseProto InstallColormapBase(uint colormapId)
    {
        var request = new InstallColormapType(colormapId);
        _socketAccessor.SocketOut.Send(ref request);
        return new ResponseProto(_socketAccessor.SocketOut.Sequence);
    }

    private ResponseProto KillClientBase(uint resource)
    {
        var request = new KillClientType(resource);
        _socketAccessor.SocketOut.Send(ref request);
        return new ResponseProto(_socketAccessor.SocketOut.Sequence);
    }

    private ResponseProto MapSubwindowsBase(uint window)
    {
        var request = new MapSubWindowsType(window);
        _socketAccessor.SocketOut.Send(ref request);
        return new ResponseProto(_socketAccessor.SocketOut.Sequence);
    }

    private ResponseProto MapWindowBase(uint window)
    {
        var request = new MapWindowType(window);
        _socketAccessor.SocketOut.Send(ref request);
        return new ResponseProto(_socketAccessor.SocketOut.Sequence);
    }

    private ResponseProto NoOperationBase(ReadOnlySpan<uint> args)
    {
        var request = new NoOperationType(args.Length);
        var requiredBuffer = request.Length * 4;
        if (requiredBuffer < _bigRequestLength)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
            scratchBuffer.WriteRequest(
                ref request,
                4,
                MemoryMarshal.Cast<uint, byte>(args));
            _socketAccessor.SocketOut.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            var workingBuffer = scratchBuffer[..requiredBuffer];
            workingBuffer.WriteRequest(
                ref request,
                4,
                MemoryMarshal.Cast<uint, byte>(args));
            _socketAccessor.SocketOut.SendExact(workingBuffer[..requiredBuffer]);
        }

        return new ResponseProto(_socketAccessor.SocketOut.Sequence);
    }

    private ResponseProto OpenFontBase(string fontName, uint fontId)
    {
        var request = new OpenFontType(fontId, (ushort)fontName.Length);
        var requiredBuffer = request.Length * 4;
        if (requiredBuffer < _bigRequestLength)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
#if NETSTANDARD
            MemoryMarshal.Write(scratchBuffer[0..12], ref request);
#else
            MemoryMarshal.Write(scratchBuffer[..12], in request);
#endif
            Encoding.ASCII.GetBytes(fontName, scratchBuffer[12..(fontName.Length + 12)]);
            scratchBuffer[(fontName.Length + 12)..requiredBuffer].Clear();
            _socketAccessor.SocketOut.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
#if NETSTANDARD
            MemoryMarshal.Write(scratchBuffer[0..12], ref request);
#else
            MemoryMarshal.Write(scratchBuffer[..12], in request);
#endif
            Encoding.ASCII.GetBytes(fontName, scratchBuffer[12..(fontName.Length + 12)]);
            scratchBuffer[(fontName.Length + 12)..requiredBuffer].Clear();
            _socketAccessor.SocketOut.SendExact(scratchBuffer[..requiredBuffer]);
        }

        return new ResponseProto(_socketAccessor.SocketOut.Sequence);
    }

    private ResponseProto PolyArcBase(uint drawable, uint gc, ReadOnlySpan<Arc> arcs)
    {
        var request = new PolyArcType(drawable, gc, arcs.Length);
        var bigRequest = new PolyArcBigType(drawable, gc, arcs.Length);
        SendWithBigRequestIfNeed(
            ref request,
            12,
            request.Length * 4,
            ref bigRequest,
            16,
            bigRequest.Length * 4,
            MemoryMarshal.Cast<Arc, byte>(arcs));
        return new ResponseProto(_socketAccessor.SocketOut.Sequence);
    }

    private ResponseProto PolyFillArcBase(uint drawable, uint gc, ReadOnlySpan<Arc> arcs)
    {
        var request = new PolyFillArcType(drawable, gc, arcs.Length);
        var bigRequest = new PolyFillArcBigType(drawable, gc, arcs.Length);
        SendWithBigRequestIfNeed(
            ref request,
            12,
            request.Length * 4,
            ref bigRequest,
            16,
            bigRequest.Length * 4,
            MemoryMarshal.Cast<Arc, byte>(arcs));
        return new ResponseProto(_socketAccessor.SocketOut.Sequence);
    }

    private ResponseProto PolyFillRectangleBase(uint drawable, uint gc, ReadOnlySpan<Rectangle> rectangles)
    {
        var request = new PolyFillRectangleType(drawable, gc, rectangles.Length);
        var bigRequest = new PolyFillRectangleBigType(drawable, gc, rectangles.Length);
        SendWithBigRequestIfNeed(
            ref request,
            12,
            request.Length * 4,
            ref bigRequest,
            16,
            bigRequest.Length * 4,
            MemoryMarshal.Cast<Rectangle, byte>(rectangles));
        return new ResponseProto(_socketAccessor.SocketOut.Sequence);
    }

    private ResponseProto PolyLineBase(CoordinateMode coordinate, uint drawable, uint gc, ReadOnlySpan<Point> points)
    {
        var request = new PolyLineType(coordinate, drawable, gc, points.Length);
        var bigRequest = new PolyLineBigType(coordinate, drawable, gc, points.Length);
        SendWithBigRequestIfNeed(
            ref request,
            12,
            request.Length * 4,
            ref bigRequest,
            16,
            bigRequest.Length * 4,
            MemoryMarshal.Cast<Point, byte>(points));
        return new ResponseProto(_socketAccessor.SocketOut.Sequence);
    }

    private ResponseProto PolyPointBase(CoordinateMode coordinate, uint drawable, uint gc, ReadOnlySpan<Point> points)
    {
        var request = new PolyPointType(coordinate, drawable, gc, points.Length);
        var bigRequest = new PolyPointBigType(coordinate, drawable, gc, points.Length);
        SendWithBigRequestIfNeed(
            ref request,
            12,
            request.Length * 4,
            ref bigRequest,
            16,
            bigRequest.Length * 4,
            MemoryMarshal.Cast<Point, byte>(points));
        return new ResponseProto(_socketAccessor.SocketOut.Sequence);
    }

    private ResponseProto PolyRectangleBase(uint drawable, uint gc, ReadOnlySpan<Rectangle> rectangles)
    {
        var request = new PolyRectangleType(drawable, gc, rectangles.Length);
        var bigRequest = new PolyRectangleBigType(drawable, gc, rectangles.Length);
        SendWithBigRequestIfNeed(
            ref request,
            12,
            request.Length * 4,
            ref bigRequest,
            16,
            bigRequest.Length * 4,
            MemoryMarshal.Cast<Rectangle, byte>(rectangles));
        return new ResponseProto(_socketAccessor.SocketOut.Sequence);
    }

    private ResponseProto PolySegmentBase(uint drawable, uint gc, ReadOnlySpan<Segment> segments)
    {
        var request = new PolySegmentType(drawable, gc, segments.Length);
        var bigRequest = new PolySegmentBigType(drawable, gc, segments.Length);
        SendWithBigRequestIfNeed(
            ref request,
            12,
            request.Length * 4,
            ref bigRequest,
            16,
            bigRequest.Length * 4,
            MemoryMarshal.Cast<Segment, byte>(segments));
        return new ResponseProto(_socketAccessor.SocketOut.Sequence);
    }

    private ResponseProto PolyText16Base(uint drawable, uint gc, ushort x, ushort y, TextItem16[] data)
    {
        var totalItemCount = data.Sum(a => a.Count);
        var request = new PolyText16Type(drawable, gc, x, y, totalItemCount);
        var requiredBuffer = request.Length * 4;
        var writIndex = 16;
        if (requiredBuffer < _bigRequestLength)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
#if NETSTANDARD
            MemoryMarshal.Write(scratchBuffer[0..16], ref request);
#else
            MemoryMarshal.Write(scratchBuffer[..16], in request);
#endif
            foreach (var item in data)
                writIndex += item.CopyTo(scratchBuffer.Slice(writIndex, item.Count));

            scratchBuffer[^totalItemCount.Padding()..].Clear();
            _socketAccessor.SocketOut.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            var workingBuffer = scratchBuffer[..requiredBuffer];
#if NETSTANDARD
            MemoryMarshal.Write(workingBuffer[0..16], ref request);
#else
            MemoryMarshal.Write(workingBuffer[..16], in request);
#endif
            foreach (var item in data)
                writIndex += item.CopyTo(workingBuffer.Slice(writIndex, item.Count));

            workingBuffer[^totalItemCount.Padding()..].Clear();
            _socketAccessor.SocketOut.SendExact(workingBuffer);
        }

        return new ResponseProto(_socketAccessor.SocketOut.Sequence);
    }

    private ResponseProto PolyText8Base(uint drawable, uint gc, ushort x, ushort y, TextItem8[] data)
    {
        var totalItemCount = data.Sum(a => a.Count);
        var request = new PolyText8Type(drawable, gc, x, y, totalItemCount);
        var requiredBuffer = request.Length * 4;
        var writIndex = 16;
        if (requiredBuffer < _bigRequestLength)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
#if NETSTANDARD
            MemoryMarshal.Write(scratchBuffer[0..16], ref request);
#else
            MemoryMarshal.Write(scratchBuffer[..16], in request);
#endif
            foreach (var item in data)
                writIndex += item.CopyTo(scratchBuffer.Slice(writIndex, item.Count));
            scratchBuffer[^totalItemCount.Padding()..].Clear();
            _socketAccessor.SocketOut.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            var workingBuffer = scratchBuffer[..requiredBuffer];
#if NETSTANDARD
            MemoryMarshal.Write(workingBuffer[0..16], ref request);
#else
            MemoryMarshal.Write(workingBuffer[..16], in request);
#endif
            foreach (var item in data)
                writIndex += item.CopyTo(workingBuffer.Slice(writIndex, item.Count));
            workingBuffer[^totalItemCount.Padding()..].Clear();
            _socketAccessor.SocketOut.SendExact(workingBuffer);
        }

        return new ResponseProto(_socketAccessor.SocketOut.Sequence);
    }

    private ResponseProto PutImageBase(ImageFormatBitmap format, uint drawable, uint gc, ushort width, ushort height,
        short x, short y, byte leftPad, byte depth, ReadOnlySpan<byte> data)
    {
        var request = new PutImageType(format, drawable, gc, width, height, x, y, leftPad, depth, data.Length);
        var bigRequest = new PutImageBigType(format, drawable, gc, width, height, x, y, leftPad, depth, data.Length);
        SendWithBigRequestIfNeed(
            ref request,
            24,
            request.Length * 4,
            ref bigRequest,
            28,
            bigRequest.Length * 4,
            data
        );
        return new ResponseProto(_socketAccessor.SocketOut.Sequence, true);
    }

    private ResponseProto RecolorCursorBase(uint cursorId, ushort foreRed, ushort foreGreen, ushort foreBlue,
        ushort backRed, ushort backGreen, ushort backBlue)
    {
        var request = new RecolorCursorType(cursorId, foreRed, foreGreen, foreBlue, backRed, backGreen, backBlue);
        _socketAccessor.SocketOut.Send(ref request);
        return new ResponseProto(_socketAccessor.SocketOut.Sequence);
    }

    private ResponseProto ReparentWindowBase(uint window, uint parent, short x, short y)
    {
        var request = new ReparentWindowType(window, parent, x, y);
        _socketAccessor.SocketOut.Send(ref request);
        return new ResponseProto(_socketAccessor.SocketOut.Sequence);
    }

    private ResponseProto RotatePropertiesBase(uint window, ushort delta, ReadOnlySpan<ATOM> properties)
    {
        var request = new RotatePropertiesType(window, properties.Length, delta);
        var bigRequest = new RotatePropertiesBigType(window, properties.Length, delta);
        SendWithBigRequestIfNeed(
            ref request,
            12,
            request.Length * 4,
            ref bigRequest,
            16,
            bigRequest.Length * 4,
            MemoryMarshal.Cast<ATOM, byte>(properties));
        return new ResponseProto(_socketAccessor.SocketOut.Sequence);
    }

    private ResponseProto SendEventBase(bool propagate, uint destination, uint eventMask, GenericEvent evnt)
    {
        var request = new SendEventType(propagate, destination, eventMask, evnt.GetResponse());
        _socketAccessor.SocketOut.Send(ref request);
        return new ResponseProto(_socketAccessor.SocketOut.Sequence);
    }

    private ResponseProto SetAccessControlBase(AccessControlMode mode)
    {
        var request = new SetAccessControlType(mode);
        _socketAccessor.SocketOut.Send(ref request);
        return new ResponseProto(_socketAccessor.SocketOut.Sequence);
    }

    private ResponseProto SetClipRectanglesBase(ClipOrdering ordering, uint gc, ushort clipX, ushort clipY,
        ReadOnlySpan<Rectangle> rectangles)
    {
        var request = new SetClipRectanglesType(ordering, gc, clipX, clipY, rectangles.Length);
        var bigRequest = new SetClipRectanglesBigType(ordering, gc, clipX, clipY, rectangles.Length);
        SendWithBigRequestIfNeed(
            ref request,
            12,
            request.Length * 4,
            ref bigRequest,
            16,
            bigRequest.Length * 4,
            MemoryMarshal.Cast<Rectangle, byte>(rectangles));
        return new ResponseProto(_socketAccessor.SocketOut.Sequence);
    }

    private ResponseProto SetCloseDownModeBase(CloseDownMode mode)
    {
        var request = new SetCloseDownModeType(mode);
        _socketAccessor.SocketOut.Send(ref request);
        return new ResponseProto(_socketAccessor.SocketOut.Sequence);
    }

    private ResponseProto SetDashesBase(uint gc, ushort dashOffset, ReadOnlySpan<byte> dashes)
    {
        var request = new SetDashesType(gc, dashOffset, dashes.Length);
        var bigRequest = new SetDashesBigType(gc, dashOffset, dashes.Length);
        SendWithBigRequestIfNeed(
            ref request,
            12,
            request.Length * 4,
            ref bigRequest,
            16,
            bigRequest.Length * 4,
            dashes);
        return new ResponseProto(_socketAccessor.SocketOut.Sequence);
    }

    private ResponseProto SetFontPathBase(string[] strPaths)
    {
        strPaths = strPaths.Where(a => a != "fixed").ToArray();
        var totalPathLength = strPaths.Sum(a => a.Length + 1);
        var request = new SetFontPathType((ushort)strPaths.Length, totalPathLength.AddPadding());
        var requiredBuffer = request.Length * 4;
        var writIndex = 8;
        if (requiredBuffer < _bigRequestLength)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
#if NETSTANDARD
            MemoryMarshal.Write(scratchBuffer[0..8], ref request);
#else
            MemoryMarshal.Write(scratchBuffer[..8], in request);
#endif
            foreach (var item in strPaths.OrderBy(a => a.Length))
            {
                scratchBuffer[writIndex++] = (byte)item.Length;
                writIndex += Encoding.ASCII.GetBytes(item, scratchBuffer.Slice(writIndex, item.Length));
            }

            scratchBuffer[^totalPathLength.Padding()..].Clear();
            _socketAccessor.SocketOut.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
#if NETSTANDARD
            MemoryMarshal.Write(scratchBuffer[0..8], ref request);
#else
            MemoryMarshal.Write(scratchBuffer[..8], in request);
#endif
            foreach (var item in strPaths.OrderBy(a => a.Length))
            {
                scratchBuffer[writIndex++] = (byte)item.Length;
                writIndex += Encoding.ASCII.GetBytes(item, scratchBuffer.Slice(writIndex, item.Length));
            }

            scratchBuffer[^totalPathLength.Padding()..].Clear();
            _socketAccessor.SocketOut.SendExact(scratchBuffer[..requiredBuffer]);
        }

        return new ResponseProto(_socketAccessor.SocketOut.Sequence);
    }

    private ResponseProto SetInputFocusBase(InputFocusMode mode, uint focus, uint time)
    {
        var request = new SetInputFocusType(mode, focus, time);
        _socketAccessor.SocketOut.Send(ref request);
        return new ResponseProto(_socketAccessor.SocketOut.Sequence);
    }

    private ResponseProto SetScreenSaverBase(short timeout, short interval, TriState preferBlanking,
        TriState allowExposures)
    {
        var request = new SetScreenSaverType(timeout, interval, preferBlanking, allowExposures);
        _socketAccessor.SocketOut.Send(ref request);
        return new ResponseProto(_socketAccessor.SocketOut.Sequence);
    }

    private ResponseProto SetSelectionOwnerBase(uint owner, ATOM atom, uint timestamp)
    {
        var request = new SetSelectionOwnerType(owner, atom, timestamp);
        _socketAccessor.SocketOut.Send(ref request);
        return new ResponseProto(_socketAccessor.SocketOut.Sequence);
    }

    private ResponseProto StoreColorsBase(uint colormapId, ReadOnlySpan<ColorItem> item)
    {
        var request = new StoreColorsType(colormapId, item.Length);
        var bigRequest = new StoreColorsBigType(colormapId, item.Length);
        SendWithBigRequestIfNeed(
            ref request,
            8,
            request.Length * 4,
            ref bigRequest,
            12,
            bigRequest.Length * 4,
            MemoryMarshal.Cast<ColorItem, byte>(item));
        return new ResponseProto(_socketAccessor.SocketOut.Sequence);
    }

    private ResponseProto StoreNamedColorBase(ColorFlag mode, uint colormapId, uint pixels, ReadOnlySpan<byte> name)
    {
        var request = new StoreNamedColorType(mode, colormapId, pixels, name.Length);
        var requiredBuffer = request.Length * 4;
        if (requiredBuffer < _bigRequestLength)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
            scratchBuffer.WriteRequest(
                ref request,
                16,
                name);
            _socketAccessor.SocketOut.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            var workingBuffer = scratchBuffer[..requiredBuffer];
            workingBuffer.WriteRequest(
                ref request,
                16,
                name);
            _socketAccessor.SocketOut.SendExact(workingBuffer[..requiredBuffer]);
        }

        return new ResponseProto(_socketAccessor.SocketOut.Sequence);
    }

    private ResponseProto UngrabButtonBase(Button button, uint grabWindow, ModifierMask mask)
    {
        var request = new UngrabButtonType(button, grabWindow, mask);
        _socketAccessor.SocketOut.Send(ref request);
        return new ResponseProto(_socketAccessor.SocketOut.Sequence);
    }

    private ResponseProto UngrabKeyBase(byte key, uint grabWindow, ModifierMask modifier)
    {
        var request = new UngrabKeyType(key, grabWindow, modifier);
        _socketAccessor.SocketOut.Send(ref request);
        return new ResponseProto(_socketAccessor.SocketOut.Sequence);
    }

    private ResponseProto UngrabKeyboardBase(uint time)
    {
        var request = new UngrabKeyboardType(time);
        _socketAccessor.SocketOut.Send(ref request);
        return new ResponseProto(_socketAccessor.SocketOut.Sequence);
    }

    private ResponseProto UngrabPointerBase(uint time)
    {
        var request = new UngrabPointerType(time);
        _socketAccessor.SocketOut.Send(ref request);
        return new ResponseProto(_socketAccessor.SocketOut.Sequence);
    }

    private ResponseProto UngrabServerBase()
    {
        var request = new UnGrabServerType();
        _socketAccessor.SocketOut.Send(ref request);
        return new ResponseProto(_socketAccessor.SocketOut.Sequence);
    }

    private ResponseProto UninstallColormapBase(uint colormapId)
    {
        var request = new UninstallColormapType(colormapId);
        _socketAccessor.SocketOut.Send(ref request);
        return new ResponseProto(_socketAccessor.SocketOut.Sequence);
    }

    private ResponseProto UnmapSubwindowsBase(uint window)
    {
        var request = new UnMapSubwindowsType(window);
        _socketAccessor.SocketOut.Send(ref request);
        return new ResponseProto(_socketAccessor.SocketOut.Sequence);
    }

    private ResponseProto UnmapWindowBase(uint window)
    {
        var request = new UnmapWindowType(window);
        _socketAccessor.SocketOut.Send(ref request);
        return new ResponseProto(_socketAccessor.SocketOut.Sequence);
    }

    private ResponseProto WarpPointerBase(uint srcWindow, uint destinationWindow, short srcX, short srcY,
        ushort srcWidth, ushort srcHeight, short destinationX, short destinationY)
    {
        var request = new WarpPointerType(srcWindow, destinationWindow, srcX, srcY, srcWidth, srcHeight, destinationX,
            destinationY);
        _socketAccessor.SocketOut.Send(ref request);
        return new ResponseProto(_socketAccessor.SocketOut.Sequence);
    }

    private ResponseProto AllocColorBase(uint colorMap, ushort red, ushort green, ushort blue)
    {
        var request = new AllocColorType(colorMap, red, green, blue);
        _socketAccessor.SocketOut.Send(ref request);
        return new ResponseProto(_socketAccessor.SocketOut.Sequence, true);
    }

    private ResponseProto QueryPointerBase(uint window)
    {
        var request = new QueryPointerType(window);
        _socketAccessor.SocketOut.Send(ref request);
        return new ResponseProto(_socketAccessor.SocketOut.Sequence, true);
    }

    private ResponseProto GrabPointerBase(bool ownerEvents, uint grabWindow, ushort mask, GrabMode pointerMode,
        GrabMode keyboardMode, uint confineTo, uint cursor, uint timeStamp)
    {
        var request = new GrabPointerType(ownerEvents, grabWindow, mask, pointerMode, keyboardMode, confineTo, cursor,
            timeStamp);
        _socketAccessor.SocketOut.Send(ref request);
        return new ResponseProto(_socketAccessor.SocketOut.Sequence, true);
    }

    private ResponseProto InternAtomBase(bool onlyIfExist, string atomName)
    {
        var request = new InternAtomType(onlyIfExist, atomName.Length);
        var requiredBuffer = request.Length * 4;
        if (requiredBuffer < _bigRequestLength)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
#if NETSTANDARD
            MemoryMarshal.Write(scratchBuffer[0..8], ref request);
#else
            MemoryMarshal.Write(scratchBuffer[..8], in request);
#endif
            Encoding.ASCII.GetBytes(atomName, scratchBuffer[8..(atomName.Length + 8)]);
            scratchBuffer[(atomName.Length + 8)..requiredBuffer].Clear();
            _socketAccessor.SocketOut.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
#if NETSTANDARD
            MemoryMarshal.Write(scratchBuffer[0..8], ref request);
#else
            MemoryMarshal.Write(scratchBuffer[..8], in request);
#endif
            Encoding.ASCII.GetBytes(atomName, scratchBuffer[8..(atomName.Length + 8)]);
            scratchBuffer[(atomName.Length + 8)..requiredBuffer].Clear();
            _socketAccessor.SocketOut.SendExact(scratchBuffer[..requiredBuffer]);
        }

        return new ResponseProto(_socketAccessor.SocketOut.Sequence, true);
    }

    private ResponseProto GetPropertyBase(bool delete, uint window, ATOM property, ATOM type, uint offset,
        uint length)
    {
        var request = new GetPropertyType(delete, window, property, type, offset, length);
        _socketAccessor.SocketOut.Send(ref request);
        return new ResponseProto(_socketAccessor.SocketOut.Sequence, true);
    }

    private ResponseProto GetWindowAttributesBase(uint window)
    {
        var request = new GetWindowAttributesType(window);
        _socketAccessor.SocketOut.Send(ref request);
        return new ResponseProto(_socketAccessor.SocketOut.Sequence, true);
    }

    private ResponseProto GetGeometryBase(uint drawable)
    {
        var request = new GetGeometryType(drawable);
        _socketAccessor.SocketOut.Send(ref request);
        return new ResponseProto(_socketAccessor.SocketOut.Sequence, true);
    }

    private ResponseProto QueryTreeBase(uint window)
    {
        var request = new QueryTreeType(window);
        _socketAccessor.SocketOut.Send(ref request);
        return new ResponseProto(_socketAccessor.SocketOut.Sequence, true);
    }

    private ResponseProto GetAtomNameBase(ATOM atom)
    {
        var request = new GetAtomNameType(atom);
        _socketAccessor.SocketOut.Send(ref request);
        return new ResponseProto(_socketAccessor.SocketOut.Sequence, true);
    }

    private ResponseProto ListPropertiesBase(uint window)
    {
        var request = new ListPropertiesType(window);
        _socketAccessor.SocketOut.Send(ref request);
        return new ResponseProto(_socketAccessor.SocketOut.Sequence, true);
    }

    private ResponseProto GetSelectionOwnerBase(ATOM atom)
    {
        var request = new GetSelectionOwnerType(atom);
        _socketAccessor.SocketOut.Send(ref request);
        return new ResponseProto(_socketAccessor.SocketOut.Sequence, true);
    }

    private ResponseProto GrabKeyboardBase(bool ownerEvents, uint grabWindow, uint timeStamp, GrabMode pointerMode,
        GrabMode keyboardMode)
    {
        var request = new GrabKeyboardType(ownerEvents, grabWindow, timeStamp, pointerMode, keyboardMode);
        _socketAccessor.SocketOut.Send(ref request);
        return new ResponseProto(_socketAccessor.SocketOut.Sequence, true);
    }

    private ResponseProto GetMotionEventsBase(uint window, uint startTime, uint endTime)
    {
        var request = new GetMotionEventsType(window, startTime, endTime);
        _socketAccessor.SocketOut.Send(ref request);
        return new ResponseProto(_socketAccessor.SocketOut.Sequence, true);
    }

    private ResponseProto TranslateCoordinatesBase(uint srcWindow, uint destinationWindow, ushort srcX, ushort srcY)
    {
        var request = new TranslateCoordinatesType(srcWindow, destinationWindow, srcX, srcY);
        _socketAccessor.SocketOut.Send(ref request);
        return new ResponseProto(_socketAccessor.SocketOut.Sequence, true);
    }

    private ResponseProto GetInputFocusBase()
    {
        var request = new GetInputFocusType();
        _socketAccessor.SocketOut.Send(ref request);
        return new ResponseProto(_socketAccessor.SocketOut.Sequence, true);
    }

    private ResponseProto QueryKeymapBase()
    {
        var request = new QueryKeymapType();
        _socketAccessor.SocketOut.Send(ref request);
        return new ResponseProto(_socketAccessor.SocketOut.Sequence, true);
    }

    private ResponseProto QueryFontBase(uint fontId)
    {
        var request = new QueryFontType(fontId);
        _socketAccessor.SocketOut.Send(ref request);
        return new ResponseProto(_socketAccessor.SocketOut.Sequence, true);
    }

    private ResponseProto QueryTextExtentsBase(uint font, ReadOnlySpan<char> stringForQuery)
    {
        var request = new QueryTextExtentsType(font, stringForQuery.Length);
        var requiredBuffer = request.Length * 4;
        if (requiredBuffer < _bigRequestLength)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];

#if NETSTANDARD
            MemoryMarshal.Write(scratchBuffer[0..8], ref request);
#else
            MemoryMarshal.Write(scratchBuffer[..8], in request);
#endif
            Encoding.Unicode.GetBytes(stringForQuery, scratchBuffer[8..(stringForQuery.Length * 2 + 8)]);
            scratchBuffer[(stringForQuery.Length * 2 + 8)..requiredBuffer].Clear();
            _socketAccessor.SocketOut.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);

#if NETSTANDARD
            MemoryMarshal.Write(scratchBuffer[0..8], ref request);
#else
            MemoryMarshal.Write(scratchBuffer[..8], in request);
#endif
            Encoding.Unicode.GetBytes(stringForQuery, scratchBuffer[8..(stringForQuery.Length * 2 + 8)]);
            scratchBuffer[(stringForQuery.Length * 2 + 8)..requiredBuffer].Clear();
            _socketAccessor.SocketOut.SendExact(scratchBuffer[..requiredBuffer]);
        }

        return new ResponseProto(_socketAccessor.SocketOut.Sequence, true);
    }

    private ResponseProto ListFontsBase(ReadOnlySpan<byte> pattern, int maxNames)
    {
        var request = new ListFontsType(pattern.Length, maxNames);
        var requiredBuffer = request.Length * 4;
        if (requiredBuffer < _bigRequestLength)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
            scratchBuffer.WriteRequest(
                ref request,
                8,
                pattern
            );
            _socketAccessor.SocketOut.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            var workingBuffer = scratchBuffer[..requiredBuffer];
            workingBuffer.WriteRequest(
                ref request,
                8,
                pattern
            );
            _socketAccessor.SocketOut.SendExact(workingBuffer);
        }

        return new ResponseProto(_socketAccessor.SocketOut.Sequence, true);
    }

    private ResponseProto ListFontsWithInfoBase(ReadOnlySpan<byte> pattan, int maxNames)
    {
        var request = new ListFontsWithInfoType(pattan.Length, maxNames);
        var requiredBuffer = request.Length * 4;
        if (requiredBuffer < _bigRequestLength)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
            scratchBuffer.WriteRequest(
                ref request,
                8,
                pattan
            );
            _socketAccessor.SocketOut.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            var workingBuffer = scratchBuffer[..requiredBuffer];
            workingBuffer.WriteRequest(
                ref request,
                8,
                pattan
            );
            _socketAccessor.SocketOut.SendExact(workingBuffer);
        }

        return new ResponseProto(_socketAccessor.SocketOut.Sequence, true);
    }

    private ResponseProto GetFontPathBase()
    {
        var request = new GetFontPathType();
        _socketAccessor.SocketOut.Send(ref request);
        return new ResponseProto(_socketAccessor.SocketOut.Sequence, true);
    }

    private ResponseProto GetImageBase(ImageFormat format, uint drawable, ushort x, ushort y, ushort width,
        ushort height, uint planeMask)
    {
        var request = new GetImageType(format, drawable, x, y, width, height, planeMask);
        _socketAccessor.SocketOut.Send(ref request);
        return new ResponseProto(_socketAccessor.SocketOut.Sequence, true);
    }

    private ResponseProto ListInstalledColormapsBase(uint window)
    {
        var request = new ListInstalledColormapsType(window);
        _socketAccessor.SocketOut.Send(ref request);
        return new ResponseProto(_socketAccessor.SocketOut.Sequence, true);
    }

    private ResponseProto AllocNamedColorBase(uint colorMap, ReadOnlySpan<byte> name)
    {
        var request = new AllocNamedColorType(colorMap, name.Length);
        var requiredBuffer = request.Length * 4;
        if (requiredBuffer < _bigRequestLength)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
            scratchBuffer.WriteRequest(
                ref request,
                12,
                name);
            _socketAccessor.SocketOut.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            var workingBuffer = scratchBuffer[..requiredBuffer];
            workingBuffer.WriteRequest(
                ref request,
                12,
                name);
            _socketAccessor.SocketOut.SendExact(workingBuffer);
        }

        return new ResponseProto(_socketAccessor.SocketOut.Sequence, true);
    }

    private ResponseProto AllocColorCellsBase(bool contiguous, uint colorMap, ushort colors, ushort planes)
    {
        var request = new AllocColorCellsType(contiguous, colorMap, colors, planes);
        _socketAccessor.SocketOut.Send(ref request);
        return new ResponseProto(_socketAccessor.SocketOut.Sequence, true);
    }

    private ResponseProto AllocColorPlanesBase(bool contiguous, uint colorMap, ushort colors, ushort reds,
        ushort greens, ushort blues)
    {
        var request = new AllocColorPlanesType(contiguous, colorMap, colors, reds, greens, blues);
        _socketAccessor.SocketOut.Send(ref request);
        return new ResponseProto(_socketAccessor.SocketOut.Sequence, true);
    }

    private ResponseProto QueryColorsBase(uint colorMap, ReadOnlySpan<uint> pixels)
    {
        var request = new QueryColorsType(colorMap, pixels.Length);
        var bigRequest = new QueryColorsBigType(colorMap, pixels.Length);
        SendWithBigRequestIfNeed(
            ref request,
            8,
            request.Length * 4,
            ref bigRequest,
            12,
            bigRequest.Length * 4,
            MemoryMarshal.Cast<uint, byte>(pixels));
        return new ResponseProto(_socketAccessor.SocketOut.Sequence, true);
    }

    private ResponseProto LookupColorBase(uint colorMap, ReadOnlySpan<byte> name)
    {
        var request = new LookupColorType(colorMap, name.Length);
        var requiredBuffer = request.Length * 4;
        if (requiredBuffer < _bigRequestLength)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
            scratchBuffer.WriteRequest(
                ref request,
                12,
                name);
            _socketAccessor.SocketOut.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            var workingBuffer = scratchBuffer[..requiredBuffer];
            workingBuffer.WriteRequest(
                ref request,
                12,
                name);
            _socketAccessor.SocketOut.SendExact(workingBuffer);
        }

        return new ResponseProto(_socketAccessor.SocketOut.Sequence, true);
    }

    private ResponseProto QueryBestSizeBase(QueryShapeOf shape, uint drawable, ushort width, ushort height)
    {
        var request = new QueryBestSizeType(shape, drawable, width, height);
        _socketAccessor.SocketOut.Send(ref request);
        return new ResponseProto(_socketAccessor.SocketOut.Sequence, true);
    }

    private ResponseProto SetModifierMappingBase(ReadOnlySpan<ulong> keycodes)
    {
        var request = new SetModifierMappingType(keycodes.Length);
        var bigRequest = new SetModifierMappingBigType(keycodes.Length);
        SendWithBigRequestIfNeed(
            ref request,
            4,
            request.Length * 4,
            ref bigRequest,
            8,
            bigRequest.Length * 4,
            MemoryMarshal.Cast<ulong, byte>(keycodes));
        return new ResponseProto(_socketAccessor.SocketOut.Sequence, true);
    }

    private ResponseProto GetModifierMappingBase()
    {
        var request = new GetModifierMappingType();
        _socketAccessor.SocketOut.Send(ref request);
        return new ResponseProto(_socketAccessor.SocketOut.Sequence, true);
    }

    private ResponseProto GetKeyboardMappingBase(byte firstKeycode, byte count)
    {
        var request = new GetKeyboardMappingType(firstKeycode, count);
        _socketAccessor.SocketOut.Send(ref request);
        return new ResponseProto(_socketAccessor.SocketOut.Sequence, true);
    }

    private ResponseProto GetKeyboardControlBase()
    {
        var request = new GetKeyboardControlType();
        _socketAccessor.SocketOut.Send(ref request);
        return new ResponseProto(_socketAccessor.SocketOut.Sequence, true);
    }

    private ResponseProto SetPointerMappingBase(ReadOnlySpan<byte> maps)
    {
        var request = new SetPointerMappingType(maps.Length);
        var bigRequest = new SetPointerMappingBigType(maps.Length);
        SendWithBigRequestIfNeed(
            ref request,
            4,
            request.Length * 4,
            ref bigRequest,
            8,
            bigRequest.Length * 4,
            maps);
        return new ResponseProto(_socketAccessor.SocketOut.Sequence, true);
    }

    private ResponseProto GetPointerMappingBase()
    {
        var request = new GetPointerMappingType();
        _socketAccessor.SocketOut.Send(ref request);
        return new ResponseProto(_socketAccessor.SocketOut.Sequence, true);
    }

    private ResponseProto GetPointerControlBase()
    {
        var request = new GetPointerControlType();
        _socketAccessor.SocketOut.Send(ref request);
        return new ResponseProto(_socketAccessor.SocketOut.Sequence, true);
    }

    private ResponseProto GetScreenSaverBase()
    {
        var request = new GetScreenSaverType();
        _socketAccessor.SocketOut.Send(ref request);
        return new ResponseProto(_socketAccessor.SocketOut.Sequence, true);
    }

    private ResponseProto ListHostsBase()
    {
        var request = new ListHostsType();
        _socketAccessor.SocketOut.Send(ref request);
        return new ResponseProto(_socketAccessor.SocketOut.Sequence, true);
    }

    const int _minStackSupport = 512;

    private void SendWithBigRequestIfNeed<TS, TB>(
        ref TS request,
        int requestLength,
        int requestSize,
        ref TB bigRequest,
        int bigRequestLength,
        uint bigRequestSize,
        ReadOnlySpan<byte> data) where TS : unmanaged where TB : unmanaged
    {
        if (bigRequestSize < ushort.MaxValue)
        {
            if (requestSize < _minStackSupport)
            {
                Span<byte> scratchBuffer = stackalloc byte[requestSize];
                scratchBuffer.WriteRequest(
                    ref request,
                    requestLength,
                    data);
                _socketAccessor.SocketOut.SendExact(scratchBuffer);
            }
            else
            {
                using var scratchBuffer = new ArrayPoolUsing<byte>(requestSize);
                var workingBuffer = scratchBuffer[..requestSize];
                workingBuffer.WriteRequest(
                    ref request,
                    requestLength,
                    data);
                _socketAccessor.SocketOut.SendExact(workingBuffer);
            }
        }
        else
        {
            EnableBigRequestIfNeeded();
            if (_bigRequestLength > bigRequestSize)
            {
                using var scratchBuffer = new ArrayPoolUsing<byte>((int)bigRequestSize);
                var workingBuffer = scratchBuffer[..(int)bigRequestSize];
                workingBuffer.WriteRequest(
                    ref bigRequest,
                    bigRequestLength,
                    data);
                _socketAccessor.SocketOut.SendExact(workingBuffer);
            }
            else
            {
                throw new NotImplementedException("todo send in chunks");
            }
        }
    }

    private void EnableBigRequestIfNeeded()
    {
        if (_extensionInternal.IsExtensionEnable(BigRequestExtension.ExtensionName)) return;
        var request = _extensionInternal.BigRequest()
                      ?? throw new InvalidOperationException("BigRequest is not supported");
        var response = request.BigRequestsEnable();
        _bigRequestLength = response.MaximumRequestLength;
    }
}