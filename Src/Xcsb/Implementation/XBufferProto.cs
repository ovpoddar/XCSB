using System.Runtime.InteropServices;
using System.Text;
using Xcsb.Handlers.Buffered;
using Xcsb.Handlers.Direct;
using Xcsb.Infrastructure;
using Xcsb.Infrastructure.Exceptions;
using Xcsb.Masks;
using Xcsb.Models;
using Xcsb.Models.String;
using Xcsb.Requests;
using Xcsb.Connection.Helpers;
#if !NETSTANDARD
using System.Numerics;
#endif

namespace Xcsb.Implementation;

internal sealed class XBufferProto : IXBufferProto
{
    private readonly BufferProtoOut _bufferProtoOut;
    private readonly BufferProtoIn _bufferProtoIn;

    public XBufferProto(ProtoInExtended protoIn, ProtoOutExtended protoOut)
    {
        _bufferProtoOut = new BufferProtoOut(protoOut);
        _bufferProtoIn = new BufferProtoIn(protoIn);
    }

    public void AllowEvents(EventsMode mode, uint time)
    {
        var request = new AllowEventsType(mode, time);
        _bufferProtoOut.Add(ref request);
    }

    public void Bell(sbyte percent)
    {
        if (percent is not <= 100 or not >= -100)
            throw new ArgumentOutOfRangeException(nameof(percent), "value must be between -100 to 100");

        var request = new BellType(percent);
        _bufferProtoOut.Add(ref request);
    }

    public void ChangeActivePointerGrab(uint cursor, uint time, ushort mask)
    {
        var request = new ChangeActivePointerGrabType(cursor, time, mask);
        _bufferProtoOut.Add(ref request);
    }

    public void ChangeGC(uint gc, GCMask mask, Span<uint> args)
    {
        if (mask.CountFlags() != args.Length)
            throw new InsufficientDataException(mask.CountFlags(), args.Length, nameof(mask), nameof(args));

        var request = new ChangeGCType(gc, mask, args.Length);
        _bufferProtoOut.Add(ref request);
        _bufferProtoOut.AddRange(args);

    }

    public void ChangeHosts(HostMode mode, Family family, Span<byte> address)
    {
        var request = new ChangeHostsType(mode, family, address.Length);
        _bufferProtoOut.Add(ref request);
        _bufferProtoOut.AddRange(address);
        _bufferProtoOut.AddRange(new byte[address.Length.Padding()]);

    }

    public void ChangeKeyboardControl(KeyboardControlMask mask, Span<uint> args)
    {
        var request = new ChangeKeyboardControlType(mask, args.Length);
        _bufferProtoOut.Add(ref request);
        _bufferProtoOut.AddRange(args);
    }

    public void ChangeKeyboardMapping(byte keycodeCount, byte firstKeycode, byte keysymsPerKeycode, Span<uint> keysym)
    {
        var request = new ChangeKeyboardMappingType(keycodeCount, firstKeycode, keysymsPerKeycode);
        _bufferProtoOut.Add(ref request);
        _bufferProtoOut.AddRange(keysym);
    }

    public void ChangePointerControl(Acceleration? acceleration, ushort? threshold)
    {
        var request = new ChangePointerControlType(acceleration?.Numerator ?? 0, acceleration?.Denominator ?? 0,
            threshold ?? 0, (byte)(acceleration is null ? 0 : 1), (byte)(threshold.HasValue ? 1 : 0));
        _bufferProtoOut.Add(ref request);
    }

    public void ChangeProperty<T>(PropertyMode mode, uint window, ATOM property, ATOM type, Span<T> args)
        where T : struct
#if !NETSTANDARD
        , INumber<T>
#endif
    {
        var size = Marshal.SizeOf<T>();
        if (size is not 1 and not 2 and not 4)
            throw new ArgumentException("type must be byte, sbyte, short, ushort, int, uint");
        var request = new ChangePropertyType(mode, window, property, type, args.Length, size);
        _bufferProtoOut.Add(ref request);
        _bufferProtoOut.AddRange(args);
        _bufferProtoOut.AddRange(new byte[(args.Length * size).Padding()]);
    }

