using System.Net.Sockets;
using System.Runtime.CompilerServices;
using Xcsb.Models.String;
using Xcsb.Masks;
using Xcsb.Models;
using Xcsb.Models.Handshake;
using Xcsb.Models.Infrastructure;
using Xcsb.Models.Infrastructure.Exceptions;
using Xcsb.Models.Infrastructure.Response;
using Xcsb.Response.Contract;
using Xcsb.Response.Event;
using Xcsb.Response.Replies;
using Xcsb.Response.Replies.Internals;
#if !NETSTANDARD
using System.Numerics;
#endif

namespace Xcsb;

#if !NETSTANDARD
[SkipLocalsInit]
#endif
internal sealed class XProto : BaseProtoClient, IXProto
{
    private int _globalId;
    private XBufferProto? _xBufferProto;
    public IXBufferProto BufferClient => _xBufferProto ??= new XBufferProto(this);

    public HandshakeSuccessResponseBody HandshakeSuccessResponseBody { get; }

    public XProto(
        HandshakeSuccessResponseBody handshakeSuccessResponseBody, 
        ClientConnectionContext connectionResult) 
        : base(connectionResult)
    {
        _globalId = 0;
        HandshakeSuccessResponseBody = handshakeSuccessResponseBody;
    }


    public AllocColorReply AllocColor(uint colorMap, ushort red, ushort green, ushort blue)
    {
        var cookie = AllocColorBase(colorMap, red, green, blue);
        var (result, error) = base.ClientConnection.ProtoIn.ReceivedResponse<AllocColorReply>(cookie.Id);
        return error.HasValue
            ? throw new XEventException(error.Value)
            : result!.Value;
    }

    public AllocColorCellsReply AllocColorCells(bool contiguous, uint colorMap, ushort colors, ushort planes)
    {
        var cookie = AllocColorCellsBase(contiguous, colorMap, colors, planes);
        var (result, error) = base.ClientConnection.ProtoIn.ReceivedResponseSpan<AllocColorCellsResponse>(cookie.Id);
        return error.HasValue
            ? throw new XEventException(error.Value)
            : new AllocColorCellsReply(result);
    }

    public AllocColorPlanesReply AllocColorPlanes(bool contiguous, uint colorMap, ushort colors, ushort reds,
        ushort greens, ushort blues)
    {
        var cookie = AllocColorPlanesBase(contiguous, colorMap, colors, reds, greens, blues);
        var (result, error) = base.ClientConnection.ProtoIn.ReceivedResponseSpan<AllocColorPlanesResponse>(cookie.Id);
        return error.HasValue
            ? throw new XEventException(error.Value)
            : new AllocColorPlanesReply(result);
    }

    public AllocNamedColorReply AllocNamedColor(uint colorMap, ReadOnlySpan<byte> name)
    {
        var cookie = AllocNamedColorBase(colorMap, name);
        var (result, error) = base.ClientConnection.ProtoIn.ReceivedResponse<AllocNamedColorReply>(cookie.Id);
        return error.HasValue
            ? throw new XEventException(error.Value)
            : result!.Value;
    }


    public GetAtomNameReply GetAtomName(ATOM atom)
    {
        var cookie = GetAtomNameBase(atom);
        var (result, error) = base.ClientConnection.ProtoIn.ReceivedResponseSpan<GetAtomNameResponse>(cookie.Id);
        return error.HasValue
            ? throw new XEventException(error.Value)
            : new GetAtomNameReply(result);
    }

    public InternAtomReply InternAtom(bool onlyIfExist, string atomName)
    {
        var cookie = InternAtomBase(onlyIfExist, atomName);
        var (result, error) = base.ClientConnection.ProtoIn.ReceivedResponse<InternAtomReply>(cookie.Id);
        return error.HasValue
            ? throw new XEventException(error.Value)
            : result!.Value;
    }

    public GetFontPathReply GetFontPath()
    {
        var cookie = GetFontPathBase();
        var (result, error) = base.ClientConnection.ProtoIn.ReceivedResponseSpan<GetFontPathResponse>(cookie.Id);
        return error.HasValue
            ? throw new XEventException(error.Value)
            : new GetFontPathReply(result);
    }

    public GetGeometryReply GetGeometry(uint drawable)
    {
        var cookie = GetGeometryBase(drawable);
        var (result, error) = base.ClientConnection.ProtoIn.ReceivedResponse<GetGeometryReply>(cookie.Id);
        return error.HasValue
            ? throw new XEventException(error.Value)
            : result!.Value;
    }

    public GetImageReply GetImage(ImageFormat format, uint drawable, ushort x, ushort y, ushort width, ushort height,
        uint planeMask)
    {
        var cookie = GetImageBase(format, drawable, x, y, width, height, planeMask);
        var (result, error) = base.ClientConnection.ProtoIn.ReceivedResponseSpan<GetImageResponse>(cookie.Id);
        return error.HasValue
            ? throw new XEventException(error.Value)
            : new GetImageReply(result);
    }

    public GetInputFocusReply GetInputFocus()
    {
        var cookie = GetInputFocusBase();
        var (result, error) = base.ClientConnection.ProtoIn.ReceivedResponse<GetInputFocusReply>(cookie.Id);
        return error.HasValue
            ? throw new XEventException(error.Value)
            : result!.Value;
    }

    public GetKeyboardControlReply GetKeyboardControl()
    {
        var cookie = GetKeyboardControlBase();
        var (result, error) = base.ClientConnection.ProtoIn.ReceivedResponse<GetKeyboardControlResponse>(cookie.Id);
        return error.HasValue
            ? throw new XEventException(error.Value)
            : new GetKeyboardControlReply(result!.Value);
    }

    public GetKeyboardMappingReply GetKeyboardMapping(byte firstKeycode, byte count)
    {
        var cookie = GetKeyboardMappingBase(firstKeycode, count);
        var (result, error) = base.ClientConnection.ProtoIn.ReceivedResponseSpan<GetKeyboardMappingResponse>(cookie.Id);
        return error.HasValue
            ? throw new XEventException(error.Value)
            : new GetKeyboardMappingReply(result, count);
    }

    public GetModifierMappingReply GetModifierMapping()
    {
        var cookie = GetModifierMappingBase();
        var (result, error) = base.ClientConnection.ProtoIn.ReceivedResponseSpan<GetModifierMappingResponse>(cookie.Id);
        return error.HasValue
            ? throw new XEventException(error.Value)
            : new GetModifierMappingReply(result);
    }

    public GetMotionEventsReply GetMotionEvents(uint window, uint startTime, uint endTime)
    {
        var cookie = GetMotionEventsBase(window, startTime, endTime);
        var (result, error) = base.ClientConnection.ProtoIn.ReceivedResponseSpan<GetMotionEventsResponse>(cookie.Id);
        return error.HasValue
            ? throw new XEventException(error.Value)
            : new GetMotionEventsReply(result);
    }

    public GetPointerControlReply GetPointerControl()
    {
        var cookie = GetPointerControlBase();
        var (result, error) = base.ClientConnection.ProtoIn.ReceivedResponse<GetPointerControlReply>(cookie.Id);
        return error.HasValue
            ? throw new XEventException(error.Value)
            : result!.Value;
    }

    public GetPointerMappingReply GetPointerMapping()
    {
        var cookie = GetPointerMappingBase();
        var (result, error) = base.ClientConnection.ProtoIn.ReceivedResponseSpan<GetPointerMappingResponse>(cookie.Id);
        return error.HasValue
            ? throw new XEventException(error.Value)
            : new GetPointerMappingReply(result);
    }

    public GetPropertyReply GetProperty(bool delete, uint window, ATOM property, ATOM type, uint offset, uint length)
    {
        var cookie = GetPropertyBase(delete, window, property, type, offset, length);
        var (result, error) = base.ClientConnection.ProtoIn.ReceivedResponseSpan<GetPropertyResponse>(cookie.Id);
        return error.HasValue
            ? throw new XEventException(error.Value)
            : new GetPropertyReply(result);
    }

    public GetScreenSaverReply GetScreenSaver()
    {
        var cookie = GetScreenSaverBase();
        var (result, error) = base.ClientConnection.ProtoIn.ReceivedResponse<GetScreenSaverReply>(cookie.Id);
        return error.HasValue
            ? throw new XEventException(error.Value)
            : result!.Value;
    }

