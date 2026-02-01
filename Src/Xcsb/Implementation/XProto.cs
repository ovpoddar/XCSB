using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using Xcsb.Handlers.Direct;
using Xcsb.Infrastructure;
using Xcsb.Infrastructure.Exceptions;
using Xcsb.Masks;
using Xcsb.Models;
using Xcsb.Models.String;
using Xcsb.Requests;
using Xcsb.Response;
using Xcsb.Response.Replies;
using Xcsb.Response.Replies.Internals;
using Xcsb.Connection;
using Xcsb.Connection.Models.Handshake;
using Xcsb.Connection.Helpers;
using Xcsb.Connection.Response;
using Xcsb.Connection.Response.Errors;
using Xcsb.Connection.Infrastructure.Exceptions;
#if !NETSTANDARD
using System.Numerics;
#endif

namespace Xcsb.Implementation;

#if !NETSTANDARD
[SkipLocalsInit]
#endif
internal sealed class XProto : IXProto
{
    private XBufferProto? _xBufferProto;
    private const int _bigRequestLength = 262140;
    private readonly ProtoInExtended _protoInExtended;
    private readonly ProtoOutExtended _protoOutExtended;

    public IXBufferProto BufferClient => _xBufferProto ??= new XBufferProto(_protoInExtended, _protoOutExtended);