    public void ChangeSaveSet(ChangeSaveSetMode changeSaveSetMode, uint window)
    {
        var request = new ChangeSaveSetType(changeSaveSetMode, window);
        _bufferProtoOut.Add(ref request);
    }

    public void ChangeWindowAttributes(uint window, ValueMask mask, Span<uint> args)
    {
        if (mask.CountFlags() != args.Length)
            throw new InsufficientDataException(mask.CountFlags(), args.Length, nameof(mask), nameof(args));

        var request = new ChangeWindowAttributesType(window, mask, args.Length);
        _bufferProtoOut.Add(ref request);
        _bufferProtoOut.AddRange(args);
    }

    public void CirculateWindow(Circulate circulate, uint window)
    {
        var request = new CirculateWindowType(circulate, window);
        _bufferProtoOut.Add(ref request);
    }

    public void ClearArea(bool exposures, uint window, short x, short y, ushort width, ushort height)
    {
        var request = new ClearAreaType(exposures, window, x, y, width, height);
        _bufferProtoOut.Add(ref request);
    }

    public void CloseFont(uint fontId)
    {
        var request = new CloseFontType(fontId);
        _bufferProtoOut.Add(ref request);
    }

    public void ConfigureWindow(uint window, ConfigureValueMask mask, Span<uint> args)
    {
        if (mask.CountFlags() != args.Length)
            throw new InsufficientDataException(mask.CountFlags(), args.Length, nameof(mask), nameof(args));
        var request = new ConfigureWindowType(window, mask, args.Length);
        _bufferProtoOut.Add(ref request);
        _bufferProtoOut.AddRange(args);
    }

    public void ConvertSelection(uint requestor, ATOM selection, ATOM target, ATOM property, uint timestamp)
    {
        var request = new ConvertSelectionType(requestor, selection, target, property, timestamp);
        _bufferProtoOut.Add(ref request);
    }

    public void CopyArea(uint srcDrawable, uint destinationDrawable, uint gc, ushort srcX, ushort srcY,
        ushort destinationX,
        ushort destinationY,
        ushort width, ushort height)
    {
        var request = new CopyAreaType(srcDrawable, destinationDrawable, gc, srcX, srcY, destinationX, destinationY,
            width, height);
        _bufferProtoOut.Add(ref request);
    }

    public void CopyColormapAndFree(uint colormapId, uint srcColormapId)
    {
        var request = new CopyColormapAndFreeType(colormapId, srcColormapId);
        _bufferProtoOut.Add(ref request);
    }

    public void CopyGC(uint srcGc, uint dstGc, GCMask mask)
    {
        var request = new CopyGCType(srcGc, dstGc, mask);
        _bufferProtoOut.Add(ref request);
    }

    public void CopyPlane(uint srcDrawable, uint destinationDrawable, uint gc, ushort srcX, ushort srcY,
        ushort destinationX, ushort destinationY, ushort width, ushort height, uint bitPlane)
    {
        var request = new CopyPlaneType(srcDrawable, destinationDrawable, gc, srcX, srcY, destinationX, destinationY,
            width, height,
            bitPlane);
        _bufferProtoOut.Add(ref request);
    }

    public void CreateColormap(ColormapAlloc alloc, uint colormapId, uint window, uint visual)
    {
        var request = new CreateColormapType(alloc, colormapId, window, visual);
        _bufferProtoOut.Add(ref request);
    }

    public void CreateCursor(uint cursorId, uint source, uint mask, ushort foreRed, ushort foreGreen, ushort foreBlue,
        ushort backRed, ushort backGreen, ushort backBlue, ushort x, ushort y)
    {
        var request = new CreateCursorType(cursorId, source, mask, foreRed, foreGreen, foreBlue, backRed, backGreen,
            backBlue, x, y);
        _bufferProtoOut.Add(ref request);
    }