    public GetSelectionOwnerReply GetSelectionOwner(ATOM atom)
    {
        var cookie = GetSelectionOwnerBase(atom);
        var (result, error) = base.ClientConnection.ProtoIn.ReceivedResponse<GetSelectionOwnerReply>(cookie.Id);
        return error.HasValue
            ? throw new XEventException(error.Value)
            : result!.Value;
    }

    public GetWindowAttributesReply GetWindowAttributes(uint window)
    {
        var cookie = GetWindowAttributesBase(window);
        var (result, error) = base.ClientConnection.ProtoIn.ReceivedResponse<GetWindowAttributesReply>(cookie.Id);
        return error.HasValue
            ? throw new XEventException(error.Value)
            : result!.Value;
    }

    public ListExtensionsReply ListExtensions()
    {
        var cookie = ListExtensionsBase();
        var (result, error) = base.ClientConnection.ProtoIn.ReceivedResponseSpan<ListExtensionsResponse>(cookie.Id);
        return error.HasValue
            ? throw new XEventException(error.Value)
            : new ListExtensionsReply(result);
    }

    public ListFontsReply ListFonts(ReadOnlySpan<byte> pattern, int maxNames)
    {
        var cookie = ListFontsBase(pattern, maxNames);
        var (result, error) = base.ClientConnection.ProtoIn.ReceivedResponseSpan<ListFontsResponse>(cookie.Id);
        return error.HasValue
            ? throw new XEventException(error.Value)
            : new ListFontsReply(result);
    }

    public ListFontsWithInfoReply[] ListFontsWithInfo(ReadOnlySpan<byte> pattan, int maxNames)
    {
        var cookie = ListFontsWithInfoBase(pattan, maxNames);
        var (result, error) = base.ClientConnection.ProtoIn.ReceivedResponseArray(cookie.Id, maxNames);
        return error.HasValue
            ? throw new XEventException(error.Value)
            : result;
    }

    public ListHostsReply ListHosts()
    {
        var cookie = ListHostsBase();
        var (result, error) = base.ClientConnection.ProtoIn.ReceivedResponseSpan<ListHostsResponse>(cookie.Id);
        return error.HasValue
            ? throw new XEventException(error.Value)
            : new ListHostsReply(result);
    }

    public ListInstalledColormapsReply ListInstalledColormaps(uint window)
    {
        var cookie = ListInstalledColormapsBase(window);
        var (result, error) = base.ClientConnection.ProtoIn.ReceivedResponseSpan<ListInstalledColormapsResponse>(cookie.Id);
        return error.HasValue
            ? throw new XEventException(error.Value)
            : new ListInstalledColormapsReply(result);
    }

    public ListPropertiesReply ListProperties(uint window)
    {
        var cookie = ListPropertiesBase(window);
        var (result, error) = base.ClientConnection.ProtoIn.ReceivedResponseSpan<ListPropertiesResponse>(cookie.Id);
        return error.HasValue
            ? throw new XEventException(error.Value)
            : new ListPropertiesReply(result);
    }

    public LookupColorReply LookupColor(uint colorMap, ReadOnlySpan<byte> name)
    {
        var cookie = LookupColorBase(colorMap, name);
        var (result, error) = base.ClientConnection.ProtoIn.ReceivedResponse<LookupColorReply>(cookie.Id);
        return error.HasValue
            ? throw new XEventException(error.Value)
            : result!.Value;
    }

    public QueryBestSizeReply QueryBestSize(QueryShapeOf shape, uint drawable, ushort width, ushort height)
    {
        var cookie = QueryBestSizeBase(shape, drawable, width, height);
        var (result, error) = base.ClientConnection.ProtoIn.ReceivedResponse<QueryBestSizeReply>(cookie.Id);
        return error.HasValue
            ? throw new XEventException(error.Value)
            : result!.Value;
    }

    public QueryColorsReply QueryColors(uint colorMap, Span<uint> pixels)
    {
        var cookie = QueryColorsBase(colorMap, pixels);
        var (result, error) = base.ClientConnection.ProtoIn.ReceivedResponseSpan<QueryColorsResponse>(cookie.Id);
        return error.HasValue
            ? throw new XEventException(error.Value)
            : new QueryColorsReply(result);
    }

    public QueryExtensionReply QueryExtension(ReadOnlySpan<byte> name)
    {
        if (name.Length > ushort.MaxValue)
            throw new ArgumentException($"{nameof(name)} is invalid, {nameof(name)} is too long.");
        var cookie = QueryExtensionBase(name);
        var (result, error) = base.ClientConnection.ProtoIn.ReceivedResponse<QueryExtensionReply>(cookie.Id);
        return error.HasValue
            ? throw new XEventException(error.Value)
            : result!.Value;
    }

    public QueryFontReply QueryFont(uint fontId)
    {
        var cookie = QueryFontBase(fontId);
        var (result, error) = base.ClientConnection.ProtoIn.ReceivedResponseSpan<QueryFontResponse>(cookie.Id);
        return error.HasValue
            ? throw new XEventException(error.Value)
            : new QueryFontReply(result);
    }

    public QueryKeymapReply QueryKeymap()
    {
        var cookie = QueryKeymapBase();
        var (result, error) = base.ClientConnection.ProtoIn.ReceivedResponse<QueryKeymapResponse>(cookie.Id);
        return error.HasValue
            ? throw new XEventException(error.Value)
            : new QueryKeymapReply(result!.Value);
    }

    public QueryPointerReply QueryPointer(uint window)
    {
        var cookie = QueryPointerBase(window);
        var (result, error) = base.ClientConnection.ProtoIn.ReceivedResponse<QueryPointerReply>(cookie.Id);
        return error.HasValue
            ? throw new XEventException(error.Value)
            : result!.Value;
    }

    public QueryTextExtentsReply QueryTextExtents(uint font, ReadOnlySpan<char> stringForQuery)
    {
        var cookie = QueryTextExtentsBase(font, stringForQuery);
        var (result, error) = base.ClientConnection.ProtoIn.ReceivedResponse<QueryTextExtentsReply>(cookie.Id);
        return error.HasValue
            ? throw new XEventException(error.Value)
            : result!.Value;
    }

    public QueryTreeReply QueryTree(uint window)
    {
        var cookie = QueryTreeBase(window);
        var (result, error) = base.ClientConnection.ProtoIn.ReceivedResponseSpan<QueryTreeResponse>(cookie.Id);
        return error.HasValue
            ? throw new XEventException(error.Value)
            : new QueryTreeReply(result);
    }


    public GrabKeyboardReply GrabKeyboard(bool ownerEvents, uint grabWindow, uint timeStamp, GrabMode pointerMode,
        GrabMode keyboardMode)
    {
        var cookie = GrabKeyboardBase(ownerEvents, grabWindow, timeStamp, pointerMode, keyboardMode);
        var (result, error) = base.ClientConnection.ProtoIn.ReceivedResponse<GrabKeyboardReply>(cookie.Id);
        return error.HasValue
            ? throw new XEventException(error.Value)
            : result!.Value;
    }

    public GrabPointerReply GrabPointer(bool ownerEvents, uint grabWindow, ushort mask, GrabMode pointerMode,
        GrabMode keyboardMode, uint confineTo, uint cursor, uint timeStamp)
    {
        var cookie = GrabPointerBase(ownerEvents, grabWindow, mask, pointerMode, keyboardMode, confineTo, cursor,
            timeStamp);
        var (result, error) = base.ClientConnection.ProtoIn.ReceivedResponse<GrabPointerReply>(cookie.Id);
        return error.HasValue
            ? throw new XEventException(error.Value)
            : result!.Value;
    }

    public SetModifierMappingReply SetModifierMapping(Span<ulong> keycodes)
    {
        var cookie = SetModifierMappingBase(keycodes);
        var (result, error) = base.ClientConnection.ProtoIn.ReceivedResponse<SetModifierMappingReply>(cookie.Id);
        return error.HasValue
            ? throw new XEventException(error.Value)
            : result!.Value;
    }

    public SetPointerMappingReply SetPointerMapping(Span<byte> maps)
    {
        var cookie = SetPointerMappingBase(maps);
        var (result, error) = base.ClientConnection.ProtoIn.ReceivedResponse<SetPointerMappingReply>(cookie.Id);
        return error.HasValue
            ? throw new XEventException(error.Value)
            : result!.Value;
    }