    public XProto(IXConnectionInternal connection)
    {
        if (connection.HandshakeStatus is not HandshakeStatus.Success || connection.HandshakeSuccessResponseBody is null)
            throw new UnauthorizedAccessException(connection.FailReason);
        _protoInExtended = new ProtoInExtended(connection.Accesser); //Todo: can be more organised with interface and staffs will think about them after api finalized
        _protoOutExtended = new ProtoOutExtended(connection.Accesser);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WaitForEvent() =>
        _protoInExtended.WaitForEventArrival();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsEventAvailable() =>
        _protoInExtended.HasEventToProcesses();

    public AllocColorReply AllocColor(uint colorMap, ushort red, ushort green, ushort blue)
    {
        var cookie = AllocColorBase(colorMap, red, green, blue);
        var (result, error) = this._protoInExtended.ReceivedResponse<AllocColorReply>(cookie.Id);
        return error.HasValue
            ? throw new XEventException(error.Value)
            : result!.Value;
    }

    public AllocColorCellsReply AllocColorCells(bool contiguous, uint colorMap, ushort colors, ushort planes)
    {
        var cookie = AllocColorCellsBase(contiguous, colorMap, colors, planes);
        var (result, error) = this._protoInExtended.ReceivedResponseSpan<AllocColorCellsResponse>(cookie.Id);
        return error.HasValue
            ? throw new XEventException(error.Value)
            : new AllocColorCellsReply(result);
    }

    public AllocColorPlanesReply AllocColorPlanes(bool contiguous, uint colorMap, ushort colors, ushort reds,
        ushort greens, ushort blues)
    {
        var cookie = AllocColorPlanesBase(contiguous, colorMap, colors, reds, greens, blues);
        var (result, error) = this._protoInExtended.ReceivedResponseSpan<AllocColorPlanesResponse>(cookie.Id);
        return error.HasValue
            ? throw new XEventException(error.Value)
            : new AllocColorPlanesReply(result);
    }

    public AllocNamedColorReply AllocNamedColor(uint colorMap, ReadOnlySpan<byte> name)
    {
        var cookie = AllocNamedColorBase(colorMap, name);
        var (result, error) = this._protoInExtended.ReceivedResponse<AllocNamedColorReply>(cookie.Id);
        return error.HasValue
            ? throw new XEventException(error.Value)
            : result!.Value;
    }


    public GetAtomNameReply GetAtomName(ATOM atom)
    {
        var cookie = GetAtomNameBase(atom);
        var (result, error) = this._protoInExtended.ReceivedResponseSpan<GetAtomNameResponse>(cookie.Id);
        return error.HasValue
            ? throw new XEventException(error.Value)
            : new GetAtomNameReply(result);
    }

    public InternAtomReply InternAtom(bool onlyIfExist, string atomName)
    {
        var cookie = InternAtomBase(onlyIfExist, atomName);
        var (result, error) = this._protoInExtended.ReceivedResponse<InternAtomReply>(cookie.Id);
        return error.HasValue
            ? throw new XEventException(error.Value)
            : result!.Value;
    }

    public GetFontPathReply GetFontPath()
    {
        var cookie = GetFontPathBase();
        var (result, error) = this._protoInExtended.ReceivedResponseSpan<GetFontPathResponse>(cookie.Id);
        return error.HasValue
            ? throw new XEventException(error.Value)
            : new GetFontPathReply(result);
    }

    public GetGeometryReply GetGeometry(uint drawable)
    {
        var cookie = GetGeometryBase(drawable);
        var (result, error) = this._protoInExtended.ReceivedResponse<GetGeometryReply>(cookie.Id);
        return error.HasValue
            ? throw new XEventException(error.Value)
            : result!.Value;
    }

    public GetImageReply GetImage(ImageFormat format, uint drawable, ushort x, ushort y, ushort width, ushort height,
        uint planeMask)
    {
        var cookie = GetImageBase(format, drawable, x, y, width, height, planeMask);
        var (result, error) = this._protoInExtended.ReceivedResponseSpan<GetImageResponse>(cookie.Id);
        return error.HasValue
            ? throw new XEventException(error.Value)
            : new GetImageReply(result);
    }

    public GetInputFocusReply GetInputFocus()
    {
        var cookie = GetInputFocusBase();
        var (result, error) = this._protoInExtended.ReceivedResponse<GetInputFocusReply>(cookie.Id);
        return error.HasValue
            ? throw new XEventException(error.Value)
            : result!.Value;
    }

    public GetKeyboardControlReply GetKeyboardControl()
    {
        var cookie = GetKeyboardControlBase();
        var (result, error) = this._protoInExtended.ReceivedResponse<GetKeyboardControlResponse>(cookie.Id);
        return error.HasValue
            ? throw new XEventException(error.Value)
            : new GetKeyboardControlReply(result!.Value);
    }

    public GetKeyboardMappingReply GetKeyboardMapping(byte firstKeycode, byte count)
    {
        var cookie = GetKeyboardMappingBase(firstKeycode, count);
        var (result, error) = this._protoInExtended.ReceivedResponseSpan<GetKeyboardMappingResponse>(cookie.Id);
        return error.HasValue
            ? throw new XEventException(error.Value)
            : new GetKeyboardMappingReply(result, count);
    }

    public GetModifierMappingReply GetModifierMapping()
    {
        var cookie = GetModifierMappingBase();
        var (result, error) = this._protoInExtended.ReceivedResponseSpan<GetModifierMappingResponse>(cookie.Id);
        return error.HasValue
            ? throw new XEventException(error.Value)
            : new GetModifierMappingReply(result);
    }

    public GetMotionEventsReply GetMotionEvents(uint window, uint startTime, uint endTime)
    {
        var cookie = GetMotionEventsBase(window, startTime, endTime);
        var (result, error) = this._protoInExtended.ReceivedResponseSpan<GetMotionEventsResponse>(cookie.Id);
        return error.HasValue
            ? throw new XEventException(error.Value)
            : new GetMotionEventsReply(result);
    }

    public GetPointerControlReply GetPointerControl()
    {
        var cookie = GetPointerControlBase();
        var (result, error) = this._protoInExtended.ReceivedResponse<GetPointerControlReply>(cookie.Id);
        return error.HasValue
            ? throw new XEventException(error.Value)
            : result!.Value;
    }

    public GetPointerMappingReply GetPointerMapping()
    {
        var cookie = GetPointerMappingBase();
        var (result, error) = this._protoInExtended.ReceivedResponseSpan<GetPointerMappingResponse>(cookie.Id);
        return error.HasValue
            ? throw new XEventException(error.Value)
            : new GetPointerMappingReply(result);
    }

    public GetPropertyReply GetProperty(bool delete, uint window, ATOM property, ATOM type, uint offset, uint length)
    {
        var cookie = GetPropertyBase(delete, window, property, type, offset, length);
        var (result, error) = this._protoInExtended.ReceivedResponseSpan<GetPropertyResponse>(cookie.Id);
        return error.HasValue
            ? throw new XEventException(error.Value)
            : new GetPropertyReply(result);
    }

    public GetScreenSaverReply GetScreenSaver()
    {
        var cookie = GetScreenSaverBase();
        var (result, error) = this._protoInExtended.ReceivedResponse<GetScreenSaverReply>(cookie.Id);
        return error.HasValue
            ? throw new XEventException(error.Value)
            : result!.Value;
    }

    public GetSelectionOwnerReply GetSelectionOwner(ATOM atom)
    {
        var cookie = GetSelectionOwnerBase(atom);
        var (result, error) = this._protoInExtended.ReceivedResponse<GetSelectionOwnerReply>(cookie.Id);
        return error.HasValue
            ? throw new XEventException(error.Value)
            : result!.Value;
    }

    public GetWindowAttributesReply GetWindowAttributes(uint window)
    {
        var cookie = GetWindowAttributesBase(window);
        var (result, error) = this._protoInExtended.ReceivedResponse<GetWindowAttributesReply>(cookie.Id);
        return error.HasValue
            ? throw new XEventException(error.Value)
            : result!.Value;
    }

    public ListExtensionsReply ListExtensions()
    {
        var cookie = ListExtensionsBase();
        var (result, error) = this._protoInExtended.ReceivedResponseSpan<ListExtensionsResponse>(cookie.Id);
        return error.HasValue
            ? throw new XEventException(error.Value)
            : new ListExtensionsReply(result);
    }

    public ListFontsReply ListFonts(ReadOnlySpan<byte> pattern, int maxNames)
    {
        var cookie = ListFontsBase(pattern, maxNames);
        var (result, error) = this._protoInExtended.ReceivedResponseSpan<ListFontsResponse>(cookie.Id);
        return error.HasValue
            ? throw new XEventException(error.Value)
            : new ListFontsReply(result);
    }

    public ListFontsWithInfoReply[] ListFontsWithInfo(ReadOnlySpan<byte> pattan, int maxNames)
    {
        var cookie = ListFontsWithInfoBase(pattan, maxNames);
        var (result, error) = this._protoInExtended.ReceivedResponseArray(cookie.Id, maxNames);
        return error.HasValue
            ? throw new XEventException(error.Value)
            : result;
    }

    public ListHostsReply ListHosts()
    {
        var cookie = ListHostsBase();
        var (result, error) = this._protoInExtended.ReceivedResponseSpan<ListHostsResponse>(cookie.Id);
        return error.HasValue
            ? throw new XEventException(error.Value)
            : new ListHostsReply(result);
    }

    public ListInstalledColormapsReply ListInstalledColormaps(uint window)
    {
        var cookie = ListInstalledColormapsBase(window);
        var (result, error) = this._protoInExtended.ReceivedResponseSpan<ListInstalledColormapsResponse>(cookie.Id);
        return error.HasValue
            ? throw new XEventException(error.Value)
            : new ListInstalledColormapsReply(result);
    }

    public ListPropertiesReply ListProperties(uint window)
    {
        var cookie = ListPropertiesBase(window);
        var (result, error) = this._protoInExtended.ReceivedResponseSpan<ListPropertiesResponse>(cookie.Id);
        return error.HasValue
            ? throw new XEventException(error.Value)
            : new ListPropertiesReply(result);
    }

    public LookupColorReply LookupColor(uint colorMap, ReadOnlySpan<byte> name)
    {
        var cookie = LookupColorBase(colorMap, name);
        var (result, error) = this._protoInExtended.ReceivedResponse<LookupColorReply>(cookie.Id);
        return error.HasValue
            ? throw new XEventException(error.Value)
            : result!.Value;
    }

    public QueryBestSizeReply QueryBestSize(QueryShapeOf shape, uint drawable, ushort width, ushort height)
    {
        var cookie = QueryBestSizeBase(shape, drawable, width, height);
        var (result, error) = this._protoInExtended.ReceivedResponse<QueryBestSizeReply>(cookie.Id);
        return error.HasValue
            ? throw new XEventException(error.Value)
            : result!.Value;
    }

    public QueryColorsReply QueryColors(uint colorMap, Span<uint> pixels)
    {
        var cookie = QueryColorsBase(colorMap, pixels);
        var (result, error) = this._protoInExtended.ReceivedResponseSpan<QueryColorsResponse>(cookie.Id);
        return error.HasValue
            ? throw new XEventException(error.Value)
            : new QueryColorsReply(result);
    }

    public QueryExtensionReply QueryExtension(ReadOnlySpan<byte> name)
    {
        if (name.Length > ushort.MaxValue)
            throw new ArgumentException($"{nameof(name)} is invalid, {nameof(name)} is too long.");
        var cookie = QueryExtensionBase(name);
        var (result, error) = this._protoInExtended.ReceivedResponse<QueryExtensionReply>(cookie.Id);
        return error.HasValue
            ? throw new XEventException(error.Value)
            : result!.Value;
    }

    public QueryFontReply QueryFont(uint fontId)
    {
        var cookie = QueryFontBase(fontId);
        var (result, error) = this._protoInExtended.ReceivedResponseSpan<QueryFontResponse>(cookie.Id);
        return error.HasValue
            ? throw new XEventException(error.Value)
            : new QueryFontReply(result);
    }

    public QueryKeymapReply QueryKeymap()
    {
        var cookie = QueryKeymapBase();
        var (result, error) = this._protoInExtended.ReceivedResponse<QueryKeymapResponse>(cookie.Id);
        return error.HasValue
            ? throw new XEventException(error.Value)
            : new QueryKeymapReply(result!.Value);
    }

    public QueryPointerReply QueryPointer(uint window)
    {
        var cookie = QueryPointerBase(window);
        var (result, error) = this._protoInExtended.ReceivedResponse<QueryPointerReply>(cookie.Id);
        return error.HasValue
            ? throw new XEventException(error.Value)
            : result!.Value;
    }

    public QueryTextExtentsReply QueryTextExtents(uint font, ReadOnlySpan<char> stringForQuery)
    {
        var cookie = QueryTextExtentsBase(font, stringForQuery);
        var (result, error) = this._protoInExtended.ReceivedResponse<QueryTextExtentsReply>(cookie.Id);
        return error.HasValue
            ? throw new XEventException(error.Value)
            : result!.Value;
    }

    public QueryTreeReply QueryTree(uint window)
    {
        var cookie = QueryTreeBase(window);
        var (result, error) = this._protoInExtended.ReceivedResponseSpan<QueryTreeResponse>(cookie.Id);
        return error.HasValue
            ? throw new XEventException(error.Value)
            : new QueryTreeReply(result);
    }


    public GrabKeyboardReply GrabKeyboard(bool ownerEvents, uint grabWindow, uint timeStamp, GrabMode pointerMode,
        GrabMode keyboardMode)
    {
        var cookie = GrabKeyboardBase(ownerEvents, grabWindow, timeStamp, pointerMode, keyboardMode);
        var (result, error) = this._protoInExtended.ReceivedResponse<GrabKeyboardReply>(cookie.Id);
        return error.HasValue
            ? throw new XEventException(error.Value)
            : result!.Value;
    }

    public GrabPointerReply GrabPointer(bool ownerEvents, uint grabWindow, ushort mask, GrabMode pointerMode,
        GrabMode keyboardMode, uint confineTo, uint cursor, uint timeStamp)
    {
        var cookie = GrabPointerBase(ownerEvents, grabWindow, mask, pointerMode, keyboardMode, confineTo, cursor,
            timeStamp);
        var (result, error) = this._protoInExtended.ReceivedResponse<GrabPointerReply>(cookie.Id);
        return error.HasValue
            ? throw new XEventException(error.Value)
            : result!.Value;
    }

    public SetModifierMappingReply SetModifierMapping(Span<ulong> keycodes)
    {
        var cookie = SetModifierMappingBase(keycodes);
        var (result, error) = this._protoInExtended.ReceivedResponse<SetModifierMappingReply>(cookie.Id);
        return error.HasValue
            ? throw new XEventException(error.Value)
            : result!.Value;
    }

    public SetPointerMappingReply SetPointerMapping(Span<byte> maps)
    {
        var cookie = SetPointerMappingBase(maps);
        var (result, error) = this._protoInExtended.ReceivedResponse<SetPointerMappingReply>(cookie.Id);
        return error.HasValue
            ? throw new XEventException(error.Value)
            : result!.Value;
    }

    public TranslateCoordinatesReply TranslateCoordinates(uint srcWindow, uint destinationWindow, ushort srcX,
        ushort srcY)
    {
        var cookie = TranslateCoordinatesBase(srcWindow, destinationWindow, srcX, srcY);
        var (result, error) = this._protoInExtended.ReceivedResponse<TranslateCoordinatesReply>(cookie.Id);
        return error.HasValue
            ? throw new XEventException(error.Value)
            : result!.Value;
    }

    public XEvent GetEvent() =>
        this._protoInExtended.ReceivedResponse();

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

    public GenericError? CheckResponseProtoResult(ResponseProto response) =>
        this._protoInExtended.GetVoidRequestResponse<GenericError>(response);

    public void VerifyResponseProtoResult(ResponseProto response)
    {
        var error = this._protoInExtended.GetVoidRequestResponse<GenericError>(response);
        if (error.HasValue) throw new XEventException(error.Value);
    }

    public void CreateWindowUnchecked(byte depth, uint window, uint parent, short x, short y, ushort width,
        ushort height, ushort borderWidth, ClassType classType, uint rootVisualId, ValueMask mask, Span<uint> args)
    {
        var cookie = this.CreateWindowBase(depth, window, parent, x, y, width, height, borderWidth, classType,
            rootVisualId, mask, args);
        this._protoInExtended.SkipErrorForSequence(cookie.Id, false);
    }

    public void ChangeWindowAttributesUnchecked(uint window, ValueMask mask, Span<uint> args)
    {
        var cookie = this.ChangeWindowAttributesBase(window, mask, args);
        this._protoInExtended.SkipErrorForSequence(cookie.Id, false);
    }

    public void DestroyWindowUnchecked(uint window)
    {
        var cookie = this.DestroyWindowBase(window);
        this._protoInExtended.SkipErrorForSequence(cookie.Id, false);
    }

    public void DestroySubwindowsUnchecked(uint window)
    {
        var cookie = this.DestroySubwindowsBase(window);
        this._protoInExtended.SkipErrorForSequence(cookie.Id, false);
    }

    public void ChangeSaveSetUnchecked(ChangeSaveSetMode changeSaveSetMode, uint window)
    {
        var cookie = this.ChangeSaveSetBase(changeSaveSetMode, window);
        this._protoInExtended.SkipErrorForSequence(cookie.Id, false);
    }

    public void ReparentWindowUnchecked(uint window, uint parent, short x, short y)
    {
        var cookie = this.ReparentWindowBase(window, parent, x, y);
        this._protoInExtended.SkipErrorForSequence(cookie.Id, false);
    }

    public void MapWindowUnchecked(uint window)
    {
        var cookie = this.MapWindowBase(window);
        this._protoInExtended.SkipErrorForSequence(cookie.Id, false);
    }

    public void MapSubwindowsUnchecked(uint window)
    {
        var cookie = this.MapSubwindowsBase(window);
        this._protoInExtended.SkipErrorForSequence(cookie.Id, false);
    }

    public void UnmapWindowUnchecked(uint window)
    {
        var cookie = this.UnmapWindowBase(window);
        this._protoInExtended.SkipErrorForSequence(cookie.Id, false);
    }

    public void UnmapSubwindowsUnchecked(uint window)
    {
        var cookie = this.UnmapSubwindowsBase(window);
        this._protoInExtended.SkipErrorForSequence(cookie.Id, false);
    }

    public void ConfigureWindowUnchecked(uint window, ConfigureValueMask mask, Span<uint> args)
    {
        var cookie = this.ConfigureWindowBase(window, mask, args);
        this._protoInExtended.SkipErrorForSequence(cookie.Id, false);
    }

    public void CirculateWindowUnchecked(Circulate circulate, uint window)
    {
        var cookie = this.CirculateWindowBase(circulate, window);
        this._protoInExtended.SkipErrorForSequence(cookie.Id, false);
    }

    public void ChangePropertyUnchecked<T>(PropertyMode mode, uint window, ATOM property, ATOM type, Span<T> args)
        where T : struct
#if !NETSTANDARD
        , INumber<T>
#endif
    {
        var cookie = this.ChangePropertyBase(mode, window, property, type, args);
        this._protoInExtended.SkipErrorForSequence(cookie.Id, false);
    }

    public void DeletePropertyUnchecked(uint window, ATOM atom)
    {
        var cookie = this.DeletePropertyBase(window, atom);
        this._protoInExtended.SkipErrorForSequence(cookie.Id, false);
    }

    public void RotatePropertiesUnchecked(uint window, ushort delta, Span<ATOM> properties)
    {
        var cookie = this.RotatePropertiesBase(window, delta, properties);
        this._protoInExtended.SkipErrorForSequence(cookie.Id, false);
    }

    public void SetSelectionOwnerUnchecked(uint owner, ATOM atom, uint timestamp)
    {
        var cookie = this.SetSelectionOwnerBase(owner, atom, timestamp);
        this._protoInExtended.SkipErrorForSequence(cookie.Id, false);
    }

    public void ConvertSelectionUnchecked(uint requestor, ATOM selection, ATOM target, ATOM property, uint timestamp)
    {
        var cookie = this.ConvertSelectionBase(requestor, selection, target, property, timestamp);
        this._protoInExtended.SkipErrorForSequence(cookie.Id, false);
    }

    public void SendEventUnchecked(bool propagate, uint destination, uint eventMask, XEvent evnt)
    {
        var cookie = this.SendEventBase(propagate, destination, eventMask, evnt);
        this._protoInExtended.SkipErrorForSequence(cookie.Id, false);
    }

    public void UngrabPointerUnchecked(uint time)
    {
        var cookie = this.UngrabPointerBase(time);
        this._protoInExtended.SkipErrorForSequence(cookie.Id, false);
    }

    public void GrabButtonUnchecked(bool ownerEvents, uint grabWindow, ushort mask, GrabMode pointerMode,
        GrabMode keyboardMode, uint confineTo, uint cursor, Button button, ModifierMask modifiers)
    {
        var cookie = this.GrabButtonBase(ownerEvents, grabWindow, mask, pointerMode, keyboardMode, confineTo, cursor,
            button, modifiers);
        this._protoInExtended.SkipErrorForSequence(cookie.Id, false);
    }

    public void UngrabButtonUnchecked(Button button, uint grabWindow, ModifierMask mask)
    {
        var cookie = this.UngrabButtonBase(button, grabWindow, mask);
        this._protoInExtended.SkipErrorForSequence(cookie.Id, false);
    }

    public void ChangeActivePointerGrabUnchecked(uint cursor, uint time, ushort mask)
    {
        var cookie = this.ChangeActivePointerGrabBase(cursor, time, mask);
        this._protoInExtended.SkipErrorForSequence(cookie.Id, false);
    }

    public void UngrabKeyboardUnchecked(uint time)
    {
        var cookie = this.UngrabKeyboardBase(time);
        this._protoInExtended.SkipErrorForSequence(cookie.Id, false);
    }

    public void GrabKeyUnchecked(bool exposures, uint grabWindow, ModifierMask mask, byte keycode, GrabMode pointerMode,
        GrabMode keyboardMode)
    {
        var cookie = this.GrabKeyBase(exposures, grabWindow, mask, keycode, pointerMode, keyboardMode);
        this._protoInExtended.SkipErrorForSequence(cookie.Id, false);
    }

    public void UngrabKeyUnchecked(byte key, uint grabWindow, ModifierMask modifier)
    {
        var cookie = this.UngrabKeyBase(key, grabWindow, modifier);
        this._protoInExtended.SkipErrorForSequence(cookie.Id, false);
    }

    public void AllowEventsUnchecked(EventsMode mode, uint time)
    {
        var cookie = this.AllowEventsBase(mode, time);
        this._protoInExtended.SkipErrorForSequence(cookie.Id, false);
    }

    public void GrabServerUnchecked()
    {
        var cookie = this.GrabServerBase();
        this._protoInExtended.SkipErrorForSequence(cookie.Id, false);
    }

    public void UngrabServerUnchecked()
    {
        var cookie = this.UngrabServerBase();
        this._protoInExtended.SkipErrorForSequence(cookie.Id, false);
    }

    public void WarpPointerUnchecked(uint srcWindow, uint destinationWindow, short srcX, short srcY, ushort srcWidth,
        ushort srcHeight, short destinationX, short destinationY)
    {
        var cookie = this.WarpPointerBase(srcWindow, destinationWindow, srcX, srcY, srcWidth, srcHeight, destinationX,
            destinationY);
        this._protoInExtended.SkipErrorForSequence(cookie.Id, false);
    }

    public void SetInputFocusUnchecked(InputFocusMode mode, uint focus, uint time)
    {
        var cookie = this.SetInputFocusBase(mode, focus, time);
        this._protoInExtended.SkipErrorForSequence(cookie.Id, false);
    }

    public void OpenFontUnchecked(string fontName, uint fontId)
    {
        var cookie = this.OpenFontBase(fontName, fontId);
        this._protoInExtended.SkipErrorForSequence(cookie.Id, false);
    }

    public void CloseFontUnchecked(uint fontId)
    {
        var cookie = this.CloseFontBase(fontId);
        this._protoInExtended.SkipErrorForSequence(cookie.Id, false);
    }

    public void SetFontPathUnchecked(string[] strPaths)
    {
        var cookie = this.SetFontPathBase(strPaths);
        this._protoInExtended.SkipErrorForSequence(cookie.Id, false);
    }

    public void CreatePixmapUnchecked(byte depth, uint pixmapId, uint drawable, ushort width, ushort height)
    {
        var cookie = this.CreatePixmapBase(depth, pixmapId, drawable, width, height);
        this._protoInExtended.SkipErrorForSequence(cookie.Id, false);
    }

    public void FreePixmapUnchecked(uint pixmapId)
    {
        var cookie = this.FreePixmapBase(pixmapId);
        this._protoInExtended.SkipErrorForSequence(cookie.Id, false);
    }

    public void CreateGCUnchecked(uint gc, uint drawable, GCMask mask, Span<uint> args)
    {
        var cookie = this.CreateGCBase(gc, drawable, mask, args);
        this._protoInExtended.SkipErrorForSequence(cookie.Id, false);
    }

    public void ChangeGCUnchecked(uint gc, GCMask mask, Span<uint> args)
    {
        var cookie = this.ChangeGCBase(gc, mask, args);
        this._protoInExtended.SkipErrorForSequence(cookie.Id, false);
    }

    public void CopyGCUnchecked(uint srcGc, uint dstGc, GCMask mask)
    {
        var cookie = this.CopyGCBase(srcGc, dstGc, mask);
        this._protoInExtended.SkipErrorForSequence(cookie.Id, false);
    }

    public void SetDashesUnchecked(uint gc, ushort dashOffset, Span<byte> dashes)
    {
        var cookie = this.SetDashesBase(gc, dashOffset, dashes);
        this._protoInExtended.SkipErrorForSequence(cookie.Id, false);
    }

    public void SetClipRectanglesUnchecked(ClipOrdering ordering, uint gc, ushort clipX, ushort clipY,
        Span<Rectangle> rectangles)
    {
        var cookie = this.SetClipRectanglesBase(ordering, gc, clipX, clipY, rectangles);
        this._protoInExtended.SkipErrorForSequence(cookie.Id, false);
    }

    public void FreeGCUnchecked(uint gc)
    {
        var cookie = this.FreeGCBase(gc);
        this._protoInExtended.SkipErrorForSequence(cookie.Id, false);
    }

    public void ClearAreaUnchecked(bool exposures, uint window, short x, short y, ushort width, ushort height)
    {
        var cookie = this.ClearAreaBase(exposures, window, x, y, width, height);
        this._protoInExtended.SkipErrorForSequence(cookie.Id, false);
    }

    public void CopyAreaUnchecked(uint srcDrawable, uint destinationDrawable, uint gc, ushort srcX, ushort srcY,
        ushort destinationX, ushort destinationY, ushort width, ushort height)
    {
        var cookie = this.CopyAreaBase(srcDrawable, destinationDrawable, gc, srcX, srcY, destinationX, destinationY,
            width, height);
        this._protoInExtended.SkipErrorForSequence(cookie.Id, false);
    }

    public void CopyPlaneUnchecked(uint srcDrawable, uint destinationDrawable, uint gc, ushort srcX, ushort srcY,
        ushort destinationX, ushort destinationY, ushort width, ushort height, uint bitPlane)
    {
        var cookie = this.CopyPlaneBase(srcDrawable, destinationDrawable, gc, srcX, srcY, destinationX, destinationY,
            width, height, bitPlane);
        this._protoInExtended.SkipErrorForSequence(cookie.Id, false);
    }

    public void PolyPointUnchecked(CoordinateMode coordinate, uint drawable, uint gc, Span<Point> points)
    {
        var cookie = this.PolyPointBase(coordinate, drawable, gc, points);
        this._protoInExtended.SkipErrorForSequence(cookie.Id, false);
    }

    public void PolyLineUnchecked(CoordinateMode coordinate, uint drawable, uint gc, Span<Point> points)
    {
        var cookie = this.PolyLineBase(coordinate, drawable, gc, points);
        this._protoInExtended.SkipErrorForSequence(cookie.Id, false);
    }

    public void PolySegmentUnchecked(uint drawable, uint gc, Span<Segment> segments)
    {
        var cookie = this.PolySegmentBase(drawable, gc, segments);
        this._protoInExtended.SkipErrorForSequence(cookie.Id, false);
    }

    public void PolyRectangleUnchecked(uint drawable, uint gc, Span<Rectangle> rectangles)
    {
        var cookie = this.PolyRectangleBase(drawable, gc, rectangles);
        this._protoInExtended.SkipErrorForSequence(cookie.Id, false);
    }

    public void PolyArcUnchecked(uint drawable, uint gc, Span<Arc> arcs)
    {
        var cookie = this.PolyArcBase(drawable, gc, arcs);
        this._protoInExtended.SkipErrorForSequence(cookie.Id, false);
    }

    public void FillPolyUnchecked(uint drawable, uint gc, PolyShape shape, CoordinateMode coordinate,
        Span<Point> points)
    {
        var cookie = this.FillPolyBase(drawable, gc, shape, coordinate, points);
        this._protoInExtended.SkipErrorForSequence(cookie.Id, false);
    }

    public void PolyFillRectangleUnchecked(uint drawable, uint gc, Span<Rectangle> rectangles)
    {
        var cookie = this.PolyFillRectangleBase(drawable, gc, rectangles);
        this._protoInExtended.SkipErrorForSequence(cookie.Id, false);
    }

    public void PolyFillArcUnchecked(uint drawable, uint gc, Span<Arc> arcs)
    {
        var cookie = this.PolyFillArcBase(drawable, gc, arcs);
        this._protoInExtended.SkipErrorForSequence(cookie.Id, false);
    }

    public void PutImageUnchecked(ImageFormatBitmap format, uint drawable, uint gc, ushort width, ushort height,
        short x,
        short y, byte leftPad, byte depth, Span<byte> data)
    {
        var cookie = this.PutImageBase(format, drawable, gc, width, height, x, y, leftPad, depth, data);
        this._protoInExtended.SkipErrorForSequence(cookie.Id, false);
    }

    public void ImageText8Unchecked(uint drawable, uint gc, short x, short y, ReadOnlySpan<byte> text)
    {
        var cookie = this.ImageText8Base(drawable, gc, x, y, text);
        this._protoInExtended.SkipErrorForSequence(cookie.Id, false);
    }

    public void ImageText16Unchecked(uint drawable, uint gc, short x, short y, ReadOnlySpan<char> text)
    {
        var cookie = this.ImageText16Base(drawable, gc, x, y, text);
        this._protoInExtended.SkipErrorForSequence(cookie.Id, false);
    }

    public void CreateColormapUnchecked(ColormapAlloc alloc, uint colormapId, uint window, uint visual)
    {
        var cookie = this.CreateColormapBase(alloc, colormapId, window, visual);
        this._protoInExtended.SkipErrorForSequence(cookie.Id, false);
    }

    public void FreeColormapUnchecked(uint colormapId)
    {
        var cookie = this.FreeColormapBase(colormapId);
        this._protoInExtended.SkipErrorForSequence(cookie.Id, false);
    }

    public void CopyColormapAndFreeUnchecked(uint colormapId, uint srcColormapId)
    {
        var cookie = this.CopyColormapAndFreeBase(colormapId, srcColormapId);
        this._protoInExtended.SkipErrorForSequence(cookie.Id, false);
    }

    public void InstallColormapUnchecked(uint colormapId)
    {
        var cookie = this.InstallColormapBase(colormapId);
        this._protoInExtended.SkipErrorForSequence(cookie.Id, false);
    }

    public void UninstallColormapUnchecked(uint colormapId)
    {
        var cookie = this.UninstallColormapBase(colormapId);
        this._protoInExtended.SkipErrorForSequence(cookie.Id, false);
    }

    public void FreeColorsUnchecked(uint colormapId, uint planeMask, Span<uint> pixels)
    {
        var cookie = this.FreeColorsBase(colormapId, planeMask, pixels);
        this._protoInExtended.SkipErrorForSequence(cookie.Id, false);
    }

    public void StoreColorsUnchecked(uint colormapId, Span<ColorItem> item)
    {
        var cookie = this.StoreColorsBase(colormapId, item);
        this._protoInExtended.SkipErrorForSequence(cookie.Id, false);
    }

    public void StoreNamedColorUnchecked(ColorFlag mode, uint colormapId, uint pixels, ReadOnlySpan<byte> name)
    {
        var cookie = this.StoreNamedColorBase(mode, colormapId, pixels, name);
        this._protoInExtended.SkipErrorForSequence(cookie.Id, false);
    }

    public void CreateCursorUnchecked(uint cursorId, uint source, uint mask, ushort foreRed, ushort foreGreen,
        ushort foreBlue, ushort backRed, ushort backGreen, ushort backBlue, ushort x, ushort y)
    {
        var cookie = this.CreateCursorBase(cursorId, source, mask, foreRed, foreGreen, foreBlue, backRed,
            backGreen, backBlue, x, y);
        this._protoInExtended.SkipErrorForSequence(cookie.Id, false);
    }

    public void CreateGlyphCursorUnchecked(uint cursorId, uint sourceFont, uint fontMask, char sourceChar,
        ushort charMask, ushort foreRed, ushort foreGreen, ushort foreBlue, ushort backRed, ushort backGreen,
        ushort backBlue)
    {
        var cookie = this.CreateGlyphCursorBase(cursorId, sourceFont, fontMask, sourceChar, charMask, foreRed,
            foreGreen, foreBlue, backRed, backGreen, backBlue);
        this._protoInExtended.SkipErrorForSequence(cookie.Id, false);
    }

    public void FreeCursorUnchecked(uint cursorId)
    {
        var cookie = this.FreeCursorBase(cursorId);
        this._protoInExtended.SkipErrorForSequence(cookie.Id, false);
    }

    public void RecolorCursorUnchecked(uint cursorId, ushort foreRed, ushort foreGreen, ushort foreBlue, ushort backRed,
        ushort backGreen, ushort backBlue)
    {
        var cookie = this.RecolorCursorBase(cursorId, foreRed, foreGreen, foreBlue, backRed, backGreen, backBlue);
        this._protoInExtended.SkipErrorForSequence(cookie.Id, false);
    }

    public void ChangeKeyboardMappingUnchecked(byte keycodeCount, byte firstKeycode, byte keysymsPerKeycode,
        Span<uint> keysym)
    {
        var cookie = this.ChangeKeyboardMappingBase(keycodeCount, firstKeycode, keysymsPerKeycode, keysym);
        this._protoInExtended.SkipErrorForSequence(cookie.Id, false);
    }

    public void BellUnchecked(sbyte percent)
    {
        var cookie = this.BellBase(percent);
        this._protoInExtended.SkipErrorForSequence(cookie.Id, false);
    }

    public void ChangeKeyboardControlUnchecked(KeyboardControlMask mask, Span<uint> args)
    {
        var cookie = this.ChangeKeyboardControlBase(mask, args);
        this._protoInExtended.SkipErrorForSequence(cookie.Id, false);
    }

    public void ChangePointerControlUnchecked(Acceleration? acceleration, ushort? threshold)
    {
        var cookie = this.ChangePointerControlBase(acceleration, threshold);
        this._protoInExtended.SkipErrorForSequence(cookie.Id, false);
    }

    public void SetScreenSaverUnchecked(short timeout, short interval, TriState preferBlanking, TriState allowExposures)
    {
        var cookie = this.SetScreenSaverBase(timeout, interval, preferBlanking, allowExposures);
        this._protoInExtended.SkipErrorForSequence(cookie.Id, false);
    }

    public void ForceScreenSaverUnchecked(ForceScreenSaverMode mode)
    {
        var cookie = this.ForceScreenSaverBase(mode);
        this._protoInExtended.SkipErrorForSequence(cookie.Id, false);
    }

    public void ChangeHostsUnchecked(HostMode mode, Family family, Span<byte> address)
    {
        var cookie = this.ChangeHostsBase(mode, family, address);
        this._protoInExtended.SkipErrorForSequence(cookie.Id, false);
    }

    public void SetAccessControlUnchecked(AccessControlMode mode)
    {
        var cookie = this.SetAccessControlBase(mode);
        this._protoInExtended.SkipErrorForSequence(cookie.Id, false);
    }

    public void SetCloseDownModeUnchecked(CloseDownMode mode)
    {
        var cookie = this.SetCloseDownModeBase(mode);
        this._protoInExtended.SkipErrorForSequence(cookie.Id, false);
    }

    public void KillClientUnchecked(uint resource)
    {
        var cookie = this.KillClientBase(resource);
        this._protoInExtended.SkipErrorForSequence(cookie.Id, false);
    }

    public void NoOperationUnchecked(Span<uint> args)
    {
        var cookie = this.NoOperationBase(args);
        this._protoInExtended.SkipErrorForSequence(cookie.Id, false);
    }

    public void PolyText8Unchecked(uint drawable, uint gc, ushort x, ushort y, TextItem8[] data)
    {
        var cookie = this.PolyText8Base(drawable, gc, x, y, data);
        this._protoInExtended.SkipErrorForSequence(cookie.Id, false);
    }

    public void PolyText16Unchecked(uint drawable, uint gc, ushort x, ushort y, TextItem16[] data)
    {
        var cookie = this.PolyText16Base(drawable, gc, x, y, data);
        this._protoInExtended.SkipErrorForSequence(cookie.Id, false);
    }

    public void CreateWindowChecked(byte depth, uint window, uint parent, short x, short y, ushort width, ushort height,
        ushort borderWidth, ClassType classType, uint rootVisualId, ValueMask mask, Span<uint> args)
    {
        var cookie = this.CreateWindowBase(depth, window, parent, x, y, width, height, borderWidth, classType,
            rootVisualId, mask, args);
        this._protoInExtended.SkipErrorForSequence(cookie.Id, true);
    }

    public void ChangeWindowAttributesChecked(uint window, ValueMask mask, Span<uint> args)
    {
        var cookie = this.ChangeWindowAttributesBase(window, mask, args);
        this._protoInExtended.SkipErrorForSequence(cookie.Id, true);
    }

    public void DestroyWindowChecked(uint window)
    {
        var cookie = this.DestroyWindowBase(window);
        this._protoInExtended.SkipErrorForSequence(cookie.Id, true);
    }

    public void DestroySubwindowsChecked(uint window)
    {
        var cookie = this.DestroySubwindowsBase(window);
        this._protoInExtended.SkipErrorForSequence(cookie.Id, true);
    }

    public void ChangeSaveSetChecked(ChangeSaveSetMode changeSaveSetMode, uint window)
    {
        var cookie = this.ChangeSaveSetBase(changeSaveSetMode, window);
        this._protoInExtended.SkipErrorForSequence(cookie.Id, true);
    }

    public void ReparentWindowChecked(uint window, uint parent, short x, short y)
    {
        var cookie = this.ReparentWindowBase(window, parent, x, y);
        this._protoInExtended.SkipErrorForSequence(cookie.Id, true);
    }

    public void MapWindowChecked(uint window)
    {
        var cookie = this.MapWindowBase(window);
        this._protoInExtended.SkipErrorForSequence(cookie.Id, true);
    }

    public void MapSubwindowsChecked(uint window)
    {
        var cookie = this.MapSubwindowsBase(window);
        this._protoInExtended.SkipErrorForSequence(cookie.Id, true);
    }

    public void UnmapWindowChecked(uint window)
    {
        var cookie = this.UnmapWindowBase(window);
        this._protoInExtended.SkipErrorForSequence(cookie.Id, true);
    }

    public void UnmapSubwindowsChecked(uint window)
    {
        var cookie = this.UnmapSubwindowsBase(window);
        this._protoInExtended.SkipErrorForSequence(cookie.Id, true);
    }

    public void ConfigureWindowChecked(uint window, ConfigureValueMask mask, Span<uint> args)
    {
        var cookie = this.ConfigureWindowBase(window, mask, args);
        this._protoInExtended.SkipErrorForSequence(cookie.Id, true);
    }

    public void CirculateWindowChecked(Circulate circulate, uint window)
    {
        var cookie = this.CirculateWindowBase(circulate, window);
        this._protoInExtended.SkipErrorForSequence(cookie.Id, true);
    }

    public void ChangePropertyChecked<T>(PropertyMode mode, uint window, ATOM property, ATOM type, Span<T> args)
        where T : struct
#if !NETSTANDARD
        , INumber<T>
#endif
    {
        var cookie = this.ChangePropertyBase(mode, window, property, type, args);
        this._protoInExtended.SkipErrorForSequence(cookie.Id, true);
    }

    public void DeletePropertyChecked(uint window, ATOM atom)
    {
        var cookie = this.DeletePropertyBase(window, atom);
        this._protoInExtended.SkipErrorForSequence(cookie.Id, true);
    }

    public void RotatePropertiesChecked(uint window, ushort delta, Span<ATOM> properties)
    {
        var cookie = this.RotatePropertiesBase(window, delta, properties);
        this._protoInExtended.SkipErrorForSequence(cookie.Id, true);
    }

    public void SetSelectionOwnerChecked(uint owner, ATOM atom, uint timestamp)
    {
        var cookie = this.SetSelectionOwnerBase(owner, atom, timestamp);
        this._protoInExtended.SkipErrorForSequence(cookie.Id, true);
    }

    public void ConvertSelectionChecked(uint requestor, ATOM selection, ATOM target, ATOM property, uint timestamp)
    {
        var cookie = this.ConvertSelectionBase(requestor, selection, target, property, timestamp);
        this._protoInExtended.SkipErrorForSequence(cookie.Id, true);
    }

    public void SendEventChecked(bool propagate, uint destination, uint eventMask, XEvent evnt)
    {
        var cookie = this.SendEventBase(propagate, destination, eventMask, evnt);
        this._protoInExtended.SkipErrorForSequence(cookie.Id, true);
    }

    public void UngrabPointerChecked(uint time)
    {
        var cookie = this.UngrabPointerBase(time);
        this._protoInExtended.SkipErrorForSequence(cookie.Id, true);
    }

    public void GrabButtonChecked(bool ownerEvents, uint grabWindow, ushort mask, GrabMode pointerMode,
        GrabMode keyboardMode, uint confineTo, uint cursor, Button button, ModifierMask modifiers)
    {
        var cookie = this.GrabButtonBase(ownerEvents, grabWindow, mask, pointerMode, keyboardMode, confineTo, cursor,
            button, modifiers);
        this._protoInExtended.SkipErrorForSequence(cookie.Id, true);
    }

    public void UngrabButtonChecked(Button button, uint grabWindow, ModifierMask mask)
    {
        var cookie = this.UngrabButtonBase(button, grabWindow, mask);
        this._protoInExtended.SkipErrorForSequence(cookie.Id, true);
    }

    public void ChangeActivePointerGrabChecked(uint cursor, uint time, ushort mask)
    {
        var cookie = this.ChangeActivePointerGrabBase(cursor, time, mask);
        this._protoInExtended.SkipErrorForSequence(cookie.Id, true);
    }

    public void UngrabKeyboardChecked(uint time)
    {
        var cookie = this.UngrabKeyboardBase(time);
        this._protoInExtended.SkipErrorForSequence(cookie.Id, true);
    }

    public void GrabKeyChecked(bool exposures, uint grabWindow, ModifierMask mask, byte keycode, GrabMode pointerMode,
        GrabMode keyboardMode)
    {
        var cookie = this.GrabKeyBase(exposures, grabWindow, mask, keycode, pointerMode, keyboardMode);
        this._protoInExtended.SkipErrorForSequence(cookie.Id, true);
    }

    public void UngrabKeyChecked(byte key, uint grabWindow, ModifierMask modifier)
    {
        var cookie = this.UngrabKeyBase(key, grabWindow, modifier);
        this._protoInExtended.SkipErrorForSequence(cookie.Id, true);
    }

    public void AllowEventsChecked(EventsMode mode, uint time)
    {
        var cookie = this.AllowEventsBase(mode, time);
        this._protoInExtended.SkipErrorForSequence(cookie.Id, true);
    }

    public void GrabServerChecked()
    {
        var cookie = this.GrabServerBase();
        this._protoInExtended.SkipErrorForSequence(cookie.Id, true);
    }

    public void UngrabServerChecked()
    {
        var cookie = this.UngrabServerBase();
        this._protoInExtended.SkipErrorForSequence(cookie.Id, true);
    }

    public void WarpPointerChecked(uint srcWindow, uint destinationWindow, short srcX, short srcY, ushort srcWidth,
        ushort srcHeight, short destinationX, short destinationY)
    {
        var cookie = this.WarpPointerBase(srcWindow, destinationWindow, srcX, srcY, srcWidth, srcHeight, destinationX,
            destinationY);
        this._protoInExtended.SkipErrorForSequence(cookie.Id, true);
    }

    public void SetInputFocusChecked(InputFocusMode mode, uint focus, uint time)
    {
        var cookie = this.SetInputFocusBase(mode, focus, time);
        this._protoInExtended.SkipErrorForSequence(cookie.Id, true);
    }

    public void OpenFontChecked(string fontName, uint fontId)
    {
        var cookie = this.OpenFontBase(fontName, fontId);
        this._protoInExtended.SkipErrorForSequence(cookie.Id, true);
    }

    public void CloseFontChecked(uint fontId)
    {
        var cookie = this.CloseFontBase(fontId);
        this._protoInExtended.SkipErrorForSequence(cookie.Id, true);
    }

    public void SetFontPathChecked(string[] strPaths)
    {
        var cookie = this.SetFontPathBase(strPaths);
        this._protoInExtended.SkipErrorForSequence(cookie.Id, true);
    }

    public void CreatePixmapChecked(byte depth, uint pixmapId, uint drawable, ushort width, ushort height)
    {
        var cookie = this.CreatePixmapBase(depth, pixmapId, drawable, width, height);
        this._protoInExtended.SkipErrorForSequence(cookie.Id, true);
    }

    public void FreePixmapChecked(uint pixmapId)
    {
        var cookie = this.FreePixmapBase(pixmapId);
        this._protoInExtended.SkipErrorForSequence(cookie.Id, true);
    }

    public void CreateGCChecked(uint gc, uint drawable, GCMask mask, Span<uint> args)
    {
        var cookie = this.CreateGCBase(gc, drawable, mask, args);
        this._protoInExtended.SkipErrorForSequence(cookie.Id, true);
    }

    public void ChangeGCChecked(uint gc, GCMask mask, Span<uint> args)
    {
        var cookie = this.ChangeGCBase(gc, mask, args);
        this._protoInExtended.SkipErrorForSequence(cookie.Id, true);
    }

    public void CopyGCChecked(uint srcGc, uint dstGc, GCMask mask)
    {
        var cookie = this.CopyGCBase(srcGc, dstGc, mask);
        this._protoInExtended.SkipErrorForSequence(cookie.Id, true);
    }

    public void SetDashesChecked(uint gc, ushort dashOffset, Span<byte> dashes)
    {
        var cookie = this.SetDashesBase(gc, dashOffset, dashes);
        this._protoInExtended.SkipErrorForSequence(cookie.Id, true);
    }

    public void SetClipRectanglesChecked(ClipOrdering ordering, uint gc, ushort clipX, ushort clipY,
        Span<Rectangle> rectangles)
    {
        var cookie = this.SetClipRectanglesBase(ordering, gc, clipX, clipY, rectangles);
        this._protoInExtended.SkipErrorForSequence(cookie.Id, true);
    }

    public void FreeGCChecked(uint gc)
    {
        var cookie = this.FreeGCBase(gc);
        this._protoInExtended.SkipErrorForSequence(cookie.Id, true);
    }

    public void ClearAreaChecked(bool exposures, uint window, short x, short y, ushort width, ushort height)
    {
        var cookie = this.ClearAreaBase(exposures, window, x, y, width, height);
        this._protoInExtended.SkipErrorForSequence(cookie.Id, true);
    }

    public void CopyAreaChecked(uint srcDrawable, uint destinationDrawable, uint gc, ushort srcX, ushort srcY,
        ushort destinationX, ushort destinationY, ushort width, ushort height)
    {
        var cookie = this.CopyAreaBase(srcDrawable, destinationDrawable, gc, srcX, srcY, destinationX, destinationY,
            width, height);
        this._protoInExtended.SkipErrorForSequence(cookie.Id, true);
    }

    public void CopyPlaneChecked(uint srcDrawable, uint destinationDrawable, uint gc, ushort srcX, ushort srcY,
        ushort destinationX, ushort destinationY, ushort width, ushort height, uint bitPlane)
    {
        var cookie = this.CopyPlaneBase(srcDrawable, destinationDrawable, gc, srcX, srcY, destinationX, destinationY,
            width, height, bitPlane);
        this._protoInExtended.SkipErrorForSequence(cookie.Id, true);
    }

    public void PolyPointChecked(CoordinateMode coordinate, uint drawable, uint gc, Span<Point> points)
    {
        var cookie = this.PolyPointBase(coordinate, drawable, gc, points);
        this._protoInExtended.SkipErrorForSequence(cookie.Id, true);
    }

    public void PolyLineChecked(CoordinateMode coordinate, uint drawable, uint gc, Span<Point> points)
    {
        var cookie = this.PolyLineBase(coordinate, drawable, gc, points);
        this._protoInExtended.SkipErrorForSequence(cookie.Id, true);
    }

    public void PolySegmentChecked(uint drawable, uint gc, Span<Segment> segments)
    {
        var cookie = this.PolySegmentBase(drawable, gc, segments);
        this._protoInExtended.SkipErrorForSequence(cookie.Id, true);
    }

    public void PolyRectangleChecked(uint drawable, uint gc, Span<Rectangle> rectangles)
    {
        var cookie = this.PolyRectangleBase(drawable, gc, rectangles);
        this._protoInExtended.SkipErrorForSequence(cookie.Id, true);
    }

    public void PolyArcChecked(uint drawable, uint gc, Span<Arc> arcs)
    {
        var cookie = this.PolyArcBase(drawable, gc, arcs);
        this._protoInExtended.SkipErrorForSequence(cookie.Id, true);
    }

    public void FillPolyChecked(uint drawable, uint gc, PolyShape shape, CoordinateMode coordinate, Span<Point> points)
    {
        var cookie = this.FillPolyBase(drawable, gc, shape, coordinate, points);
        this._protoInExtended.SkipErrorForSequence(cookie.Id, true);
    }

    public void PolyFillRectangleChecked(uint drawable, uint gc, Span<Rectangle> rectangles)
    {
        var cookie = this.PolyFillRectangleBase(drawable, gc, rectangles);
        this._protoInExtended.SkipErrorForSequence(cookie.Id, true);
    }

    public void PolyFillArcChecked(uint drawable, uint gc, Span<Arc> arcs)
    {
        var cookie = this.PolyFillArcBase(drawable, gc, arcs);
        this._protoInExtended.SkipErrorForSequence(cookie.Id, true);
    }

    public void PutImageChecked(ImageFormatBitmap format, uint drawable, uint gc, ushort width, ushort height, short x,
        short y, byte leftPad, byte depth, Span<byte> data)
    {
        var cookie = this.PutImageBase(format, drawable, gc, width, height, x, y, leftPad, depth, data);
        this._protoInExtended.SkipErrorForSequence(cookie.Id, true);
    }

    public void ImageText8Checked(uint drawable, uint gc, short x, short y, ReadOnlySpan<byte> text)
    {
        var cookie = this.ImageText8Base(drawable, gc, x, y, text);
        this._protoInExtended.SkipErrorForSequence(cookie.Id, true);
    }

    public void ImageText16Checked(uint drawable, uint gc, short x, short y, ReadOnlySpan<char> text)
    {
        var cookie = this.ImageText16Base(drawable, gc, x, y, text);
        this._protoInExtended.SkipErrorForSequence(cookie.Id, true);
    }

    public void CreateColormapChecked(ColormapAlloc alloc, uint colormapId, uint window, uint visual)
    {
        var cookie = this.CreateColormapBase(alloc, colormapId, window, visual);
        this._protoInExtended.SkipErrorForSequence(cookie.Id, true);
    }

    public void FreeColormapChecked(uint colormapId)
    {
        var cookie = this.FreeColormapBase(colormapId);
        this._protoInExtended.SkipErrorForSequence(cookie.Id, true);
    }

    public void CopyColormapAndFreeChecked(uint colormapId, uint srcColormapId)
    {
        var cookie = this.CopyColormapAndFreeBase(colormapId, srcColormapId);
        this._protoInExtended.SkipErrorForSequence(cookie.Id, true);
    }

    public void InstallColormapChecked(uint colormapId)
    {
        var cookie = this.InstallColormapBase(colormapId);
        this._protoInExtended.SkipErrorForSequence(cookie.Id, true);
    }

    public void UninstallColormapChecked(uint colormapId)
    {
        var cookie = this.UninstallColormapBase(colormapId);
        this._protoInExtended.SkipErrorForSequence(cookie.Id, true);
    }

    public void FreeColorsChecked(uint colormapId, uint planeMask, Span<uint> pixels)
    {
        var cookie = this.FreeColorsBase(colormapId, planeMask, pixels);
        this._protoInExtended.SkipErrorForSequence(cookie.Id, true);
    }

    public void StoreColorsChecked(uint colormapId, Span<ColorItem> item)
    {
        var cookie = this.StoreColorsBase(colormapId, item);
        this._protoInExtended.SkipErrorForSequence(cookie.Id, true);
    }

    public void StoreNamedColorChecked(ColorFlag mode, uint colormapId, uint pixels, ReadOnlySpan<byte> name)
    {
        var cookie = this.StoreNamedColorBase(mode, colormapId, pixels, name);
        this._protoInExtended.SkipErrorForSequence(cookie.Id, true);
    }

    public void CreateCursorChecked(uint cursorId, uint source, uint mask, ushort foreRed, ushort foreGreen,
        ushort foreBlue, ushort backRed, ushort backGreen, ushort backBlue, ushort x, ushort y)
    {
        var cookie = this.CreateCursorBase(cursorId, source, mask, foreRed, foreGreen, foreBlue, backRed, backGreen,
            backBlue, x, y);
        this._protoInExtended.SkipErrorForSequence(cookie.Id, true);
    }

    public void CreateGlyphCursorChecked(uint cursorId, uint sourceFont, uint fontMask, char sourceChar,
        ushort charMask, ushort foreRed, ushort foreGreen, ushort foreBlue, ushort backRed, ushort backGreen,
        ushort backBlue)
    {
        var cookie = this.CreateGlyphCursorBase(cursorId, sourceFont, fontMask, sourceChar, charMask, foreRed,
            foreGreen, foreBlue, backRed, backGreen, backBlue);
        this._protoInExtended.SkipErrorForSequence(cookie.Id, true);
    }

    public void FreeCursorChecked(uint cursorId)
    {
        var cookie = this.FreeCursorBase(cursorId);
        this._protoInExtended.SkipErrorForSequence(cookie.Id, true);
    }

    public void RecolorCursorChecked(uint cursorId, ushort foreRed, ushort foreGreen, ushort foreBlue, ushort backRed,
        ushort backGreen, ushort backBlue)
    {
        var cookie = this.RecolorCursorBase(cursorId, foreRed, foreGreen, foreBlue, backRed, backGreen, backBlue);
        this._protoInExtended.SkipErrorForSequence(cookie.Id, true);
    }

    public void ChangeKeyboardMappingChecked(byte keycodeCount, byte firstKeycode, byte keysymsPerKeycode,
        Span<uint> keysym)
    {
        var cookie = this.ChangeKeyboardMappingBase(keycodeCount, firstKeycode, keysymsPerKeycode, keysym);
        this._protoInExtended.SkipErrorForSequence(cookie.Id, true);
    }

    public void BellChecked(sbyte percent)
    {
        var cookie = this.BellBase(percent);
        this._protoInExtended.SkipErrorForSequence(cookie.Id, true);
    }

    public void ChangeKeyboardControlChecked(KeyboardControlMask mask, Span<uint> args)
    {
        var cookie = this.ChangeKeyboardControlBase(mask, args);
        this._protoInExtended.SkipErrorForSequence(cookie.Id, true);
    }

    public void ChangePointerControlChecked(Acceleration? acceleration, ushort? threshold)
    {
        var cookie = this.ChangePointerControlBase(acceleration, threshold);
        this._protoInExtended.SkipErrorForSequence(cookie.Id, true);
    }

    public void SetScreenSaverChecked(short timeout, short interval, TriState preferBlanking, TriState allowExposures)
    {
        var cookie = this.SetScreenSaverBase(timeout, interval, preferBlanking, allowExposures);
        this._protoInExtended.SkipErrorForSequence(cookie.Id, true);
    }

    public void ForceScreenSaverChecked(ForceScreenSaverMode mode)
    {
        var cookie = this.ForceScreenSaverBase(mode);
        this._protoInExtended.SkipErrorForSequence(cookie.Id, true);
    }

    public void ChangeHostsChecked(HostMode mode, Family family, Span<byte> address)
    {
        var cookie = this.ChangeHostsBase(mode, family, address);
        this._protoInExtended.SkipErrorForSequence(cookie.Id, true);
    }

    public void SetAccessControlChecked(AccessControlMode mode)
    {
        var cookie = this.SetAccessControlBase(mode);
        this._protoInExtended.SkipErrorForSequence(cookie.Id, true);
    }

    public void SetCloseDownModeChecked(CloseDownMode mode)
    {
        var cookie = this.SetCloseDownModeBase(mode);
        this._protoInExtended.SkipErrorForSequence(cookie.Id, true);
    }

    public void KillClientChecked(uint resource)
    {
        var cookie = this.KillClientBase(resource);
        this._protoInExtended.SkipErrorForSequence(cookie.Id, true);
    }

    public void NoOperationChecked(Span<uint> args)
    {
        var cookie = this.NoOperationBase(args);
        this._protoInExtended.SkipErrorForSequence(cookie.Id, true);
    }

    public void PolyText8Checked(uint drawable, uint gc, ushort x, ushort y, TextItem8[] data)
    {
        var cookie = this.PolyText8Base(drawable, gc, x, y, data);
        this._protoInExtended.SkipErrorForSequence(cookie.Id, true);
    }

    public void PolyText16Checked(uint drawable, uint gc, ushort x, ushort y, TextItem16[] data)
    {
        var cookie = this.PolyText16Base(drawable, gc, x, y, data);
        this._protoInExtended.SkipErrorForSequence(cookie.Id, true);
    }

    private ResponseProto ChangeWindowAttributesBase(uint window, ValueMask mask, Span<uint> args)
    {
        if (mask.CountFlags() != args.Length)
            throw new InsufficientDataException(mask.CountFlags(), args.Length, nameof(mask), nameof(args));

        var request = new ChangeWindowAttributesType(window, mask, args.Length);
        var requiredBuffer = request.Length * 4;
        if (requiredBuffer < _bigRequestLength)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
            scratchBuffer.WriteRequest(
                ref request,
                12,
                MemoryMarshal.Cast<uint, byte>(args));
            _protoOutExtended.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            var workingBuffer = scratchBuffer[..requiredBuffer];
            workingBuffer.WriteRequest(
                ref request,
                12,
                MemoryMarshal.Cast<uint, byte>(args));
            _protoOutExtended.SendExact(workingBuffer);
        }

        return new ResponseProto(_protoOutExtended.Sequence);
    }