    public void CreateGC(uint gc, uint drawable, GCMask mask, Span<uint> args)
    {
        if (mask.CountFlags() != args.Length)
            throw new InsufficientDataException(mask.CountFlags(), args.Length, nameof(mask), nameof(args));

        var request = new CreateGCType(gc, drawable, mask, args.Length);
        _bufferProtoOut.Add(ref request);
        _bufferProtoOut.AddRange(args);
    }

    public void CreateGlyphCursor(uint cursorId, uint sourceFont, uint fontMask, char sourceChar, ushort charMask,
        ushort foreRed, ushort foreGreen, ushort foreBlue, ushort backRed, ushort backGreen, ushort backBlue)
    {
        var request = new CreateGlyphCursorType(cursorId, sourceFont, fontMask, sourceChar, charMask, foreRed,
            foreGreen, foreBlue, backRed, backGreen, backBlue);
        _bufferProtoOut.Add(ref request);
    }

    public void CreatePixmap(byte depth, uint pixmapId, uint drawable, ushort width, ushort height)
    {
        var request = new CreatePixmapType(depth, pixmapId, drawable, width, height);
        _bufferProtoOut.Add(ref request);
    }

    public void CreateWindow(byte depth, uint window, uint parent, short x, short y, ushort width, ushort height,
        ushort borderWidth, ClassType classType, uint rootVisualId, ValueMask mask, Span<uint> args)
    {
        var request = new CreateWindowType(depth, window, parent, x, y, width, height, borderWidth, classType,
            rootVisualId, mask, args.Length);
        _bufferProtoOut.Add(ref request);
        _bufferProtoOut.AddRange(args);
    }

    public void DeleteProperty(uint window, ATOM atom)
    {
        var request = new DeletePropertyType(window, atom);
        _bufferProtoOut.Add(ref request);
    }

    public void DestroySubwindows(uint window)
    {
        var request = new DestroySubWindowsType(window);
        _bufferProtoOut.Add(ref request);
    }

    public void DestroyWindow(uint window)
    {
        var request = new DestroyWindowType(window);
        _bufferProtoOut.Add(ref request);
    }

    public void FillPoly(uint drawable, uint gc, PolyShape shape, CoordinateMode coordinate, Span<Point> points)
    {
        var request = new FillPolyType(drawable, gc, shape, coordinate, points.Length);
        _bufferProtoOut.Add(ref request);
        _bufferProtoOut.AddRange(points);
    }

    public void FlushChecked() =>
        FlushBase(true);

    public void Flush() =>
        FlushBase(false);

    private void FlushBase(bool shouldThorw)
    {
        try
        {
            _bufferProtoIn.ProtoIn.FlushSocket();
            var outProtoSequence = _bufferProtoOut.Sequence;
            _bufferProtoOut.Flush();
            _bufferProtoIn.FlushSocket(_bufferProtoIn.ProtoIn.Sequence, outProtoSequence, shouldThorw);
        }
        finally
        {
            _bufferProtoOut.Reset();
        }
    }

    public void ForceScreenSaver(ForceScreenSaverMode mode)
    {
        var request = new ForceScreenSaverType(mode);
        _bufferProtoOut.Add(ref request);

    }

    public void FreeColormap(uint colormapId)
    {
        var request = new FreeColormapType(colormapId);
        _bufferProtoOut.Add(ref request);

    }

    public void FreeColors(uint colormapId, uint planeMask, Span<uint> pixels)
    {
        var request = new FreeColorsType(colormapId, planeMask, pixels.Length);
        _bufferProtoOut.Add(ref request);
        _bufferProtoOut.AddRange(pixels);

    }

    public void FreeCursor(uint cursorId)
    {
        var request = new FreeCursorType(cursorId);
        _bufferProtoOut.Add(ref request);

    }

    public void FreeGC(uint gc)
    {
        var request = new FreeGCType(gc);
        _bufferProtoOut.Add(ref request);

    }

    public void FreePixmap(uint pixmapId)
    {
        var request = new FreePixmapType(pixmapId);
        _bufferProtoOut.Add(ref request);

    }