    public TranslateCoordinatesReply TranslateCoordinates(uint srcWindow, uint destinationWindow, ushort srcX,
        ushort srcY)
    {
        var cookie = TranslateCoordinatesBase(srcWindow, destinationWindow, srcX, srcY);
        var (result, error) = base.ClientConnection.ProtoIn.ReceivedResponse<TranslateCoordinatesReply>(cookie.Id);
        return error.HasValue
            ? throw new XEventException(error.Value)
            : result!.Value;
    }


    public void WaitForEvent()
    {
        if (!IsEventAvailable())
            base.ClientConnection.Socket.Poll(-1, SelectMode.SelectRead);
    }

    public uint NewId() =>
        (uint)((HandshakeSuccessResponseBody.ResourceIDMask & _globalId++) |
               HandshakeSuccessResponseBody.ResourceIDBase);

    public XEvent GetEvent() =>
        base.ClientConnection.ProtoIn.ReceivedResponse();

    public bool IsEventAvailable() =>
        !base.ClientConnection.ProtoIn.BufferEvents.IsEmpty || base.ClientConnection.Socket.Available >= Unsafe.SizeOf<GenericEvent>();

    public ResponseProto CreateWindow(byte depth, uint window, uint parent, short x, short y, ushort width,
        ushort height, ushort borderWidth, ClassType classType, uint rootVisualId, ValueMask mask, Span<uint> args) =>
        CreateWindowBase(depth, window, parent, x, y, width, height, borderWidth, classType, rootVisualId, mask, args);

    public ResponseProto ChangeWindowAttributes(uint window, ValueMask mask, Span<uint> args) =>
        ChangeWindowAttributesBase(window, mask, args);

    public ResponseProto DestroyWindow(uint window) =>
        DestroyWindowBase(window);

    public ResponseProto DestroySubwindows(uint window) =>
        DestroySubwindowsBase(window);

    public ResponseProto ChangeSaveSet(ChangeSaveSetMode changeSaveSetMode, uint window) =>
        ChangeSaveSetBase(changeSaveSetMode, window);

    public ResponseProto ReparentWindow(uint window, uint parent, short x, short y) =>
        ReparentWindowBase(window, parent, x, y);

    public ResponseProto MapWindow(uint window) =>
        MapWindowBase(window);

    public ResponseProto MapSubwindows(uint window) =>
        MapSubwindowsBase(window);

    public ResponseProto UnmapWindow(uint window) =>
        UnmapWindowBase(window);

    public ResponseProto UnmapSubwindows(uint window) =>
        UnmapSubwindowsBase(window);

    public ResponseProto ConfigureWindow(uint window, ConfigureValueMask mask, Span<uint> args) =>
        ConfigureWindowBase(window, mask, args);

    public ResponseProto CirculateWindow(Circulate circulate, uint window) =>
        CirculateWindowBase(circulate, window);

    public ResponseProto ChangeProperty<T>(PropertyMode mode, uint window, ATOM property, ATOM type, Span<T> args)
        where T : struct
#if !NETSTANDARD
        , INumber<T>
#endif
        => ChangePropertyBase(mode, window, property, type, args);

    public ResponseProto DeleteProperty(uint window, ATOM atom) =>
        DeletePropertyBase(window, atom);

    public ResponseProto RotateProperties(uint window, ushort delta, Span<ATOM> properties) =>
        RotatePropertiesBase(window, delta, properties);

    public ResponseProto SetSelectionOwner(uint owner, ATOM atom, uint timestamp) =>
        SetSelectionOwnerBase(owner, atom, timestamp);

    public ResponseProto ConvertSelection(uint requestor, ATOM selection, ATOM target, ATOM property, uint timestamp) =>
        ConvertSelectionBase(requestor, selection, target, property, timestamp);

    public ResponseProto SendEvent(bool propagate, uint destination, uint eventMask, XEvent evnt) =>
        SendEventBase(propagate, destination, eventMask, evnt);

    public ResponseProto UngrabPointer(uint time) =>
        UngrabPointerBase(time);

    public ResponseProto GrabButton(bool ownerEvents, uint grabWindow, ushort mask, GrabMode pointerMode,
        GrabMode keyboardMode, uint confineTo, uint cursor, Button button, ModifierMask modifiers) =>
        GrabButtonBase(ownerEvents, grabWindow, mask, pointerMode, keyboardMode, confineTo, cursor, button, modifiers);

    public ResponseProto UngrabButton(Button button, uint grabWindow, ModifierMask mask) =>
        UngrabButtonBase(button, grabWindow, mask);

    public ResponseProto ChangeActivePointerGrab(uint cursor, uint time, ushort mask) =>
        ChangeActivePointerGrabBase(cursor, time, mask);

    public ResponseProto UngrabKeyboard(uint time) =>
        UngrabKeyboardBase(time);

    public ResponseProto GrabKey(bool exposures, uint grabWindow, ModifierMask mask, byte keycode, GrabMode pointerMode,
        GrabMode keyboardMode) => GrabKeyBase(exposures, grabWindow, mask, keycode, pointerMode, keyboardMode);

    public ResponseProto UngrabKey(byte key, uint grabWindow, ModifierMask modifier) =>
        UngrabKeyBase(key, grabWindow, modifier);

    public ResponseProto AllowEvents(EventsMode mode, uint time) =>
        AllowEventsBase(mode, time);

    public ResponseProto GrabServer() =>
        GrabServerBase();

    public ResponseProto UngrabServer() =>
        UngrabServerBase();

    public ResponseProto WarpPointer(uint srcWindow, uint destinationWindow, short srcX, short srcY, ushort srcWidth,
        ushort srcHeight, short destinationX, short destinationY) =>
        WarpPointerBase(srcWindow, destinationWindow, srcX, srcY, srcWidth, srcHeight, destinationX, destinationY);

    public ResponseProto SetInputFocus(InputFocusMode mode, uint focus, uint time) =>
        SetInputFocusBase(mode, focus, time);

    public ResponseProto OpenFont(string fontName, uint fontId) =>
        OpenFontBase(fontName, fontId);

    public ResponseProto CloseFont(uint fontId) =>
        CloseFontBase(fontId);

    public ResponseProto SetFontPath(string[] strPaths) =>
        SetFontPathBase(strPaths);

    public ResponseProto CreatePixmap(byte depth, uint pixmapId, uint drawable, ushort width, ushort height) =>
        CreatePixmapBase(depth, pixmapId, drawable, width, height);

    public ResponseProto FreePixmap(uint pixmapId) =>
        FreePixmapBase(pixmapId);

    public ResponseProto CreateGC(uint gc, uint drawable, GCMask mask, Span<uint> args) =>
        CreateGCBase(gc, drawable, mask, args);

    public ResponseProto ChangeGC(uint gc, GCMask mask, Span<uint> args) =>
        ChangeGCBase(gc, mask, args);

    public ResponseProto CopyGC(uint srcGc, uint dstGc, GCMask mask) =>
        CopyGCBase(srcGc, dstGc, mask);

    public ResponseProto SetDashes(uint gc, ushort dashOffset, Span<byte> dashes) =>
        SetDashesBase(gc, dashOffset, dashes);

    public ResponseProto SetClipRectangles(ClipOrdering ordering, uint gc, ushort clipX, ushort clipY,
        Span<Rectangle> rectangles) =>
        SetClipRectanglesBase(ordering, gc, clipX, clipY, rectangles);

    public ResponseProto FreeGC(uint gc) =>
        FreeGCBase(gc);

    public ResponseProto ClearArea(bool exposures, uint window, short x, short y, ushort width, ushort height) =>
        ClearAreaBase(exposures, window, x, y, width, height);

    public ResponseProto CopyArea(uint srcDrawable, uint destinationDrawable, uint gc, ushort srcX, ushort srcY,
        ushort destinationX, ushort destinationY, ushort width, ushort height) =>
        CopyAreaBase(srcDrawable, destinationDrawable, gc, srcX, srcY, destinationX, destinationY, width, height);

    public ResponseProto CopyPlane(uint srcDrawable, uint destinationDrawable, uint gc, ushort srcX, ushort srcY,
        ushort destinationX, ushort destinationY, ushort width, ushort height, uint bitPlane) =>
        CopyPlaneBase(srcDrawable, destinationDrawable, gc, srcX, srcY, destinationX, destinationY, width, height,
            bitPlane);