    private ResponseProto DestroyWindowBase(uint window)
    {
        var request = new DestroyWindowType(window);
        _protoOutExtended.Send(ref request);
        return new ResponseProto(_protoOutExtended.Sequence);
    }

    private ResponseProto AllowEventsBase(EventsMode mode, uint time)
    {
        var request = new AllowEventsType(mode, time);
        _protoOutExtended.Send(ref request);
        return new ResponseProto(_protoOutExtended.Sequence);
    }

    private ResponseProto BellBase(sbyte percent)
    {
        if (percent is > 100 or < -100)
            throw new ArgumentOutOfRangeException(nameof(percent), "value must be between -100 to 100");

        var request = new BellType(percent);
        _protoOutExtended.Send(ref request);
        return new ResponseProto(_protoOutExtended.Sequence);
    }

    private ResponseProto ChangeActivePointerGrabBase(uint cursor, uint time, ushort mask)
    {
        var request = new ChangeActivePointerGrabType(cursor, time, mask);
        _protoOutExtended.Send(ref request);
        return new ResponseProto(_protoOutExtended.Sequence);
    }

    private ResponseProto ChangeGCBase(uint gc, GCMask mask, Span<uint> args)
    {
        if (mask.CountFlags() != args.Length)
            throw new InsufficientDataException(mask.CountFlags(), args.Length, nameof(mask), nameof(args));

        var request = new ChangeGCType(gc, mask, args.Length);
        var requiredBuffer = request.Length * 4;
        if (requiredBuffer < _bigRequestLength)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
            scratchBuffer.WriteRequest(
                ref request,
                12,
                MemoryMarshal.Cast<uint, byte>(args));
            _protoOutExtended.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            var workingBuffer = scratchBuffer[..requiredBuffer];
            workingBuffer.WriteRequest(ref request,
                12,
                MemoryMarshal.Cast<uint, byte>(args));
            _protoOutExtended.SendExact(workingBuffer);
        }