    public void GrabButton(bool ownerEvents, uint grabWindow, ushort mask, GrabMode pointerMode, GrabMode keyboardMode,
        uint confineTo, uint cursor, Button button, ModifierMask modifiers)
    {
        var request = new GrabButtonType(ownerEvents, grabWindow, mask, pointerMode, keyboardMode, confineTo, cursor,
            button, modifiers);
        _bufferProtoOut.Add(ref request);

    }

    public void GrabKey(bool exposures, uint grabWindow, ModifierMask mask, byte keycode, GrabMode pointerMode,
        GrabMode keyboardMode)
    {
        var request = new GrabKeyType(exposures, grabWindow, mask, keycode, pointerMode, keyboardMode);
        _bufferProtoOut.Add(ref request);

    }

    public void GrabServer()
    {
        var request = new GrabServerType();
        _bufferProtoOut.Add(ref request);
    }

    public void ImageText16(uint drawable, uint gc, short x, short y, ReadOnlySpan<char> text)
    {
        var request = new ImageText16Type(drawable, gc, x, y, text.Length);
        _bufferProtoOut.Add(ref request);
        _bufferProtoOut.AddRange(Encoding.BigEndianUnicode.GetBytes(text.ToString()));
        _bufferProtoOut.AddRange(new byte[(16 + (text.Length * 2)).Padding()]);

    }

    public void ImageText8(uint drawable, uint gc, short x, short y, ReadOnlySpan<byte> text)
    {
        var request = new ImageText8Type(drawable, gc, x, y, text.Length);
        _bufferProtoOut.Add(ref request);
        _bufferProtoOut.AddRange(text);
        _bufferProtoOut.AddRange(new byte[text.Length.Padding()]);
    }

    public void InstallColormap(uint colormapId)
    {
        var request = new InstallColormapType(colormapId);
        _bufferProtoOut.Add(ref request);

    }

    public void KillClient(uint resource)
    {
        var request = new KillClientType(resource);
        _bufferProtoOut.Add(ref request);

    }

    public void MapSubwindows(uint window)
    {
        var request = new MapSubWindowsType(window);
        _bufferProtoOut.Add(ref request);

    }

    public void MapWindow(uint window)
    {
        var request = new MapWindowType(window);
        _bufferProtoOut.Add(ref request);

    }

    public void NoOperation(Span<uint> args)
    {
        var request = new NoOperationType(args.Length);
        _bufferProtoOut.Add(ref request);
        _bufferProtoOut.AddRange(args);
    }

    public void OpenFont(string fontName, uint fontId)
    {
        var request = new OpenFontType(fontId, fontName.Length);
        _bufferProtoOut.Add(ref request);
        _bufferProtoOut.AddRange(Encoding.ASCII.GetBytes(fontName));
        _bufferProtoOut.AddRange(new byte[fontName.Length.Padding()]);

    }

    public void PolyArc(uint drawable, uint gc, Span<Arc> arcs)
    {
        var request = new PolyArcType(drawable, gc, arcs.Length);
        _bufferProtoOut.Add(ref request);
        _bufferProtoOut.AddRange(arcs);
    }

    public void PolyFillArc(uint drawable, uint gc, Span<Arc> arcs)
    {
        var request = new PolyFillArcType(drawable, gc, arcs.Length);
        _bufferProtoOut.Add(ref request);
        _bufferProtoOut.AddRange(arcs);
    }

    public void PolyFillRectangle(uint drawable, uint gc, Span<Rectangle> rectangles)
    {
        var request = new PolyFillRectangleType(drawable, gc, rectangles.Length);
        _bufferProtoOut.Add(ref request);
        _bufferProtoOut.AddRange(rectangles);
    }

    public void PolyLine(CoordinateMode coordinate, uint drawable, uint gc, Span<Point> points)
    {
        var request = new PolyLineType(coordinate, drawable, gc, points.Length);
        _bufferProtoOut.Add(ref request);
        _bufferProtoOut.AddRange(points);
    }