    public ResponseProto PolyPoint(CoordinateMode coordinate, uint drawable, uint gc, Span<Point> points) =>
        PolyPointBase(coordinate, drawable, gc, points);

    public ResponseProto PolyLine(CoordinateMode coordinate, uint drawable, uint gc, Span<Point> points) =>
        PolyLineBase(coordinate, drawable, gc, points);

    public ResponseProto PolySegment(uint drawable, uint gc, Span<Segment> segments) =>
        PolySegmentBase(drawable, gc, segments);

    public ResponseProto PolyRectangle(uint drawable, uint gc, Span<Rectangle> rectangles) =>
        PolyRectangleBase(drawable, gc, rectangles);

    public ResponseProto PolyArc(uint drawable, uint gc, Span<Arc> arcs) =>
        PolyArcBase(drawable, gc, arcs);

    public ResponseProto FillPoly(uint drawable, uint gc, PolyShape shape, CoordinateMode coordinate,
        Span<Point> points) =>
        FillPolyBase(drawable, gc, shape, coordinate, points);

    public ResponseProto PolyFillRectangle(uint drawable, uint gc, Span<Rectangle> rectangles) =>
        PolyFillRectangleBase(drawable, gc, rectangles);

    public ResponseProto PolyFillArc(uint drawable, uint gc, Span<Arc> arcs) =>
        PolyFillArcBase(drawable, gc, arcs);

    public ResponseProto PutImage(ImageFormatBitmap format, uint drawable, uint gc, ushort width, ushort height,
        short x, short y, byte leftPad, byte depth, Span<byte> data) =>
        PutImageBase(format, drawable, gc, width, height, x, y, leftPad, depth, data);

    public ResponseProto ImageText8(uint drawable, uint gc, short x, short y, ReadOnlySpan<byte> text) =>
        ImageText8Base(drawable, gc, x, y, text);

    public ResponseProto ImageText16(uint drawable, uint gc, short x, short y, ReadOnlySpan<char> text) =>
        ImageText16Base(drawable, gc, x, y, text);

    public ResponseProto CreateColormap(ColormapAlloc alloc, uint colormapId, uint window, uint visual) =>
        CreateColormapBase(alloc, colormapId, window, visual);

    public ResponseProto FreeColormap(uint colormapId) =>
        FreeColormapBase(colormapId);

    public ResponseProto CopyColormapAndFree(uint colormapId, uint srcColormapId) =>
        CopyColormapAndFreeBase(colormapId, srcColormapId);

    public ResponseProto InstallColormap(uint colormapId) =>
        InstallColormapBase(colormapId);

    public ResponseProto UninstallColormap(uint colormapId) =>
        UninstallColormapBase(colormapId);

    public ResponseProto FreeColors(uint colormapId, uint planeMask, Span<uint> pixels) =>
        FreeColorsBase(colormapId, planeMask, pixels);

    public ResponseProto StoreColors(uint colormapId, Span<ColorItem> item) =>
        StoreColorsBase(colormapId, item);

    public ResponseProto StoreNamedColor(ColorFlag mode, uint colormapId, uint pixels, ReadOnlySpan<byte> name) =>
        StoreNamedColorBase(mode, colormapId, pixels, name);

    public ResponseProto CreateCursor(uint cursorId, uint source, uint mask, ushort foreRed, ushort foreGreen,
        ushort foreBlue, ushort backRed, ushort backGreen, ushort backBlue, ushort x, ushort y) =>
        CreateCursorBase(cursorId, source, mask, foreRed, foreGreen, foreBlue, backRed, backGreen, backBlue, x, y);

    public ResponseProto CreateGlyphCursor(uint cursorId, uint sourceFont, uint fontMask, char sourceChar,
        ushort charMask, ushort foreRed, ushort foreGreen, ushort foreBlue, ushort backRed, ushort backGreen,
        ushort backBlue) =>
        CreateGlyphCursorBase(cursorId, sourceFont, fontMask, sourceChar, charMask, foreRed, foreGreen, foreBlue,
            backRed, backGreen, backBlue);

    public ResponseProto FreeCursor(uint cursorId) =>
        FreeCursorBase(cursorId);

    public ResponseProto RecolorCursor(uint cursorId, ushort foreRed, ushort foreGreen, ushort foreBlue, ushort backRed,
        ushort backGreen, ushort backBlue) =>
        RecolorCursorBase(cursorId, foreRed, foreGreen, foreBlue, backRed, backGreen, backBlue);

    public ResponseProto ChangeKeyboardMapping(byte keycodeCount, byte firstKeycode, byte keysymsPerKeycode,
        Span<uint> keysym) =>
        ChangeKeyboardMappingBase(keycodeCount, firstKeycode, keysymsPerKeycode, keysym);

    public ResponseProto Bell(sbyte percent) =>
        BellBase(percent);

    public ResponseProto ChangeKeyboardControl(KeyboardControlMask mask, Span<uint> args) =>
        ChangeKeyboardControlBase(mask, args);

    public ResponseProto ChangePointerControl(Acceleration? acceleration, ushort? threshold) =>
        ChangePointerControlBase(acceleration, threshold);

    public ResponseProto SetScreenSaver(short timeout, short interval, TriState preferBlanking,
        TriState allowExposures) =>
        SetScreenSaverBase(timeout, interval, preferBlanking, allowExposures);

    public ResponseProto ForceScreenSaver(ForceScreenSaverMode mode) =>
        ForceScreenSaverBase(mode);

    public ResponseProto ChangeHosts(HostMode mode, Family family, Span<byte> address) =>
        ChangeHostsBase(mode, family, address);

    public ResponseProto SetAccessControl(AccessControlMode mode) =>
        SetAccessControlBase(mode);

    public ResponseProto SetCloseDownMode(CloseDownMode mode) =>
        SetCloseDownModeBase(mode);

    public ResponseProto KillClient(uint resource) =>
        KillClientBase(resource);

    public ResponseProto NoOperation(Span<uint> args) =>
        NoOperationBase(args);

    public ResponseProto PolyText8(uint drawable, uint gc, ushort x, ushort y, TextItem8[] data) =>
        PolyText8Base(drawable, gc, x, y, data);

    public ResponseProto PolyText16(uint drawable, uint gc, ushort x, ushort y, TextItem16[] data) =>
        PolyText16Base(drawable, gc, x, y, data);

    public void CreateWindowUnchecked(byte depth, uint window, uint parent, short x, short y, ushort width,
        ushort height, ushort borderWidth, ClassType classType, uint rootVisualId, ValueMask mask, Span<uint> args)
    {
        var cookie = this.CreateWindowBase(depth, window, parent, x, y, width, height, borderWidth, classType,
            rootVisualId, mask, args);
        base.ClientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, false);
    }

    public void ChangeWindowAttributesUnchecked(uint window, ValueMask mask, Span<uint> args)
    {
        var cookie = this.ChangeWindowAttributesBase(window, mask, args);
        base.ClientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, false);
    }

    public void DestroyWindowUnchecked(uint window)
    {
        var cookie = this.DestroyWindowBase(window);
        base.ClientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, false);
    }

    public void DestroySubwindowsUnchecked(uint window)
    {
        var cookie = this.DestroySubwindowsBase(window);
        base.ClientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, false);
    }

    public void ChangeSaveSetUnchecked(ChangeSaveSetMode changeSaveSetMode, uint window)
    {
        var cookie = this.ChangeSaveSetBase(changeSaveSetMode, window);
        base.ClientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, false);
    }

    public void ReparentWindowUnchecked(uint window, uint parent, short x, short y)
    {
        var cookie = this.ReparentWindowBase(window, parent, x, y);
        base.ClientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, false);
    }

    public void MapWindowUnchecked(uint window)
    {
        var cookie = this.MapWindowBase(window);
        base.ClientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, false);
    }

    public void MapSubwindowsUnchecked(uint window)
    {
        var cookie = this.MapSubwindowsBase(window);
        base.ClientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, false);
    }

    public void UnmapWindowUnchecked(uint window)
    {
        var cookie = this.UnmapWindowBase(window);
        base.ClientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, false);
    }

    public void UnmapSubwindowsUnchecked(uint window)
    {
        var cookie = this.UnmapSubwindowsBase(window);
        base.ClientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, false);
    }

    public void ConfigureWindowUnchecked(uint window, ConfigureValueMask mask, Span<uint> args)
    {
        var cookie = this.ConfigureWindowBase(window, mask, args);
        base.ClientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, false);
    }

    public void CirculateWindowUnchecked(Circulate circulate, uint window)
    {
        var cookie = this.CirculateWindowBase(circulate, window);
        base.ClientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, false);
    }

    public void ChangePropertyUnchecked<T>(PropertyMode mode, uint window, ATOM property, ATOM type, Span<T> args)
        where T : struct