        return new ResponseProto(_protoOutExtended.Sequence);
    }

    private ResponseProto ChangeHostsBase(HostMode mode, Family family, Span<byte> address)
    {
        var request = new ChangeHostsType(mode, family, address.Length);
        var requiredBuffer = request.Length * 4;
        if (requiredBuffer < _bigRequestLength)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
            scratchBuffer.WriteRequest(
                ref request,
                8,
                address);
            _protoOutExtended.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            var workingBuffer = scratchBuffer[..requiredBuffer];
            workingBuffer.WriteRequest(
                ref request,
                8,
                address);
        }

        return new ResponseProto(_protoOutExtended.Sequence);
    }

    private ResponseProto ChangeKeyboardControlBase(KeyboardControlMask mask, Span<uint> args)
    {
        var request = new ChangeKeyboardControlType(mask, args.Length);
        var requiredBuffer = request.Length * 4;
        if (requiredBuffer < _bigRequestLength)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
            scratchBuffer.WriteRequest(
                ref request,
                8,
                MemoryMarshal.Cast<uint, byte>(args));
            _protoOutExtended.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            var workingBuffer = scratchBuffer[..requiredBuffer];
            workingBuffer.WriteRequest(
                ref request,
                8,
                MemoryMarshal.Cast<uint, byte>(args));
        }

        return new ResponseProto(_protoOutExtended.Sequence);
    }

    private ResponseProto ChangeKeyboardMappingBase(byte keycodeCount, byte firstKeycode, byte keysymsPerKeycode,
        Span<uint> keysym)
    {
        var request = new ChangeKeyboardMappingType(keycodeCount, firstKeycode, keysymsPerKeycode);
        var requiredBuffer = request.Length * 4;
        if (requiredBuffer < _bigRequestLength)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
            scratchBuffer.WriteRequest(
                ref request,
                8,
                MemoryMarshal.Cast<uint, byte>(keysym));
            _protoOutExtended.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            var workingBuffer = scratchBuffer[..requiredBuffer];
            workingBuffer.WriteRequest(
                ref request,
                8,
                MemoryMarshal.Cast<uint, byte>(keysym));
        }

        return new ResponseProto(_protoOutExtended.Sequence);
    }

    private ResponseProto ChangePointerControlBase(Acceleration? acceleration, ushort? threshold)
    {
        var request = new ChangePointerControlType(acceleration?.Numerator ?? 0, acceleration?.Denominator ?? 0,
            threshold ?? 0, (byte)(acceleration is null ? 0 : 1), (byte)(threshold.HasValue ? 1 : 0));
        _protoOutExtended.Send(ref request);
        return new ResponseProto(_protoOutExtended.Sequence);
    }

    private ResponseProto ChangePropertyBase<T>(PropertyMode mode, uint window, ATOM property, ATOM type,
        Span<T> args)
        where T : struct