    public void PolyPoint(CoordinateMode coordinate, uint drawable, uint gc, Span<Point> points)
    {
        var request = new PolyPointType(coordinate, drawable, gc, points.Length);
        _bufferProtoOut.Add(ref request);
        _bufferProtoOut.AddRange(points);
    }

    public void PolyRectangle(uint drawable, uint gc, Span<Rectangle> rectangles)
    {
        var request = new PolyRectangleType(drawable, gc, rectangles.Length);
        _bufferProtoOut.Add(ref request);
        _bufferProtoOut.AddRange(rectangles);
    }

    public void PolySegment(uint drawable, uint gc, Span<Segment> segments)
    {
        var request = new PolySegmentType(drawable, gc, segments.Length);
        _bufferProtoOut.Add(ref request);
        _bufferProtoOut.AddRange(segments);
    }

    public void PolyText16(uint drawable, uint gc, ushort x, ushort y, TextItem16[] data)
    {
        var request = new PolyText16Type(drawable, gc, x, y, data.Sum(a => a.Count));
        _bufferProtoOut.Add(ref request);
        foreach (var item in data)
            _bufferProtoOut.AddRange(item.ToArray());
        _bufferProtoOut.AddRange(new byte[data.Sum(a => a.Count).Padding()]);
    }

    public void PolyText8(uint drawable, uint gc, ushort x, ushort y, TextItem8[] data)
    {
        var request = new PolyText8Type(drawable, gc, x, y, data.Sum(a => a.Count));
        _bufferProtoOut.Add(ref request);
        foreach (var item in data)
            _bufferProtoOut.AddRange(item.ToArray());
        _bufferProtoOut.AddRange(new byte[data.Sum(a => a.Count).Padding()]);
    }

    public void PutImage(ImageFormatBitmap format, uint drawable, uint gc, ushort width, ushort height, short x, short y,
        byte leftPad, byte depth, Span<byte> data)
    {
        var request = new PutImageType(format, drawable, gc, width, height, x, y, leftPad, depth, data.Length);
        _bufferProtoOut.Add(ref request);
        _bufferProtoOut.AddRange(data);
        _bufferProtoOut.AddRange(new byte[data.Length.Padding()]);
    }

    public void RecolorCursor(uint cursorId, ushort foreRed, ushort foreGreen, ushort foreBlue, ushort backRed,
        ushort backGreen, ushort backBlue)
    {
        var request = new RecolorCursorType(cursorId, foreRed, foreGreen, foreBlue, backRed, backGreen, backBlue);
        _bufferProtoOut.Add(ref request);
    }

    public void ReparentWindow(uint window, uint parent, short x, short y)
    {
        var request = new ReparentWindowType(window, parent, x, y);
        _bufferProtoOut.Add(ref request);
    }

    public void RotateProperties(uint window, ushort delta, Span<ATOM> properties)
    {
        var request = new RotatePropertiesType(window, properties.Length, delta);
        _bufferProtoOut.Add(ref request);
        _bufferProtoOut.AddRange(properties);
    }

    public void SendEvent(bool propagate, uint destination, uint eventMask, XEvent evnt)
    {
        var request = new SendEventType(propagate, destination, eventMask, evnt);
        _bufferProtoOut.Add(ref request);
    }

    public void SetAccessControl(AccessControlMode mode)
    {
        var request = new SetAccessControlType(mode);
        _bufferProtoOut.Add(ref request);
    }

    public void SetClipRectangles(ClipOrdering ordering, uint gc, ushort clipX, ushort clipY, Span<Rectangle> rectangles)
    {
        var request = new SetClipRectanglesType(ordering, gc, clipX, clipY, rectangles.Length);
        _bufferProtoOut.Add(ref request);
        _bufferProtoOut.AddRange(rectangles);
    }

    public void SetCloseDownMode(CloseDownMode mode)
    {
        var request = new SetCloseDownModeType(mode);
        _bufferProtoOut.Add(ref request);
    }