#if !NETSTANDARD
        , INumber<T>
#endif
    {
        var cookie = this.ChangePropertyBase(mode, window, property, type, args);
        base.ClientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, false);
    }

    public void DeletePropertyUnchecked(uint window, ATOM atom)
    {
        var cookie = this.DeletePropertyBase(window, atom);
        base.ClientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, false);
    }

    public void RotatePropertiesUnchecked(uint window, ushort delta, Span<ATOM> properties)
    {
        var cookie = this.RotatePropertiesBase(window, delta, properties);
        base.ClientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, false);
    }

    public void SetSelectionOwnerUnchecked(uint owner, ATOM atom, uint timestamp)
    {
        var cookie = this.SetSelectionOwnerBase(owner, atom, timestamp);
        base.ClientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, false);
    }

    public void ConvertSelectionUnchecked(uint requestor, ATOM selection, ATOM target, ATOM property, uint timestamp)
    {
        var cookie = this.ConvertSelectionBase(requestor, selection, target, property, timestamp);
        base.ClientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, false);
    }

    public void SendEventUnchecked(bool propagate, uint destination, uint eventMask, XEvent evnt)
    {
        var cookie = this.SendEventBase(propagate, destination, eventMask, evnt);
        base.ClientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, false);
    }

    public void UngrabPointerUnchecked(uint time)
    {
        var cookie = this.UngrabPointerBase(time);
        base.ClientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, false);
    }

    public void GrabButtonUnchecked(bool ownerEvents, uint grabWindow, ushort mask, GrabMode pointerMode,
        GrabMode keyboardMode, uint confineTo, uint cursor, Button button, ModifierMask modifiers)
    {
        var cookie = this.GrabButtonBase(ownerEvents, grabWindow, mask, pointerMode, keyboardMode, confineTo, cursor,
            button, modifiers);
        base.ClientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, false);
    }

    public void UngrabButtonUnchecked(Button button, uint grabWindow, ModifierMask mask)
    {
        var cookie = this.UngrabButtonBase(button, grabWindow, mask);
        base.ClientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, false);
    }

    public void ChangeActivePointerGrabUnchecked(uint cursor, uint time, ushort mask)
    {
        var cookie = this.ChangeActivePointerGrabBase(cursor, time, mask);
        base.ClientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, false);
    }

    public void UngrabKeyboardUnchecked(uint time)
    {
        var cookie = this.UngrabKeyboardBase(time);
        base.ClientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, false);
    }

    public void GrabKeyUnchecked(bool exposures, uint grabWindow, ModifierMask mask, byte keycode, GrabMode pointerMode,
        GrabMode keyboardMode)
    {
        var cookie = this.GrabKeyBase(exposures, grabWindow, mask, keycode, pointerMode, keyboardMode);
        base.ClientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, false);
    }

    public void UngrabKeyUnchecked(byte key, uint grabWindow, ModifierMask modifier)
    {
        var cookie = this.UngrabKeyBase(key, grabWindow, modifier);
        base.ClientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, false);
    }

    public void AllowEventsUnchecked(EventsMode mode, uint time)
    {
        var cookie = this.AllowEventsBase(mode, time);
        base.ClientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, false);
    }

    public void GrabServerUnchecked()
    {
        var cookie = this.GrabServerBase();
        base.ClientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, false);
    }

    public void UngrabServerUnchecked()
    {
        var cookie = this.UngrabServerBase();
        base.ClientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, false);
    }

    public void WarpPointerUnchecked(uint srcWindow, uint destinationWindow, short srcX, short srcY, ushort srcWidth,
        ushort srcHeight, short destinationX, short destinationY)
    {
        var cookie = this.WarpPointerBase(srcWindow, destinationWindow, srcX, srcY, srcWidth, srcHeight, destinationX,
            destinationY);
        base.ClientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, false);
    }

    public void SetInputFocusUnchecked(InputFocusMode mode, uint focus, uint time)
    {
        var cookie = this.SetInputFocusBase(mode, focus, time);
        base.ClientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, false);
    }

    public void OpenFontUnchecked(string fontName, uint fontId)
    {
        var cookie = this.OpenFontBase(fontName, fontId);
        base.ClientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, false);
    }

    public void CloseFontUnchecked(uint fontId)
    {
        var cookie = this.CloseFontBase(fontId);
        base.ClientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, false);
    }

    public void SetFontPathUnchecked(string[] strPaths)
    {
        var cookie = this.SetFontPathBase(strPaths);
        base.ClientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, false);
    }

    public void CreatePixmapUnchecked(byte depth, uint pixmapId, uint drawable, ushort width, ushort height)
    {
        var cookie = this.CreatePixmapBase(depth, pixmapId, drawable, width, height);
        base.ClientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, false);
    }

    public void FreePixmapUnchecked(uint pixmapId)
    {
        var cookie = this.FreePixmapBase(pixmapId);
        base.ClientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, false);
    }

    public void CreateGCUnchecked(uint gc, uint drawable, GCMask mask, Span<uint> args)
    {
        var cookie = this.CreateGCBase(gc, drawable, mask, args);
        base.ClientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, false);
    }

    public void ChangeGCUnchecked(uint gc, GCMask mask, Span<uint> args)
    {
        var cookie = this.ChangeGCBase(gc, mask, args);
        base.ClientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, false);
    }

    public void CopyGCUnchecked(uint srcGc, uint dstGc, GCMask mask)
    {
        var cookie = this.CopyGCBase(srcGc, dstGc, mask);
        base.ClientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, false);
    }

    public void SetDashesUnchecked(uint gc, ushort dashOffset, Span<byte> dashes)
    {
        var cookie = this.SetDashesBase(gc, dashOffset, dashes);
        base.ClientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, false);
    }

    public void SetClipRectanglesUnchecked(ClipOrdering ordering, uint gc, ushort clipX, ushort clipY,
        Span<Rectangle> rectangles)
    {
        var cookie = this.SetClipRectanglesBase(ordering, gc, clipX, clipY, rectangles);
        base.ClientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, false);
    }

    public void FreeGCUnchecked(uint gc)
    {
        var cookie = this.FreeGCBase(gc);
        base.ClientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, false);
    }

    public void ClearAreaUnchecked(bool exposures, uint window, short x, short y, ushort width, ushort height)
    {
        var cookie = this.ClearAreaBase(exposures, window, x, y, width, height);
        base.ClientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, false);
    }

    public void CopyAreaUnchecked(uint srcDrawable, uint destinationDrawable, uint gc, ushort srcX, ushort srcY,
        ushort destinationX, ushort destinationY, ushort width, ushort height)
    {
        var cookie = this.CopyAreaBase(srcDrawable, destinationDrawable, gc, srcX, srcY, destinationX, destinationY,
            width, height);
        base.ClientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, false);
    }

    public void CopyPlaneUnchecked(uint srcDrawable, uint destinationDrawable, uint gc, ushort srcX, ushort srcY,
        ushort destinationX, ushort destinationY, ushort width, ushort height, uint bitPlane)
    {
        var cookie = this.CopyPlaneBase(srcDrawable, destinationDrawable, gc, srcX, srcY, destinationX, destinationY,
            width, height, bitPlane);
        base.ClientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, false);
    }

    public void PolyPointUnchecked(CoordinateMode coordinate, uint drawable, uint gc, Span<Point> points)
    {
        var cookie = this.PolyPointBase(coordinate, drawable, gc, points);
        base.ClientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, false);
    }

    public void PolyLineUnchecked(CoordinateMode coordinate, uint drawable, uint gc, Span<Point> points)
    {
        var cookie = this.PolyLineBase(coordinate, drawable, gc, points);
        base.ClientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, false);
    }

    public void PolySegmentUnchecked(uint drawable, uint gc, Span<Segment> segments)
    {
        var cookie = this.PolySegmentBase(drawable, gc, segments);
        base.ClientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, false);
    }

    public void PolyRectangleUnchecked(uint drawable, uint gc, Span<Rectangle> rectangles)
    {
        var cookie = this.PolyRectangleBase(drawable, gc, rectangles);
        base.ClientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, false);
    }

    public void PolyArcUnchecked(uint drawable, uint gc, Span<Arc> arcs)
    {
        var cookie = this.PolyArcBase(drawable, gc, arcs);
        base.ClientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, false);
    }

    public void FillPolyUnchecked(uint drawable, uint gc, PolyShape shape, CoordinateMode coordinate,
        Span<Point> points)
    {
        var cookie = this.FillPolyBase(drawable, gc, shape, coordinate, points);
        base.ClientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, false);
    }

    public void PolyFillRectangleUnchecked(uint drawable, uint gc, Span<Rectangle> rectangles)
    {
        var cookie = this.PolyFillRectangleBase(drawable, gc, rectangles);
        base.ClientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, false);
    }

    public void PolyFillArcUnchecked(uint drawable, uint gc, Span<Arc> arcs)
    {
        var cookie = this.PolyFillArcBase(drawable, gc, arcs);
        base.ClientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, false);
    }

    public void PutImageUnchecked(ImageFormatBitmap format, uint drawable, uint gc, ushort width, ushort height,
        short x,
        short y, byte leftPad, byte depth, Span<byte> data)
    {
        var cookie = this.PutImageBase(format, drawable, gc, width, height, x, y, leftPad, depth, data);
        base.ClientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, false);
    }

    public void ImageText8Unchecked(uint drawable, uint gc, short x, short y, ReadOnlySpan<byte> text)
    {
        var cookie = this.ImageText8Base(drawable, gc, x, y, text);
        base.ClientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, false);
    }

    public void ImageText16Unchecked(uint drawable, uint gc, short x, short y, ReadOnlySpan<char> text)
    {
        var cookie = this.ImageText16Base(drawable, gc, x, y, text);
        base.ClientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, false);
    }

    public void CreateColormapUnchecked(ColormapAlloc alloc, uint colormapId, uint window, uint visual)
    {
        var cookie = this.CreateColormapBase(alloc, colormapId, window, visual);
        base.ClientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, false);
    }

    public void FreeColormapUnchecked(uint colormapId)
    {
        var cookie = this.FreeColormapBase(colormapId);
        base.ClientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, false);
    }

    public void CopyColormapAndFreeUnchecked(uint colormapId, uint srcColormapId)
    {
        var cookie = this.CopyColormapAndFreeBase(colormapId, srcColormapId);
        base.ClientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, false);
    }

    public void InstallColormapUnchecked(uint colormapId)
    {
        var cookie = this.InstallColormapBase(colormapId);
        base.ClientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, false);
    }

    public void UninstallColormapUnchecked(uint colormapId)
    {
        var cookie = this.UninstallColormapBase(colormapId);
        base.ClientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, false);
    }

    public void FreeColorsUnchecked(uint colormapId, uint planeMask, Span<uint> pixels)
    {
        var cookie = this.FreeColorsBase(colormapId, planeMask, pixels);
        base.ClientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, false);
    }

    public void StoreColorsUnchecked(uint colormapId, Span<ColorItem> item)
    {
        var cookie = this.StoreColorsBase(colormapId, item);
        base.ClientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, false);
    }

    public void StoreNamedColorUnchecked(ColorFlag mode, uint colormapId, uint pixels, ReadOnlySpan<byte> name)
    {
        var cookie = this.StoreNamedColorBase(mode, colormapId, pixels, name);
        base.ClientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, false);
    }

    public void CreateCursorUnchecked(uint cursorId, uint source, uint mask, ushort foreRed, ushort foreGreen,
        ushort foreBlue, ushort backRed, ushort backGreen, ushort backBlue, ushort x, ushort y)
    {
        var cookie = this.CreateCursorBase(cursorId, source, mask, foreRed, foreGreen, foreBlue, backRed,
            backGreen, backBlue, x, y);
        base.ClientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, false);
    }

    public void CreateGlyphCursorUnchecked(uint cursorId, uint sourceFont, uint fontMask, char sourceChar,
        ushort charMask, ushort foreRed, ushort foreGreen, ushort foreBlue, ushort backRed, ushort backGreen,
        ushort backBlue)
    {
        var cookie = this.CreateGlyphCursorBase(cursorId, sourceFont, fontMask, sourceChar, charMask, foreRed,
            foreGreen, foreBlue, backRed, backGreen, backBlue);
        base.ClientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, false);
    }

    public void FreeCursorUnchecked(uint cursorId)
    {
        var cookie = this.FreeCursorBase(cursorId);
        base.ClientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, false);
    }

    public void RecolorCursorUnchecked(uint cursorId, ushort foreRed, ushort foreGreen, ushort foreBlue, ushort backRed,
        ushort backGreen, ushort backBlue)
    {
        var cookie = this.RecolorCursorBase(cursorId, foreRed, foreGreen, foreBlue, backRed, backGreen, backBlue);
        base.ClientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, false);
    }

    public void ChangeKeyboardMappingUnchecked(byte keycodeCount, byte firstKeycode, byte keysymsPerKeycode,
        Span<uint> keysym)
    {
        var cookie = this.ChangeKeyboardMappingBase(keycodeCount, firstKeycode, keysymsPerKeycode, keysym);
        base.ClientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, false);
    }

    public void BellUnchecked(sbyte percent)
    {
        var cookie = this.BellBase(percent);
        base.ClientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, false);
    }

    public void ChangeKeyboardControlUnchecked(KeyboardControlMask mask, Span<uint> args)
    {
        var cookie = this.ChangeKeyboardControlBase(mask, args);
        base.ClientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, false);
    }

    public void ChangePointerControlUnchecked(Acceleration? acceleration, ushort? threshold)
    {
        var cookie = this.ChangePointerControlBase(acceleration, threshold);
        base.ClientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, false);
    }

    public void SetScreenSaverUnchecked(short timeout, short interval, TriState preferBlanking, TriState allowExposures)
    {
        var cookie = this.SetScreenSaverBase(timeout, interval, preferBlanking, allowExposures);
        base.ClientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, false);
    }

    public void ForceScreenSaverUnchecked(ForceScreenSaverMode mode)
    {
        var cookie = this.ForceScreenSaverBase(mode);
        base.ClientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, false);
    }

    public void ChangeHostsUnchecked(HostMode mode, Family family, Span<byte> address)
    {
        var cookie = this.ChangeHostsBase(mode, family, address);
        base.ClientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, false);
    }

    public void SetAccessControlUnchecked(AccessControlMode mode)
    {
        var cookie = this.SetAccessControlBase(mode);
        base.ClientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, false);
    }

    public void SetCloseDownModeUnchecked(CloseDownMode mode)
    {
        var cookie = this.SetCloseDownModeBase(mode);
        base.ClientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, false);
    }

    public void KillClientUnchecked(uint resource)
    {
        var cookie = this.KillClientBase(resource);
        base.ClientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, false);
    }

    public void NoOperationUnchecked(Span<uint> args)
    {
        var cookie = this.NoOperationBase(args);
        base.ClientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, false);
    }

    public void PolyText8Unchecked(uint drawable, uint gc, ushort x, ushort y, TextItem8[] data)
    {
        var cookie = this.PolyText8Base(drawable, gc, x, y, data);
        base.ClientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, false);
    }

    public void PolyText16Unchecked(uint drawable, uint gc, ushort x, ushort y, TextItem16[] data)
    {
        var cookie = this.PolyText16Base(drawable, gc, x, y, data);
        base.ClientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, false);
    }

    public void CreateWindowChecked(byte depth, uint window, uint parent, short x, short y, ushort width, ushort height,
        ushort borderWidth, ClassType classType, uint rootVisualId, ValueMask mask, Span<uint> args)
    {
        var cookie = this.CreateWindowBase(depth, window, parent, x, y, width, height, borderWidth, classType,
            rootVisualId, mask, args);
        base.ClientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, true);
    }

    public void ChangeWindowAttributesChecked(uint window, ValueMask mask, Span<uint> args)
    {
        var cookie = this.ChangeWindowAttributesBase(window, mask, args);
        base.ClientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, true);
    }

    public void DestroyWindowChecked(uint window)
    {
        var cookie = this.DestroyWindowBase(window);
        base.ClientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, true);
    }

    public void DestroySubwindowsChecked(uint window)
    {
        var cookie = this.DestroySubwindowsBase(window);
        base.ClientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, true);
    }

    public void ChangeSaveSetChecked(ChangeSaveSetMode changeSaveSetMode, uint window)
    {
        var cookie = this.ChangeSaveSetBase(changeSaveSetMode, window);
        base.ClientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, true);
    }

    public void ReparentWindowChecked(uint window, uint parent, short x, short y)
    {
        var cookie = this.ReparentWindowBase(window, parent, x, y);
        base.ClientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, true);
    }

    public void MapWindowChecked(uint window)
    {
        var cookie = this.MapWindowBase(window);
        base.ClientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, true);
    }

    public void MapSubwindowsChecked(uint window)
    {
        var cookie = this.MapSubwindowsBase(window);
        base.ClientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, true);
    }

    public void UnmapWindowChecked(uint window)
    {
        var cookie = this.UnmapWindowBase(window);
        base.ClientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, true);
    }

    public void UnmapSubwindowsChecked(uint window)
    {
        var cookie = this.UnmapSubwindowsBase(window);
        base.ClientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, true);
    }

    public void ConfigureWindowChecked(uint window, ConfigureValueMask mask, Span<uint> args)
    {
        var cookie = this.ConfigureWindowBase(window, mask, args);
        base.ClientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, true);
    }

    public void CirculateWindowChecked(Circulate circulate, uint window)
    {
        var cookie = this.CirculateWindowBase(circulate, window);
        base.ClientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, true);
    }

    public void ChangePropertyChecked<T>(PropertyMode mode, uint window, ATOM property, ATOM type, Span<T> args)
        where T : struct