#if !NETSTANDARD
            , INumber<T>
#endif
    {
        var size = Marshal.SizeOf<T>();
        if (size is not 1 and not 2 and not 4)
            throw new ArgumentException("type must be byte, sbyte, short, ushort, int, uint");
        var request = new ChangePropertyType(mode, window, property, type, args.Length, size);
        var requiredBuffer = request.Length * 4;
        if (requiredBuffer < _bigRequestLength)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
            scratchBuffer.WriteRequest(
                ref request,
                24,
                MemoryMarshal.Cast<T, byte>(args));
            _protoOutExtended.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            var workingBuffer = scratchBuffer[..requiredBuffer];
            workingBuffer.WriteRequest(
                ref request,
                24,
                MemoryMarshal.Cast<T, byte>(args));
        }

        return new ResponseProto(_protoOutExtended.Sequence);
    }

    private ResponseProto ChangeSaveSetBase(ChangeSaveSetMode changeSaveSetMode, uint window)
    {
        var request = new ChangeSaveSetType(changeSaveSetMode, window);
        _protoOutExtended.Send(ref request);
        return new ResponseProto(_protoOutExtended.Sequence);
    }

    private ResponseProto CirculateWindowBase(Circulate circulate, uint window)
    {
        var request = new CirculateWindowType(circulate, window);
        _protoOutExtended.Send(ref request);
        return new ResponseProto(_protoOutExtended.Sequence);
    }

    private ResponseProto ClearAreaBase(bool exposures, uint window, short x, short y, ushort width, ushort height)
    {
        var request = new ClearAreaType(exposures, window, x, y, width, height);
        _protoOutExtended.Send(ref request);
        return new ResponseProto(_protoOutExtended.Sequence);
    }

    private ResponseProto CloseFontBase(uint fontId)
    {
        var request = new CloseFontType(fontId);
        _protoOutExtended.Send(ref request);
        return new ResponseProto(_protoOutExtended.Sequence);
    }

    private ResponseProto ConfigureWindowBase(uint window, ConfigureValueMask mask, Span<uint> args)
    {
        if (mask.CountFlags() != args.Length)
            throw new InsufficientDataException(mask.CountFlags(), args.Length, nameof(mask), nameof(args));

        var request = new ConfigureWindowType(window, mask, args.Length);
        var requiredBuffer = request.Length * 4;
        if (requiredBuffer < _bigRequestLength)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
            scratchBuffer.WriteRequest(
                ref request,
                12,
                MemoryMarshal.Cast<uint, byte>(args));
            _protoOutExtended.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            var workingBuffer = scratchBuffer[..requiredBuffer];
            workingBuffer.WriteRequest(
                ref request,
                12,
                MemoryMarshal.Cast<uint, byte>(args));
        }

        return new ResponseProto(_protoOutExtended.Sequence);
    }

    private ResponseProto ConvertSelectionBase(uint requestor, ATOM selection, ATOM target, ATOM property,
        uint timestamp)
    {
        var request = new ConvertSelectionType(requestor, selection, target, property, timestamp);
        _protoOutExtended.Send(ref request);
        return new ResponseProto(_protoOutExtended.Sequence);
    }

    private ResponseProto CopyAreaBase(uint srcDrawable, uint destinationDrawable, uint gc, ushort srcX, ushort srcY,
        ushort destinationX, ushort destinationY, ushort width, ushort height)
    {
        var request = new CopyAreaType(srcDrawable, destinationDrawable, gc, srcX, srcY, destinationX, destinationY,
            width, height);
        _protoOutExtended.Send(ref request);
        return new ResponseProto(_protoOutExtended.Sequence);
    }

    private ResponseProto CopyColormapAndFreeBase(uint colormapId, uint srcColormapId)
    {
        var request = new CopyColormapAndFreeType(colormapId, srcColormapId);
        _protoOutExtended.Send(ref request);
        return new ResponseProto(_protoOutExtended.Sequence);
    }

    private ResponseProto CopyGCBase(uint srcGc, uint dstGc, GCMask mask)
    {
        var request = new CopyGCType(srcGc, dstGc, mask);
        _protoOutExtended.Send(ref request);
        return new ResponseProto(_protoOutExtended.Sequence);
    }

    private ResponseProto CopyPlaneBase(uint srcDrawable, uint destinationDrawable, uint gc, ushort srcX, ushort srcY,
        ushort destinationX, ushort destinationY, ushort width, ushort height, uint bitPlane)
    {
        var request = new CopyPlaneType(srcDrawable, destinationDrawable, gc, srcX, srcY, destinationX, destinationY,
            width, height, bitPlane);
        _protoOutExtended.Send(ref request);
        return new ResponseProto(_protoOutExtended.Sequence);
    }

    private ResponseProto CreateColormapBase(ColormapAlloc alloc, uint colormapId, uint window, uint visual)
    {
        var request = new CreateColormapType(alloc, colormapId, window, visual);
        _protoOutExtended.Send(ref request);
        return new ResponseProto(_protoOutExtended.Sequence);
    }

    private ResponseProto CreateCursorBase(uint cursorId, uint source, uint mask, ushort foreRed, ushort foreGreen,
        ushort foreBlue,
        ushort backRed, ushort backGreen, ushort backBlue, ushort x, ushort y)
    {
        var request = new CreateCursorType(cursorId, source, mask, foreRed, foreGreen, foreBlue, backRed, backGreen,
            backBlue, x, y);
        _protoOutExtended.Send(ref request);
        return new ResponseProto(_protoOutExtended.Sequence);
    }

    private ResponseProto CreateGCBase(uint gc, uint drawable, GCMask mask, Span<uint> args)
    {

        if (mask.CountFlags() != args.Length)
            throw new InsufficientDataException(mask.CountFlags(), args.Length, nameof(mask), nameof(args));

        var request = new CreateGCType(gc, drawable, mask, args.Length);
        var requiredBuffer = request.Length * 4;
        if (requiredBuffer < _bigRequestLength)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
            scratchBuffer.WriteRequest(
                ref request,
                16,
                MemoryMarshal.Cast<uint, byte>(args));
            _protoOutExtended.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            var workingBuffer = scratchBuffer[..requiredBuffer];
            workingBuffer.WriteRequest(
                ref request,
                16,
                MemoryMarshal.Cast<uint, byte>(args));
        }

        return new ResponseProto(_protoOutExtended.Sequence);
    }

    private ResponseProto CreateGlyphCursorBase(uint cursorId, uint sourceFont, uint fontMask, char sourceChar,
        ushort charMask,
        ushort foreRed, ushort foreGreen, ushort foreBlue, ushort backRed, ushort backGreen, ushort backBlue)
    {
        var request = new CreateGlyphCursorType(cursorId, sourceFont, fontMask, sourceChar, charMask, foreRed,
            foreGreen, foreBlue, backRed, backGreen, backBlue);
        _protoOutExtended.Send(ref request);
        return new ResponseProto(_protoOutExtended.Sequence);
    }

    private ResponseProto CreatePixmapBase(byte depth, uint pixmapId, uint drawable, ushort width, ushort height)
    {
        var request = new CreatePixmapType(depth, pixmapId, drawable, width, height);
        _protoOutExtended.Send(ref request);
        return new ResponseProto(_protoOutExtended.Sequence);
    }

    private ResponseProto CreateWindowBase(byte depth, uint window, uint parent, short x, short y, ushort width,
        ushort height,
        ushort borderWidth, ClassType classType, uint rootVisualId, ValueMask mask, Span<uint> args)
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
            _protoOutExtended.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            var workingBuffer = scratchBuffer[..requiredBuffer];
            workingBuffer.WriteRequest(
                ref request,
                32,
                MemoryMarshal.Cast<uint, byte>(args));
        }

        return new ResponseProto(_protoOutExtended.Sequence);
    }

    private ResponseProto DeletePropertyBase(uint window, ATOM atom)
    {
        var request = new DeletePropertyType(window, atom);
        _protoOutExtended.Send(ref request);
        return new ResponseProto(_protoOutExtended.Sequence);
    }

    private ResponseProto DestroySubwindowsBase(uint window)
    {
        var request = new DestroySubWindowsType(window);
        _protoOutExtended.Send(ref request);
        return new ResponseProto(_protoOutExtended.Sequence);
    }

    private ResponseProto FillPolyBase(uint drawable, uint gc, PolyShape shape, CoordinateMode coordinate,
        Span<Point> points)
    {
        var request = new FillPolyType(drawable, gc, shape, coordinate, points.Length);
        var requiredBuffer = request.Length * 4;
        if (requiredBuffer < _bigRequestLength)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
            scratchBuffer.WriteRequest(
                ref request,
                16,
                MemoryMarshal.Cast<Point, byte>(points));
            _protoOutExtended.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            var workingBuffer = scratchBuffer[..requiredBuffer];
            workingBuffer.WriteRequest(
                ref request,
                16,
                MemoryMarshal.Cast<Point, byte>(points));
        }

        return new ResponseProto(_protoOutExtended.Sequence);
    }

    private ResponseProto ForceScreenSaverBase(ForceScreenSaverMode mode)
    {
        var request = new ForceScreenSaverType(mode);
        _protoOutExtended.Send(ref request);
        return new ResponseProto(_protoOutExtended.Sequence);
    }

    private ResponseProto FreeColormapBase(uint colormapId)
    {
        var request = new FreeColormapType(colormapId);
        _protoOutExtended.Send(ref request);
        return new ResponseProto(_protoOutExtended.Sequence);
    }

    private ResponseProto FreeColorsBase(uint colormapId, uint planeMask, Span<uint> pixels)
    {
        var request = new FreeColorsType(colormapId, planeMask, pixels.Length);
        var requiredBuffer = request.Length * 4;
        if (requiredBuffer < _bigRequestLength)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
            scratchBuffer.WriteRequest(
                ref request,
                12,
                MemoryMarshal.Cast<uint, byte>(pixels));
            _protoOutExtended.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            var workingBuffer = scratchBuffer[..requiredBuffer];
            workingBuffer.WriteRequest(
                ref request,
                12,
                MemoryMarshal.Cast<uint, byte>(pixels));
        }

        return new ResponseProto(_protoOutExtended.Sequence);
    }

    private ResponseProto FreeCursorBase(uint cursorId)
    {
        var request = new FreeCursorType(cursorId);
        _protoOutExtended.Send(ref request);
        return new ResponseProto(_protoOutExtended.Sequence);
    }

    private ResponseProto FreeGCBase(uint gc)
    {
        var request = new FreeGCType(gc);
        _protoOutExtended.Send(ref request);
        return new ResponseProto(_protoOutExtended.Sequence);
    }

    private ResponseProto FreePixmapBase(uint pixmapId)
    {
        var request = new FreePixmapType(pixmapId);
        _protoOutExtended.Send(ref request);
        return new ResponseProto(_protoOutExtended.Sequence);
    }

    private ResponseProto GrabButtonBase(bool ownerEvents, uint grabWindow, ushort mask, GrabMode pointerMode,
        GrabMode keyboardMode,
        uint confineTo, uint cursor, Button button, ModifierMask modifiers)
    {
        var request = new GrabButtonType(ownerEvents, grabWindow, mask, pointerMode, keyboardMode, confineTo, cursor,
            button, modifiers);
        _protoOutExtended.Send(ref request);
        return new ResponseProto(_protoOutExtended.Sequence);
    }

    private ResponseProto GrabKeyBase(bool exposures, uint grabWindow, ModifierMask mask, byte keycode,
        GrabMode pointerMode,
        GrabMode keyboardMode)
    {
        var request = new GrabKeyType(exposures, grabWindow, mask, keycode, pointerMode, keyboardMode);
        _protoOutExtended.Send(ref request);
        return new ResponseProto(_protoOutExtended.Sequence);
    }

    private ResponseProto GrabServerBase()
    {
        var request = new GrabServerType();
        _protoOutExtended.Send(ref request);
        return new ResponseProto(_protoOutExtended.Sequence);
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
            _protoOutExtended.SendExact(scratchBuffer);
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
            _protoOutExtended.SendExact(scratchBuffer[..requiredBuffer]);
        }

        return new ResponseProto(_protoOutExtended.Sequence);
    }

    private ResponseProto ImageText8Base(uint drawable, uint gc, short x, short y, ReadOnlySpan<byte> text)
    {
        var request = new ImageText8Type(drawable, gc, x, y, text.Length);
        var requiredBuffer = request.Length * 4;
        if (requiredBuffer < _bigRequestLength)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
            scratchBuffer.WriteRequest(
                ref request,
                16,
                text
            );
            _protoOutExtended.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            var workingBuffer = scratchBuffer[..requiredBuffer];
            workingBuffer.WriteRequest(
                ref request,
                16,
                text
            );
        }

        return new ResponseProto(_protoOutExtended.Sequence);
    }

    private ResponseProto InstallColormapBase(uint colormapId)
    {
        var request = new InstallColormapType(colormapId);
        _protoOutExtended.Send(ref request);
        return new ResponseProto(_protoOutExtended.Sequence);
    }

    private ResponseProto KillClientBase(uint resource)
    {
        var request = new KillClientType(resource);
        _protoOutExtended.Send(ref request);
        return new ResponseProto(_protoOutExtended.Sequence);
    }

    private ResponseProto MapSubwindowsBase(uint window)
    {
        var request = new MapSubWindowsType(window);
        _protoOutExtended.Send(ref request);
        return new ResponseProto(_protoOutExtended.Sequence);
    }

    private ResponseProto MapWindowBase(uint window)
    {
        var request = new MapWindowType(window);
        _protoOutExtended.Send(ref request);
        return new ResponseProto(_protoOutExtended.Sequence);
    }

    private ResponseProto NoOperationBase(Span<uint> args)
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
            _protoOutExtended.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            var workingBuffer = scratchBuffer[..requiredBuffer];
            workingBuffer.WriteRequest(
                ref request,
                4,
                MemoryMarshal.Cast<uint, byte>(args));
        }

        return new ResponseProto(_protoOutExtended.Sequence);
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
            _protoOutExtended.SendExact(scratchBuffer);
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
            _protoOutExtended.SendExact(scratchBuffer[..requiredBuffer]);
        }

        return new ResponseProto(_protoOutExtended.Sequence);
    }

    private ResponseProto PolyArcBase(uint drawable, uint gc, Span<Arc> arcs)
    {
        var request = new PolyArcType(drawable, gc, arcs.Length);
        var requiredBuffer = request.Length * 4;
        if (requiredBuffer < _bigRequestLength)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
            scratchBuffer.WriteRequest(
                ref request,
                12,
                MemoryMarshal.Cast<Arc, byte>(arcs));
            _protoOutExtended.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            var workingBuffer = scratchBuffer[..requiredBuffer];
            workingBuffer.WriteRequest(
                ref request,
                12,
                MemoryMarshal.Cast<Arc, byte>(arcs));
        }

        return new ResponseProto(_protoOutExtended.Sequence);
    }

    private ResponseProto PolyFillArcBase(uint drawable, uint gc, Span<Arc> arcs)
    {
        var request = new PolyFillArcType(drawable, gc, arcs.Length);
        var requiredBuffer = request.Length * 4;
        if (requiredBuffer < _bigRequestLength)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
            scratchBuffer.WriteRequest(
                ref request,
                12,
                MemoryMarshal.Cast<Arc, byte>(arcs));
            _protoOutExtended.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            var workingBuffer = scratchBuffer[..requiredBuffer];
            workingBuffer.WriteRequest(
                ref request,
                12,
                MemoryMarshal.Cast<Arc, byte>(arcs));
        }

        return new ResponseProto(_protoOutExtended.Sequence);
    }

    private ResponseProto PolyFillRectangleBase(uint drawable, uint gc, Span<Rectangle> rectangles)
    {
        var request = new PolyFillRectangleType(drawable, gc, rectangles.Length);
        var requiredBuffer = request.Length * 4;
        if (requiredBuffer < _bigRequestLength)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
            scratchBuffer.WriteRequest(
                ref request,
                12,
                MemoryMarshal.Cast<Rectangle, byte>(rectangles));
            _protoOutExtended.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            var workingBuffer = scratchBuffer[..requiredBuffer];
            workingBuffer.WriteRequest(
                ref request,
                12,
                MemoryMarshal.Cast<Rectangle, byte>(rectangles));
        }

        return new ResponseProto(_protoOutExtended.Sequence);
    }

    private ResponseProto PolyLineBase(CoordinateMode coordinate, uint drawable, uint gc, Span<Point> points)
    {
        var request = new PolyLineType(coordinate, drawable, gc, points.Length);
        var requiredBuffer = request.Length * 4;
        if (requiredBuffer < _bigRequestLength)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
            scratchBuffer.WriteRequest(
                ref request,
                12,
                MemoryMarshal.Cast<Point, byte>(points));
            _protoOutExtended.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            var workingBuffer = scratchBuffer[..requiredBuffer];
            workingBuffer.WriteRequest(
                ref request,
                12,
                MemoryMarshal.Cast<Point, byte>(points));
        }

        return new ResponseProto(_protoOutExtended.Sequence);
    }

    private ResponseProto PolyPointBase(CoordinateMode coordinate, uint drawable, uint gc, Span<Point> points)
    {
        var request = new PolyPointType(coordinate, drawable, gc, points.Length);
        var requiredBuffer = request.Length * 4;
        if (requiredBuffer < _bigRequestLength)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
            scratchBuffer.WriteRequest(
                ref request,
                12,
                MemoryMarshal.Cast<Point, byte>(points));
            _protoOutExtended.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            var workingBuffer = scratchBuffer[..requiredBuffer];
            workingBuffer.WriteRequest(
                ref request,
                12,
                MemoryMarshal.Cast<Point, byte>(points));
        }

        return new ResponseProto(_protoOutExtended.Sequence);
    }

    private ResponseProto PolyRectangleBase(uint drawable, uint gc, Span<Rectangle> rectangles)
    {
        var request = new PolyRectangleType(drawable, gc, rectangles.Length);
        var requiredBuffer = request.Length * 4;
        if (requiredBuffer < _bigRequestLength)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
            scratchBuffer.WriteRequest(
                ref request,
                12,
                MemoryMarshal.Cast<Rectangle, byte>(rectangles));
            _protoOutExtended.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            var workingBuffer = scratchBuffer[..requiredBuffer];
            workingBuffer.WriteRequest(
                ref request,
                12,
                MemoryMarshal.Cast<Rectangle, byte>(rectangles));
        }

        return new ResponseProto(_protoOutExtended.Sequence);
    }

    private ResponseProto PolySegmentBase(uint drawable, uint gc, Span<Segment> segments)
    {
        var request = new PolySegmentType(drawable, gc, segments.Length);
        var requiredBuffer = request.Length * 4;
        if (requiredBuffer < _bigRequestLength)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
            scratchBuffer.WriteRequest(
                ref request,
                12,
                MemoryMarshal.Cast<Segment, byte>(segments));
            _protoOutExtended.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            var workingBuffer = scratchBuffer[..requiredBuffer];
            workingBuffer.WriteRequest(
                ref request,
                12,
                MemoryMarshal.Cast<Segment, byte>(segments));
        }

        return new ResponseProto(_protoOutExtended.Sequence);
    }

    private ResponseProto PolyText16Base(uint drawable, uint gc, ushort x, ushort y, TextItem16[] data)
    {
        var request = new PolyText16Type(drawable, gc, x, y, data.Sum(a => a.Count));
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

            scratchBuffer[^data.Sum(a => a.Count).Padding()..].Clear();
            _protoOutExtended.SendExact(scratchBuffer);
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

            workingBuffer[^data.Sum(a => a.Count).Padding()..].Clear();
            _protoOutExtended.SendExact(workingBuffer);
        }

        return new ResponseProto(_protoOutExtended.Sequence);
    }

    private ResponseProto PolyText8Base(uint drawable, uint gc, ushort x, ushort y, TextItem8[] data)
    {
        var request = new PolyText8Type(drawable, gc, x, y, data.Sum(a => a.Count));
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
            scratchBuffer[^data.Sum(a => a.Count).Padding()..].Clear();
            _protoOutExtended.SendExact(scratchBuffer);
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
            workingBuffer[^data.Sum(a => a.Count).Padding()..].Clear();
            _protoOutExtended.SendExact(workingBuffer);
        }

        return new ResponseProto(_protoOutExtended.Sequence);
    }

    private ResponseProto PutImageBase(ImageFormatBitmap format, uint drawable, uint gc, ushort width, ushort height,
        short x,
        short y,
        byte leftPad, byte depth, Span<byte> data)
    {
        var request = new PutImageType(format, drawable, gc, width, height, x, y, leftPad, depth, data.Length);
        var requiredBuffer = request.Length * 4;
        if (requiredBuffer < _bigRequestLength)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
            scratchBuffer.WriteRequest(
                ref request,
                24,
                data);
            _protoOutExtended.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            var workingBuffer = scratchBuffer[..requiredBuffer];
            workingBuffer.WriteRequest(
                ref request,
                24,
                data);
        }

        return new ResponseProto(_protoOutExtended.Sequence);
    }

    private ResponseProto RecolorCursorBase(uint cursorId, ushort foreRed, ushort foreGreen, ushort foreBlue,
        ushort backRed,
        ushort backGreen, ushort backBlue)
    {
        var request = new RecolorCursorType(cursorId, foreRed, foreGreen, foreBlue, backRed, backGreen, backBlue);
        _protoOutExtended.Send(ref request);
        return new ResponseProto(_protoOutExtended.Sequence);
    }

    private ResponseProto ReparentWindowBase(uint window, uint parent, short x, short y)
    {
        var request = new ReparentWindowType(window, parent, x, y);
        _protoOutExtended.Send(ref request);
        return new ResponseProto(_protoOutExtended.Sequence);
    }

    private ResponseProto RotatePropertiesBase(uint window, ushort delta, Span<ATOM> properties)
    {
        var request = new RotatePropertiesType(window, properties.Length, delta);
        var requiredBuffer = request.Length * 4;
        if (requiredBuffer < _bigRequestLength)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
            scratchBuffer.WriteRequest(
                ref request,
                12,
                MemoryMarshal.Cast<ATOM, byte>(properties));
            _protoOutExtended.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            var workingBuffer = scratchBuffer[..requiredBuffer];
            workingBuffer.WriteRequest(
                ref request,
                12,
                MemoryMarshal.Cast<ATOM, byte>(properties));
        }

        return new ResponseProto(_protoOutExtended.Sequence);
    }

    private ResponseProto SendEventBase(bool propagate, uint destination, uint eventMask, XEvent evnt)
    {
        var request = new SendEventType(propagate, destination, eventMask, evnt);
        _protoOutExtended.Send(ref request);
        return new ResponseProto(_protoOutExtended.Sequence);
    }

    private ResponseProto SetAccessControlBase(AccessControlMode mode)
    {
        var request = new SetAccessControlType(mode);
        _protoOutExtended.Send(ref request);
        return new ResponseProto(_protoOutExtended.Sequence);
    }

    private ResponseProto SetClipRectanglesBase(ClipOrdering ordering, uint gc, ushort clipX, ushort clipY,
        Span<Rectangle> rectangles)
    {
        var request = new SetClipRectanglesType(ordering, gc, clipX, clipY, rectangles.Length);
        var requiredBuffer = request.Length * 4;
        if (requiredBuffer < _bigRequestLength)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
            scratchBuffer.WriteRequest(
                ref request,
                12,
                MemoryMarshal.Cast<Rectangle, byte>(rectangles));
            _protoOutExtended.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            var workingBuffer = scratchBuffer[..requiredBuffer];
            workingBuffer.WriteRequest(
                ref request,
                12,
                MemoryMarshal.Cast<Rectangle, byte>(rectangles));
        }

        return new ResponseProto(_protoOutExtended.Sequence);
    }

    private ResponseProto SetCloseDownModeBase(CloseDownMode mode)
    {
        var request = new SetCloseDownModeType(mode);
        _protoOutExtended.Send(ref request);
        return new ResponseProto(_protoOutExtended.Sequence);
    }

    private ResponseProto SetDashesBase(uint gc, ushort dashOffset, Span<byte> dashes)
    {
        var request = new SetDashesType(gc, dashOffset, dashes.Length);
        var requiredBuffer = request.Length * 4;
        if (requiredBuffer < _bigRequestLength)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
            scratchBuffer.WriteRequest(
                ref request,
                12,
                dashes);
            _protoOutExtended.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            var workingBuffer = scratchBuffer[..requiredBuffer];
            workingBuffer.WriteRequest(
                ref request,
                12,
                dashes);
        }

        return new ResponseProto(_protoOutExtended.Sequence);
    }

    private ResponseProto SetFontPathBase(string[] strPaths)
    {
        var length = strPaths.Length;
        strPaths = strPaths.Where(a => a != "fixed").ToArray();
        var request = new SetFontPathType((ushort)length, strPaths.Sum(a => a.Length + 1).AddPadding());
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

            scratchBuffer[^strPaths.Sum(a => a.Length + 1).Padding()..].Clear();
            _protoOutExtended.SendExact(scratchBuffer);
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

            scratchBuffer[^strPaths.Sum(a => a.Length + 1).Padding()..].Clear();
            _protoOutExtended.SendExact(scratchBuffer[..requiredBuffer]);
        }

        return new ResponseProto(_protoOutExtended.Sequence);
    }

    private ResponseProto SetInputFocusBase(InputFocusMode mode, uint focus, uint time)
    {
        var request = new SetInputFocusType(mode, focus, time);
        _protoOutExtended.Send(ref request);
        return new ResponseProto(_protoOutExtended.Sequence);
    }

    private ResponseProto SetScreenSaverBase(short timeout, short interval, TriState preferBlanking,
        TriState allowExposures)
    {
        var request = new SetScreenSaverType(timeout, interval, preferBlanking, allowExposures);
        _protoOutExtended.Send(ref request);
        return new ResponseProto(_protoOutExtended.Sequence);
    }

    private ResponseProto SetSelectionOwnerBase(uint owner, ATOM atom, uint timestamp)
    {
        var request = new SetSelectionOwnerType(owner, atom, timestamp);
        _protoOutExtended.Send(ref request);
        return new ResponseProto(_protoOutExtended.Sequence);
    }

    private ResponseProto StoreColorsBase(uint colormapId, Span<ColorItem> item)
    {
        var request = new StoreColorsType(colormapId, item.Length);
        var requiredBuffer = request.Length * 4;
        if (requiredBuffer < _bigRequestLength)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
            scratchBuffer.WriteRequest(
                ref request,
                8,
                MemoryMarshal.Cast<ColorItem, byte>(item));
            _protoOutExtended.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            var workingBuffer = scratchBuffer[..requiredBuffer];
            workingBuffer.WriteRequest(
                ref request,
                8,
                MemoryMarshal.Cast<ColorItem, byte>(item));
        }

        return new ResponseProto(_protoOutExtended.Sequence);
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
            _protoOutExtended.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            var workingBuffer = scratchBuffer[..requiredBuffer];
            workingBuffer.WriteRequest(
                ref request,
                16,
                name);
        }

        return new ResponseProto(_protoOutExtended.Sequence);
    }

    private ResponseProto UngrabButtonBase(Button button, uint grabWindow, ModifierMask mask)
    {
        var request = new UngrabButtonType(button, grabWindow, mask);
        _protoOutExtended.Send(ref request);
        return new ResponseProto(_protoOutExtended.Sequence);
    }

    private ResponseProto UngrabKeyBase(byte key, uint grabWindow, ModifierMask modifier)
    {
        var request = new UngrabKeyType(key, grabWindow, modifier);
        _protoOutExtended.Send(ref request);
        return new ResponseProto(_protoOutExtended.Sequence);
    }

    private ResponseProto UngrabKeyboardBase(uint time)
    {
        var request = new UngrabKeyboardType(time);
        _protoOutExtended.Send(ref request);
        return new ResponseProto(_protoOutExtended.Sequence);
    }

    private ResponseProto UngrabPointerBase(uint time)
    {
        var request = new UngrabPointerType(time);
        _protoOutExtended.Send(ref request);
        return new ResponseProto(_protoOutExtended.Sequence);
    }

    private ResponseProto UngrabServerBase()
    {
        var request = new UnGrabServerType();
        _protoOutExtended.Send(ref request);
        return new ResponseProto(_protoOutExtended.Sequence);
    }

    private ResponseProto UninstallColormapBase(uint colormapId)
    {
        var request = new UninstallColormapType(colormapId);
        _protoOutExtended.Send(ref request);
        return new ResponseProto(_protoOutExtended.Sequence);
    }

    private ResponseProto UnmapSubwindowsBase(uint window)
    {
        var request = new UnMapSubwindowsType(window);
        _protoOutExtended.Send(ref request);
        return new ResponseProto(_protoOutExtended.Sequence);
    }

    private ResponseProto UnmapWindowBase(uint window)
    {
        var request = new UnmapWindowType(window);
        _protoOutExtended.Send(ref request);
        return new ResponseProto(_protoOutExtended.Sequence);
    }

    private ResponseProto WarpPointerBase(uint srcWindow, uint destinationWindow, short srcX, short srcY,
        ushort srcWidth,
        ushort srcHeight, short destinationX, short destinationY)
    {
        var request = new WarpPointerType(srcWindow, destinationWindow, srcX, srcY, srcWidth, srcHeight, destinationX,
            destinationY);
        _protoOutExtended.Send(ref request);
        return new ResponseProto(_protoOutExtended.Sequence);
    }

    private ResponseProto AllocColorBase(uint colorMap, ushort red, ushort green, ushort blue)
    {
        var request = new AllocColorType(colorMap, red, green, blue);
        _protoOutExtended.Send(ref request);
        return new ResponseProto(_protoOutExtended.Sequence, true);
    }

    private ResponseProto QueryPointerBase(uint window)
    {
        var request = new QueryPointerType(window);
        _protoOutExtended.Send(ref request);
        return new ResponseProto(_protoOutExtended.Sequence, true);
    }

    private ResponseProto GrabPointerBase(bool ownerEvents, uint grabWindow, ushort mask, GrabMode pointerMode,
        GrabMode keyboardMode, uint confineTo, uint cursor, uint timeStamp)
    {
        var request = new GrabPointerType(ownerEvents, grabWindow, mask, pointerMode, keyboardMode, confineTo, cursor,
            timeStamp);
        _protoOutExtended.Send(ref request);
        return new ResponseProto(_protoOutExtended.Sequence, true);
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
            _protoOutExtended.SendExact(scratchBuffer);
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
            _protoOutExtended.SendExact(scratchBuffer[..requiredBuffer]);
        }

        return new ResponseProto(_protoOutExtended.Sequence, true);
    }

    private ResponseProto GetPropertyBase(bool delete, uint window, ATOM property, ATOM type, uint offset,
        uint length)
    {
        var request = new GetPropertyType(delete, window, property, type, offset, length);
        _protoOutExtended.Send(ref request);
        return new ResponseProto(_protoOutExtended.Sequence, true);
    }

    private ResponseProto GetWindowAttributesBase(uint window)
    {
        var request = new GetWindowAttributesType(window);
        _protoOutExtended.Send(ref request);
        return new ResponseProto(_protoOutExtended.Sequence, true);
    }

    private ResponseProto GetGeometryBase(uint drawable)
    {
        var request = new GetGeometryType(drawable);
        _protoOutExtended.Send(ref request);
        return new ResponseProto(_protoOutExtended.Sequence, true);
    }

    private ResponseProto QueryTreeBase(uint window)
    {
        var request = new QueryTreeType(window);
        _protoOutExtended.Send(ref request);
        return new ResponseProto(_protoOutExtended.Sequence, true);
    }

    private ResponseProto GetAtomNameBase(ATOM atom)
    {
        var request = new GetAtomNameType(atom);
        _protoOutExtended.Send(ref request);
        return new ResponseProto(_protoOutExtended.Sequence, true);
    }

    private ResponseProto ListPropertiesBase(uint window)
    {
        var request = new ListPropertiesType(window);
        _protoOutExtended.Send(ref request);
        return new ResponseProto(_protoOutExtended.Sequence, true);
    }

    private ResponseProto GetSelectionOwnerBase(ATOM atom)
    {
        var request = new GetSelectionOwnerType(atom);
        _protoOutExtended.Send(ref request);
        return new ResponseProto(_protoOutExtended.Sequence, true);
    }

    private ResponseProto GrabKeyboardBase(bool ownerEvents, uint grabWindow, uint timeStamp, GrabMode pointerMode,
        GrabMode keyboardMode)
    {
        var request = new GrabKeyboardType(ownerEvents, grabWindow, timeStamp, pointerMode, keyboardMode);
        _protoOutExtended.Send(ref request);
        return new ResponseProto(_protoOutExtended.Sequence, true);
    }

    private ResponseProto GetMotionEventsBase(uint window, uint startTime, uint endTime)
    {
        var request = new GetMotionEventsType(window, startTime, endTime);
        _protoOutExtended.Send(ref request);
        return new ResponseProto(_protoOutExtended.Sequence, true);
    }

    private ResponseProto TranslateCoordinatesBase(uint srcWindow, uint destinationWindow, ushort srcX, ushort srcY)
    {
        var request = new TranslateCoordinatesType(srcWindow, destinationWindow, srcX, srcY);
        _protoOutExtended.Send(ref request);
        return new ResponseProto(_protoOutExtended.Sequence, true);
    }

    private ResponseProto GetInputFocusBase()
    {
        var request = new GetInputFocusType();
        _protoOutExtended.Send(ref request);
        return new ResponseProto(_protoOutExtended.Sequence, true);
    }

    private ResponseProto QueryKeymapBase()
    {
        var request = new QueryKeymapType();
        _protoOutExtended.Send(ref request);
        return new ResponseProto(_protoOutExtended.Sequence, true);
    }

    private ResponseProto QueryFontBase(uint fontId)
    {
        var request = new QueryFontType(fontId);
        _protoOutExtended.Send(ref request);
        return new ResponseProto(_protoOutExtended.Sequence, true);
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
            _protoOutExtended.SendExact(scratchBuffer);
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
            _protoOutExtended.SendExact(scratchBuffer[..requiredBuffer]);
        }

        return new ResponseProto(_protoOutExtended.Sequence, true);
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
            _protoOutExtended.SendExact(scratchBuffer);
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
            _protoOutExtended.SendExact(workingBuffer);
        }

        return new ResponseProto(_protoOutExtended.Sequence, true);
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
            _protoOutExtended.SendExact(scratchBuffer);
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
            _protoOutExtended.SendExact(workingBuffer);
        }

        return new ResponseProto(_protoOutExtended.Sequence, true);
    }

    private ResponseProto GetFontPathBase()
    {
        var request = new GetFontPathType();
        _protoOutExtended.Send(ref request);
        return new ResponseProto(_protoOutExtended.Sequence, true);
    }

    private ResponseProto GetImageBase(ImageFormat format, uint drawable, ushort x, ushort y, ushort width,
        ushort height,
        uint planeMask)
    {
        var request = new GetImageType(format, drawable, x, y, width, height, planeMask);
        _protoOutExtended.Send(ref request);
        return new ResponseProto(_protoOutExtended.Sequence, true);
    }

    private ResponseProto ListInstalledColormapsBase(uint window)
    {
        var request = new ListInstalledColormapsType(window);
        _protoOutExtended.Send(ref request);
        return new ResponseProto(_protoOutExtended.Sequence, true);
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
            _protoOutExtended.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            var workingBuffer = scratchBuffer[..requiredBuffer];
            workingBuffer.WriteRequest(
                ref request,
                12,
                name);
            _protoOutExtended.SendExact(workingBuffer);
        }

        return new ResponseProto(_protoOutExtended.Sequence, true);
    }

    private ResponseProto AllocColorCellsBase(bool contiguous, uint colorMap, ushort colors, ushort planes)
    {
        var request = new AllocColorCellsType(contiguous, colorMap, colors, planes);
        _protoOutExtended.Send(ref request);
        return new ResponseProto(_protoOutExtended.Sequence, true);
    }

    private ResponseProto AllocColorPlanesBase(bool contiguous, uint colorMap, ushort colors, ushort reds,
        ushort greens,
        ushort blues)
    {
        var request = new AllocColorPlanesType(contiguous, colorMap, colors, reds, greens, blues);
        _protoOutExtended.Send(ref request);
        return new ResponseProto(_protoOutExtended.Sequence, true);
    }

    private ResponseProto QueryColorsBase(uint colorMap, Span<uint> pixels)
    {
        var request = new QueryColorsType(colorMap, pixels.Length);
        var requiredBuffer = request.Length * 4;
        if (requiredBuffer < _bigRequestLength)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
            scratchBuffer.WriteRequest(
                ref request,
                8,
                MemoryMarshal.Cast<uint, byte>(pixels));
            _protoOutExtended.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            var workingBuffer = scratchBuffer[..requiredBuffer];
            workingBuffer.WriteRequest(
                ref request,
                8,
                MemoryMarshal.Cast<uint, byte>(pixels));
            _protoOutExtended.SendExact(workingBuffer);
        }

        return new ResponseProto(_protoOutExtended.Sequence, true);
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
            _protoOutExtended.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            var workingBuffer = scratchBuffer[..requiredBuffer];
            workingBuffer.WriteRequest(
                ref request,
                12,
                name);
            _protoOutExtended.SendExact(workingBuffer);
        }

        return new ResponseProto(_protoOutExtended.Sequence, true);
    }

    private ResponseProto QueryBestSizeBase(QueryShapeOf shape, uint drawable, ushort width, ushort height)
    {
        var request = new QueryBestSizeType(shape, drawable, width, height);
        _protoOutExtended.Send(ref request);
        return new ResponseProto(_protoOutExtended.Sequence, true);
    }

    private ResponseProto QueryExtensionBase(ReadOnlySpan<byte> name)
    {
        var request = new QueryExtensionType((ushort)name.Length);
        var requiredBuffer = request.Length * 4;
        if (requiredBuffer < _bigRequestLength)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
            scratchBuffer.WriteRequest(ref request, 8, name);
            _protoOutExtended.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            var workingBuffer = scratchBuffer[..requiredBuffer];
            workingBuffer.WriteRequest(ref request, 8, name);
            _protoOutExtended.SendExact(workingBuffer);
        }

        return new ResponseProto(_protoOutExtended.Sequence, true);
    }

    private ResponseProto ListExtensionsBase()
    {
        var request = new ListExtensionsType();
        _protoOutExtended.Send(ref request);
        return new ResponseProto(_protoOutExtended.Sequence, true);
    }

    private ResponseProto SetModifierMappingBase(Span<ulong> keycodes)
    {
        var request = new SetModifierMappingType(keycodes.Length);
        var requiredBuffer = request.Length * 4;
        if (requiredBuffer < _bigRequestLength)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
            scratchBuffer.WriteRequest(
                ref request,
                4,
                MemoryMarshal.Cast<ulong, byte>(keycodes));
            _protoOutExtended.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchbuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            var workingBuffer = scratchbuffer[..requiredBuffer];
            workingBuffer.WriteRequest(
                ref request,
                4,
                MemoryMarshal.Cast<ulong, byte>(keycodes));
            _protoOutExtended.SendExact(workingBuffer);
        }

        return new ResponseProto(_protoOutExtended.Sequence, true);
    }

    private ResponseProto GetModifierMappingBase()
    {
        var request = new GetModifierMappingType();
        _protoOutExtended.Send(ref request);
        return new ResponseProto(_protoOutExtended.Sequence, true);
    }

    private ResponseProto GetKeyboardMappingBase(byte firstKeycode, byte count)
    {
        var request = new GetKeyboardMappingType(firstKeycode, count);
        _protoOutExtended.Send(ref request);
        return new ResponseProto(_protoOutExtended.Sequence, true);
    }

    private ResponseProto GetKeyboardControlBase()
    {
        var request = new GetKeyboardControlType();
        _protoOutExtended.Send(ref request);
        return new ResponseProto(_protoOutExtended.Sequence, true);
    }

    private ResponseProto SetPointerMappingBase(Span<byte> maps)
    {
        var request = new SetPointerMappingType(maps.Length);
        var requiredBuffer = request.Length * 4;
        if (requiredBuffer < _bigRequestLength)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
            scratchBuffer.WriteRequest(
                ref request,
                4,
                maps);
            _protoOutExtended.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            var workingBuffer = scratchBuffer[..requiredBuffer];
            workingBuffer.WriteRequest(
                ref request,
                4,
                maps);
            _protoOutExtended.SendExact(workingBuffer[..requiredBuffer]);
        }

        return new ResponseProto(_protoOutExtended.Sequence, true);
    }

    private ResponseProto GetPointerMappingBase()
    {
        var request = new GetPointerMappingType();
        _protoOutExtended.Send(ref request);
        return new ResponseProto(_protoOutExtended.Sequence, true);
    }

    private ResponseProto GetPointerControlBase()
    {
        var request = new GetPointerControlType();
        _protoOutExtended.Send(ref request);
        return new ResponseProto(_protoOutExtended.Sequence, true);
    }

    private ResponseProto GetScreenSaverBase()
    {
        var request = new GetScreenSaverType();
        _protoOutExtended.Send(ref request);
        return new ResponseProto(_protoOutExtended.Sequence, true);
    }

    private ResponseProto ListHostsBase()
    {
        var request = new ListHostsType();
        _protoOutExtended.Send(ref request);
        return new ResponseProto(_protoOutExtended.Sequence, true);
    }
}