    public void SetDashes(uint gc, ushort dashOffset, Span<byte> dashes)
    {
        var request = new SetDashesType(gc, dashOffset, dashes.Length);
        _bufferProtoOut.Add(ref request);
        _bufferProtoOut.AddRange(dashes);
        _bufferProtoOut.AddRange(new byte[dashes.Length.Padding()]);
    }

    public void SetFontPath(string[] strPaths)
    {
        var length = strPaths.Length;
        strPaths = strPaths.Where(a => a != "fixed").ToArray();
        var request = new SetFontPathType((ushort)length, strPaths.Sum(a => a.Length + 1).AddPadding());
        _bufferProtoOut.Add(ref request);
        foreach (var path in strPaths.OrderBy(a => a.Length))
        {
            _bufferProtoOut.Add((byte)path.Length);
            _bufferProtoOut.AddRange(Encoding.ASCII.GetBytes(path));
        }
        _bufferProtoOut.AddRange(new byte[strPaths.Sum(a => a.Length + 1).Padding()]);
    }

    public void SetInputFocus(InputFocusMode mode, uint focus, uint time)
    {
        var request = new SetInputFocusType(mode, focus, time);
        _bufferProtoOut.Add(ref request);
    }

    public void SetScreenSaver(short timeout, short interval, TriState preferBlanking, TriState allowExposures)
    {
        var request = new SetScreenSaverType(timeout, interval, preferBlanking, allowExposures);
        _bufferProtoOut.Add(ref request);
    }

    public void SetSelectionOwner(uint owner, ATOM atom, uint timestamp)
    {
        var request = new SetSelectionOwnerType(owner, atom, timestamp);
        _bufferProtoOut.Add(ref request);
    }

    public void StoreColors(uint colormapId, Span<ColorItem> item)
    {
        var request = new StoreColorsType(colormapId, item.Length);
        _bufferProtoOut.Add(ref request);
        _bufferProtoOut.AddRange(item);
    }

    public void StoreNamedColor(ColorFlag mode, uint colormapId, uint pixels, ReadOnlySpan<byte> name)
    {
        var request = new StoreNamedColorType(mode, colormapId, pixels, name.Length);
        _bufferProtoOut.Add(ref request);
        _bufferProtoOut.AddRange(name);
        _bufferProtoOut.AddRange(new byte[name.Length.Padding()]);
    }

    public void UngrabButton(Button button, uint grabWindow, ModifierMask mask)
    {
        var request = new UngrabButtonType(button, grabWindow, mask);
        _bufferProtoOut.Add(ref request);
    }

    public void UngrabKey(byte key, uint grabWindow, ModifierMask modifier)
    {
        var request = new UngrabKeyType(key, grabWindow, modifier);
        _bufferProtoOut.Add(ref request);
    }

    public void UngrabKeyboard(uint time)
    {
        var request = new UngrabKeyboardType(time);
        _bufferProtoOut.Add(ref request);
    }

    public void UngrabPointer(uint time)
    {
        var request = new UngrabPointerType(time);
        _bufferProtoOut.Add(ref request);
    }

    public void UngrabServer()
    {
        var request = new UnGrabServerType();
        _bufferProtoOut.Add(ref request);
    }

    public void UninstallColormap(uint colormapId)
    {
        var request = new UninstallColormapType(colormapId);
        _bufferProtoOut.Add(ref request);
    }

    public void UnmapSubwindows(uint window)
    {
        var request = new UnMapSubwindowsType(window);
        _bufferProtoOut.Add(ref request);
    }

    public void UnmapWindow(uint window)
    {
        var request = new UnmapWindowType(window);
        _bufferProtoOut.Add(ref request);
    }

    public void WarpPointer(uint srcWindow, uint destinationWindow, short srcX, short srcY, ushort srcWidth,
        ushort srcHeight, short destinationX, short destinationY)
    {
        var request = new WarpPointerType(srcWindow, destinationWindow, srcX, srcY, srcWidth, srcHeight, destinationX,
            destinationY);
        _bufferProtoOut.Add(ref request);
    }
}