#if !NETSTANDARD
        , INumber<T>
#endif
    {
        var cookie = this.ChangePropertyBase(mode, window, property, type, args);
        base.ClientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, true);
    }

    public void DeletePropertyChecked(uint window, ATOM atom)
    {
        var cookie = this.DeletePropertyBase(window, atom);
        base.ClientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, true);
    }

    public void RotatePropertiesChecked(uint window, ushort delta, Span<ATOM> properties)
    {
        var cookie = this.RotatePropertiesBase(window, delta, properties);
        base.ClientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, true);
    }

    public void SetSelectionOwnerChecked(uint owner, ATOM atom, uint timestamp)
    {
        var cookie = this.SetSelectionOwnerBase(owner, atom, timestamp);
        base.ClientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, true);
    }

    public void ConvertSelectionChecked(uint requestor, ATOM selection, ATOM target, ATOM property, uint timestamp)
    {
        var cookie = this.ConvertSelectionBase(requestor, selection, target, property, timestamp);
        base.ClientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, true);
    }

    public void SendEventChecked(bool propagate, uint destination, uint eventMask, XEvent evnt)
    {
        var cookie = this.SendEventBase(propagate, destination, eventMask, evnt);
        base.ClientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, true);
    }

    public void UngrabPointerChecked(uint time)
    {
        var cookie = this.UngrabPointerBase(time);
        base.ClientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, true);
    }

    public void GrabButtonChecked(bool ownerEvents, uint grabWindow, ushort mask, GrabMode pointerMode,
        GrabMode keyboardMode, uint confineTo, uint cursor, Button button, ModifierMask modifiers)
    {
        var cookie = this.GrabButtonBase(ownerEvents, grabWindow, mask, pointerMode, keyboardMode, confineTo, cursor,
            button, modifiers);
        base.ClientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, true);
    }

    public void UngrabButtonChecked(Button button, uint grabWindow, ModifierMask mask)
    {
        var cookie = this.UngrabButtonBase(button, grabWindow, mask);
        base.ClientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, true);
    }

    public void ChangeActivePointerGrabChecked(uint cursor, uint time, ushort mask)
    {
        var cookie = this.ChangeActivePointerGrabBase(cursor, time, mask);
        base.ClientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, true);
    }

    public void UngrabKeyboardChecked(uint time)
    {
        var cookie = this.UngrabKeyboardBase(time);
        base.ClientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, true);
    }

    public void GrabKeyChecked(bool exposures, uint grabWindow, ModifierMask mask, byte keycode, GrabMode pointerMode,
        GrabMode keyboardMode)
    {
        var cookie = this.GrabKeyBase(exposures, grabWindow, mask, keycode, pointerMode, keyboardMode);
        base.ClientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, true);
    }

    public void UngrabKeyChecked(byte key, uint grabWindow, ModifierMask modifier)
    {
        var cookie = this.UngrabKeyBase(key, grabWindow, modifier);
        base.ClientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, true);
    }

    public void AllowEventsChecked(EventsMode mode, uint time)
    {
        var cookie = this.AllowEventsBase(mode, time);
        base.ClientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, true);
    }

    public void GrabServerChecked()
    {
        var cookie = this.GrabServerBase();
        base.ClientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, true);
    }

    public void UngrabServerChecked()
    {
        var cookie = this.UngrabServerBase();
        base.ClientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, true);
    }

    public void WarpPointerChecked(uint srcWindow, uint destinationWindow, short srcX, short srcY, ushort srcWidth,
        ushort srcHeight, short destinationX, short destinationY)
    {
        var cookie = this.WarpPointerBase(srcWindow, destinationWindow, srcX, srcY, srcWidth, srcHeight, destinationX,
            destinationY);
        base.ClientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, true);
    }

    public void SetInputFocusChecked(InputFocusMode mode, uint focus, uint time)
    {
        var cookie = this.SetInputFocusBase(mode, focus, time);
        base.ClientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, true);
    }

    public void OpenFontChecked(string fontName, uint fontId)
    {
        var cookie = this.OpenFontBase(fontName, fontId);
        base.ClientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, true);
    }

    public void CloseFontChecked(uint fontId)
    {
        var cookie = this.CloseFontBase(fontId);
        base.ClientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, true);
    }

    public void SetFontPathChecked(string[] strPaths)
    {
        var cookie = this.SetFontPathBase(strPaths);
        base.ClientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, true);
    }

    public void CreatePixmapChecked(byte depth, uint pixmapId, uint drawable, ushort width, ushort height)
    {
        var cookie = this.CreatePixmapBase(depth, pixmapId, drawable, width, height);
        base.ClientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, true);
    }

    public void FreePixmapChecked(uint pixmapId)
    {
        var cookie = this.FreePixmapBase(pixmapId);
        base.ClientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, true);
    }

    public void CreateGCChecked(uint gc, uint drawable, GCMask mask, Span<uint> args)
    {
        var cookie = this.CreateGCBase(gc, drawable, mask, args);
        base.ClientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, true);
    }

    public void ChangeGCChecked(uint gc, GCMask mask, Span<uint> args)
    {
        var cookie = this.ChangeGCBase(gc, mask, args);
        base.ClientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, true);
    }

    public void CopyGCChecked(uint srcGc, uint dstGc, GCMask mask)
    {
        var cookie = this.CopyGCBase(srcGc, dstGc, mask);
        base.ClientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, true);
    }

    public void SetDashesChecked(uint gc, ushort dashOffset, Span<byte> dashes)
    {
        var cookie = this.SetDashesBase(gc, dashOffset, dashes);
        base.ClientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, true);
    }

    public void SetClipRectanglesChecked(ClipOrdering ordering, uint gc, ushort clipX, ushort clipY,
        Span<Rectangle> rectangles)
    {
        var cookie = this.SetClipRectanglesBase(ordering, gc, clipX, clipY, rectangles);
        base.ClientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, true);
    }

    public void FreeGCChecked(uint gc)
    {
        var cookie = this.FreeGCBase(gc);
        base.ClientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, true);
    }

    public void ClearAreaChecked(bool exposures, uint window, short x, short y, ushort width, ushort height)
    {
        var cookie = this.ClearAreaBase(exposures, window, x, y, width, height);
        base.ClientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, true);
    }

    public void CopyAreaChecked(uint srcDrawable, uint destinationDrawable, uint gc, ushort srcX, ushort srcY,
        ushort destinationX, ushort destinationY, ushort width, ushort height)
    {
        var cookie = this.CopyAreaBase(srcDrawable, destinationDrawable, gc, srcX, srcY, destinationX, destinationY,
            width, height);
        base.ClientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, true);
    }

    public void CopyPlaneChecked(uint srcDrawable, uint destinationDrawable, uint gc, ushort srcX, ushort srcY,
        ushort destinationX, ushort destinationY, ushort width, ushort height, uint bitPlane)
    {
        var cookie = this.CopyPlaneBase(srcDrawable, destinationDrawable, gc, srcX, srcY, destinationX, destinationY,
            width, height, bitPlane);
        base.ClientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, true);
    }

    public void PolyPointChecked(CoordinateMode coordinate, uint drawable, uint gc, Span<Point> points)
    {
        var cookie = this.PolyPointBase(coordinate, drawable, gc, points);
        base.ClientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, true);
    }

    public void PolyLineChecked(CoordinateMode coordinate, uint drawable, uint gc, Span<Point> points)
    {
        var cookie = this.PolyLineBase(coordinate, drawable, gc, points);
        base.ClientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, true);
    }

    public void PolySegmentChecked(uint drawable, uint gc, Span<Segment> segments)
    {
        var cookie = this.PolySegmentBase(drawable, gc, segments);
        base.ClientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, true);
    }

    public void PolyRectangleChecked(uint drawable, uint gc, Span<Rectangle> rectangles)
    {
        var cookie = this.PolyRectangleBase(drawable, gc, rectangles);
        base.ClientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, true);
    }

    public void PolyArcChecked(uint drawable, uint gc, Span<Arc> arcs)
    {
        var cookie = this.PolyArcBase(drawable, gc, arcs);
        base.ClientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, true);
    }

    public void FillPolyChecked(uint drawable, uint gc, PolyShape shape, CoordinateMode coordinate, Span<Point> points)
    {
        var cookie = this.FillPolyBase(drawable, gc, shape, coordinate, points);
        base.ClientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, true);
    }

    public void PolyFillRectangleChecked(uint drawable, uint gc, Span<Rectangle> rectangles)
    {
        var cookie = this.PolyFillRectangleBase(drawable, gc, rectangles);
        base.ClientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, true);
    }

    public void PolyFillArcChecked(uint drawable, uint gc, Span<Arc> arcs)
    {
        var cookie = this.PolyFillArcBase(drawable, gc, arcs);
        base.ClientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, true);
    }

    public void PutImageChecked(ImageFormatBitmap format, uint drawable, uint gc, ushort width, ushort height, short x,
        short y, byte leftPad, byte depth, Span<byte> data)
    {
        var cookie = this.PutImageBase(format, drawable, gc, width, height, x, y, leftPad, depth, data);
        base.ClientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, true);
    }

    public void ImageText8Checked(uint drawable, uint gc, short x, short y, ReadOnlySpan<byte> text)
    {
        var cookie = this.ImageText8Base(drawable, gc, x, y, text);
        base.ClientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, true);
    }

    public void ImageText16Checked(uint drawable, uint gc, short x, short y, ReadOnlySpan<char> text)
    {
        var cookie = this.ImageText16Base(drawable, gc, x, y, text);
        base.ClientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, true);
    }

    public void CreateColormapChecked(ColormapAlloc alloc, uint colormapId, uint window, uint visual)
    {
        var cookie = this.CreateColormapBase(alloc, colormapId, window, visual);
        base.ClientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, true);
    }

    public void FreeColormapChecked(uint colormapId)
    {
        var cookie = this.FreeColormapBase(colormapId);
        base.ClientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, true);
    }

    public void CopyColormapAndFreeChecked(uint colormapId, uint srcColormapId)
    {
        var cookie = this.CopyColormapAndFreeBase(colormapId, srcColormapId);
        base.ClientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, true);
    }

    public void InstallColormapChecked(uint colormapId)
    {
        var cookie = this.InstallColormapBase(colormapId);
        base.ClientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, true);
    }

    public void UninstallColormapChecked(uint colormapId)
    {
        var cookie = this.UninstallColormapBase(colormapId);
        base.ClientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, true);
    }

    public void FreeColorsChecked(uint colormapId, uint planeMask, Span<uint> pixels)
    {
        var cookie = this.FreeColorsBase(colormapId, planeMask, pixels);
        base.ClientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, true);
    }

    public void StoreColorsChecked(uint colormapId, Span<ColorItem> item)
    {
        var cookie = this.StoreColorsBase(colormapId, item);
        base.ClientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, true);
    }

    public void StoreNamedColorChecked(ColorFlag mode, uint colormapId, uint pixels, ReadOnlySpan<byte> name)
    {
        var cookie = this.StoreNamedColorBase(mode, colormapId, pixels, name);
        base.ClientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, true);
    }

    public void CreateCursorChecked(uint cursorId, uint source, uint mask, ushort foreRed, ushort foreGreen,
        ushort foreBlue, ushort backRed, ushort backGreen, ushort backBlue, ushort x, ushort y)
    {
        var cookie = this.CreateCursorBase(cursorId, source, mask, foreRed, foreGreen, foreBlue, backRed, backGreen,
            backBlue, x, y);
        base.ClientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, true);
    }

    public void CreateGlyphCursorChecked(uint cursorId, uint sourceFont, uint fontMask, char sourceChar,
        ushort charMask, ushort foreRed, ushort foreGreen, ushort foreBlue, ushort backRed, ushort backGreen,
        ushort backBlue)
    {
        var cookie = this.CreateGlyphCursorBase(cursorId, sourceFont, fontMask, sourceChar, charMask, foreRed,
            foreGreen, foreBlue, backRed, backGreen, backBlue);
        base.ClientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, true);
    }

    public void FreeCursorChecked(uint cursorId)
    {
        var cookie = this.FreeCursorBase(cursorId);
        base.ClientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, true);
    }

    public void RecolorCursorChecked(uint cursorId, ushort foreRed, ushort foreGreen, ushort foreBlue, ushort backRed,
        ushort backGreen, ushort backBlue)
    {
        var cookie = this.RecolorCursorBase(cursorId, foreRed, foreGreen, foreBlue, backRed, backGreen, backBlue);
        base.ClientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, true);
    }

    public void ChangeKeyboardMappingChecked(byte keycodeCount, byte firstKeycode, byte keysymsPerKeycode,
        Span<uint> keysym)
    {
        var cookie = this.ChangeKeyboardMappingBase(keycodeCount, firstKeycode, keysymsPerKeycode, keysym);
        base.ClientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, true);
    }

    public void BellChecked(sbyte percent)
    {
        var cookie = this.BellBase(percent);
        base.ClientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, true);
    }

    public void ChangeKeyboardControlChecked(KeyboardControlMask mask, Span<uint> args)
    {
        var cookie = this.ChangeKeyboardControlBase(mask, args);
        base.ClientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, true);
    }

    public void ChangePointerControlChecked(Acceleration? acceleration, ushort? threshold)
    {
        var cookie = this.ChangePointerControlBase(acceleration, threshold);
        base.ClientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, true);
    }

    public void SetScreenSaverChecked(short timeout, short interval, TriState preferBlanking, TriState allowExposures)
    {
        var cookie = this.SetScreenSaverBase(timeout, interval, preferBlanking, allowExposures);
        base.ClientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, true);
    }

    public void ForceScreenSaverChecked(ForceScreenSaverMode mode)
    {
        var cookie = this.ForceScreenSaverBase(mode);
        base.ClientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, true);
    }

    public void ChangeHostsChecked(HostMode mode, Family family, Span<byte> address)
    {
        var cookie = this.ChangeHostsBase(mode, family, address);
        base.ClientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, true);
    }

    public void SetAccessControlChecked(AccessControlMode mode)
    {
        var cookie = this.SetAccessControlBase(mode);
        base.ClientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, true);
    }

    public void SetCloseDownModeChecked(CloseDownMode mode)
    {
        var cookie = this.SetCloseDownModeBase(mode);
        base.ClientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, true);
    }

    public void KillClientChecked(uint resource)
    {
        var cookie = this.KillClientBase(resource);
        base.ClientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, true);
    }

    public void NoOperationChecked(Span<uint> args)
    {
        var cookie = this.NoOperationBase(args);
        base.ClientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, true);
    }

    public void PolyText8Checked(uint drawable, uint gc, ushort x, ushort y, TextItem8[] data)
    {
        var cookie = this.PolyText8Base(drawable, gc, x, y, data);
        base.ClientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, true);
    }

    public void PolyText16Checked(uint drawable, uint gc, ushort x, ushort y, TextItem16[] data)
    {
        var cookie = this.PolyText16Base(drawable, gc, x, y, data);
        base.ClientConnection.ProtoIn.SkipErrorForSequence(cookie.Id, true);
    }
}