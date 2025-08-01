using System.Runtime.InteropServices;
using System.Text;
using Xcsb.Event;
using Xcsb.Helpers;
using Xcsb.Masks;
using Xcsb.Models;
using Xcsb.Models.Infrastructure.Exceptions;
using Xcsb.Requests;

#if !NETSTANDARD
using System.Numerics;
#endif

namespace Xcsb;

internal class XBufferProto : BaseProtoClient, IXBufferProto
{
    private readonly List<byte> _buffer = [];
    private int _requestLength;

    public XBufferProto(XProto xProto) : base(xProto.socket)
    {
        // todo: pass a configuration object and based on that set up the XBufferProto
        _requestLength = 0;
    }

    public void AllowEvents(EventsMode mode, uint time)
    {
        var request = new AllowEventsType(mode, time);
        _buffer.Add(ref request);
        _requestLength++;
    }

    public void Bell(sbyte percent)
    {
        if (percent is not <= 100 or not >= -100)
            throw new ArgumentOutOfRangeException(nameof(percent), "value must be between -100 to 100");

        var request = new BellType(percent);
        _buffer.Add(ref request);
        _requestLength++;
    }

    public void ChangeActivePointerGrab(uint cursor, uint time, ushort mask)
    {
        var request = new ChangeActivePointerGrabType(cursor, time, mask);
        _buffer.Add(ref request);
        _requestLength++;
    }

    public void ChangeGC(uint gc, GCMask mask, params uint[] args)
    {
        var request = new ChangeGCType(gc, mask, args.Length);
        _buffer.Add(ref request);
        _buffer.AddRange(MemoryMarshal.Cast<uint, byte>(args));

        _requestLength++;
    }

    public void ChangeHosts(HostMode mode, Family family, byte[] address)
    {
        var request = new ChangeHostsType(mode, family, address.Length);
        _buffer.Add(ref request);
        _buffer.AddRange(address);
        _buffer.AddRange(new byte[address.Length.Padding()]);

        _requestLength++;
    }

    public void ChangeKeyboardControl(KeyboardControlMask mask, params uint[] args)
    {
        var request = new ChangeKeyboardControlType(mask, args.Length);
        _buffer.Add(ref request);
        _buffer.AddRange(MemoryMarshal.Cast<uint, byte>(args));
        _requestLength++;
    }

    public void ChangeKeyboardMapping(byte keycodeCount, byte firstKeycode, byte keysymsPerKeycode, uint[] keysym)
    {
        var request = new ChangeKeyboardMappingType(keycodeCount, firstKeycode, keysymsPerKeycode);
        _buffer.Add(ref request);
        _buffer.AddRange(MemoryMarshal.Cast<uint, byte>(keysym));
        _requestLength++;
    }

    public void ChangePointerControl(Acceleration acceleration, ushort? threshold)
    {
        var request = new ChangePointerControlType(acceleration?.Numerator ?? 0, acceleration?.Denominator ?? 0,
            threshold ?? 0,
            (byte)(acceleration is null ? 0 : 1), (byte)(threshold.HasValue ? 1 : 0));
        _buffer.Add(ref request);
        _requestLength++;
    }

    public void ChangeProperty<T>(PropertyMode mode, uint window, uint property, uint type, params T[] args)
        where T : struct
#if !NETSTANDARD
        , INumber<T>
#endif
    {
        var size = Marshal.SizeOf<T>();
        if (size is not 1 or 2 or 4)
            throw new ArgumentException("type must be byte, sbyte, short, ushort, int, uint");
        var request = new ChangePropertyType(mode, window, property, type, args.Length, (byte)(size * 8));
        _buffer.Add(ref request);
        _buffer.AddRange(MemoryMarshal.Cast<T, byte>(args));
        _buffer.AddRange(new byte[args.Length.Padding()]);
        _requestLength++;
    }

    public void ChangeSaveSet(ChangeSaveSetMode changeSaveSetMode, uint window)
    {
        var request = new ChangeSaveSetType(changeSaveSetMode, window);
        _buffer.Add(ref request);
        _requestLength++;
    }

    public void ChangeWindowAttributes(uint window, ValueMask mask, params uint[] args)
    {
        var request = new ChangeWindowAttributesType(window, mask, args.Length);
        _buffer.Add(ref request);
        _buffer.AddRange(MemoryMarshal.Cast<uint, byte>(args));
        _requestLength++;
    }

    public void CirculateWindow(Circulate circulate, uint window)
    {
        var request = new CirculateWindowType(circulate, window);
        _buffer.Add(ref request);
        _requestLength++;
    }

    public void ClearArea(bool exposures, uint window, short x, short y, ushort width, ushort height)
    {
        var request = new ClearAreaType(exposures, window, x, y, width, height);
        _buffer.Add(ref request);
        _requestLength++;
    }

    public void CloseFont(uint fontId)
    {
        var request = new CloseFontType(fontId);
        _buffer.Add(ref request);
        _requestLength++;
    }

    public void ConfigureWindow(uint window, ConfigureValueMask mask, params uint[] args)
    {
        var request = new ConfigureWindowType(window, mask, args.Length);
        _buffer.Add(ref request);
        _buffer.AddRange(MemoryMarshal.Cast<uint, byte>(args));
        _requestLength++;
    }

    public void ConvertSelection(uint requestor, uint selection, uint target, uint property, uint timestamp)
    {
        var request = new ConvertSelectionType(requestor, selection, target, property, timestamp);
        _buffer.Add(ref request);
        _requestLength++;
    }

    public void CopyArea(uint srcDrawable, uint destinationDrawable, uint gc, ushort srcX, ushort srcY,
        ushort destinationX,
        ushort destinationY,
        ushort width, ushort height)
    {
        var request = new CopyAreaType(srcDrawable, destinationDrawable, gc, srcX, srcY, destinationX, destinationY,
            width, height);
        _buffer.Add(ref request);
        _requestLength++;
    }

    public void CopyColormapAndFree(uint colormapId, uint srcColormapId)
    {
        var request = new CopyColormapAndFreeType(colormapId, srcColormapId);
        _buffer.Add(ref request);
        _requestLength++;
    }

    public void CopyGC(uint srcGc, uint dstGc, GCMask mask)
    {
        var request = new CopyGCType(srcGc, dstGc, mask);
        _buffer.Add(ref request);
        _requestLength++;
    }

    public void CopyPlane(uint srcDrawable, uint destinationDrawable, uint gc, ushort srcX, ushort srcY,
        ushort destinationX, ushort destinationY, ushort width, ushort height, uint bitPlane)
    {
        var request = new CopyPlaneType(srcDrawable, destinationDrawable, gc, srcX, srcY, destinationX, destinationY,
            width, height,
            bitPlane);
        _buffer.Add(ref request);
        _requestLength++;
    }

    public void CreateColormap(ColormapAlloc alloc, uint colormapId, uint window, uint visual)
    {
        var request = new CreateColormapType(alloc, colormapId, window, visual);
        _buffer.Add(ref request);
        _requestLength++;
    }

    public void CreateCursor(uint cursorId, uint source, uint mask, ushort foreRed, ushort foreGreen, ushort foreBlue,
        ushort backRed, ushort backGreen, ushort backBlue, ushort x, ushort y)
    {
        var request = new CreateCursorType(cursorId, source, mask, foreRed, foreGreen, foreBlue, backRed, backGreen,
            backBlue, x, y);
        _buffer.Add(ref request);
        _requestLength++;
    }

    public void CreateGC(uint gc, uint drawable, GCMask mask, params uint[] args)
    {
        var request = new CreateGCType(gc, drawable, mask, args.Length);
        _buffer.Add(ref request);
        _buffer.AddRange(MemoryMarshal.Cast<uint, byte>(args));
        _requestLength++;
    }

    public void CreateGlyphCursor(uint cursorId, uint sourceFont, uint fontMask, char sourceChar, ushort charMask,
        ushort foreRed, ushort foreGreen, ushort foreBlue, ushort backRed, ushort backGreen, ushort backBlue)
    {
        var request = new CreateGlyphCursorType(cursorId, sourceFont, fontMask, sourceChar, charMask, foreRed,
            foreGreen, foreBlue, backRed, backGreen, backBlue);
        _buffer.Add(ref request);
        _requestLength++;
    }

    public void CreatePixmap(byte depth, uint pixmapId, uint drawable, ushort width, ushort height)
    {
        var request = new CreatePixmapType(depth, pixmapId, drawable, width, height);
        _buffer.Add(ref request);
        _requestLength++;
    }

    public void CreateWindow(byte depth, uint window, uint parent, short x, short y, ushort width, ushort height,
        ushort borderWidth, ClassType classType, uint rootVisualId, ValueMask mask, params uint[] args)
    {
        var request = new CreateWindowType(depth, window, parent, x, y, width, height, borderWidth, classType,
            rootVisualId, mask, args.Length);
        _buffer.Add(ref request);
        _buffer.AddRange(MemoryMarshal.Cast<uint, byte>(args));
        _requestLength++;
    }

    public void DeleteProperty(uint window, uint atom)
    {
        var request = new DeletePropertyType(window, atom);
        _buffer.Add(ref request);
        _requestLength++;
    }

    public void DestroySubwindows(uint window)
    {
        var request = new DestroySubWindowsType(window);
        _buffer.Add(ref request);
        _requestLength++;
    }

    public void DestroyWindow(uint window)
    {
        var request = new DestroyWindowType(window);
        _buffer.Add(ref request);
        _requestLength++;
    }

    public void FillPoly(uint drawable, uint gc, PolyShape shape, CoordinateMode coordinate, Point[] points)
    {
        var request = new FillPolyType(drawable, gc, shape, coordinate, points.Length);
        _buffer.Add(ref request);
        _buffer.AddRange(MemoryMarshal.Cast<Point, byte>(points));
        _requestLength++;
    }

    public void FlushChecked()
    {
#if NETSTANDARD
        socket.SendExact(_buffer.ToArray());
#else
        socket.SendExact(CollectionsMarshal.AsSpan(_buffer));
#endif
        using var buffer = new ArrayPoolUsing<byte>(Marshal.SizeOf<XEvent>() * _requestLength);
        var received = socket.Receive(buffer);
        foreach (var evnt in MemoryMarshal.Cast<byte, XEvent>(buffer[..received]))
            if (evnt.EventType == EventType.Error)
                throw new XEventException(evnt.ErrorEvent);
            else if ((int)evnt.EventType == 1)
                throw new Exception("internal issue");
            else
            {
                bufferEvents.Push(evnt);

                sequenceNumber += (ushort)_requestLength;
                _requestLength = 0;
                _buffer.Clear();
            }
    }

    public void Flush()
    {
        try
        {
#if NETSTANDARD
            socket.SendExact(_buffer.ToArray());
#else
            socket.SendExact(CollectionsMarshal.AsSpan(_buffer));
#endif
            using var buffer = new ArrayPoolUsing<byte>(socket.Available);
            var received = socket.Receive(buffer);
            foreach (var evnt in MemoryMarshal.Cast<byte, XEvent>(buffer[..received]))
                if (evnt.EventType == EventType.Error)
                    _requestLength--;
                else if ((int)evnt.EventType == 1)
                    continue;
                else
                    bufferEvents.Push(evnt);
        }
        finally
        {
            sequenceNumber += (ushort)_requestLength;
            _requestLength = 0;
            _buffer.Clear();
        }
    }

    public void ForceScreenSaver(ForceScreenSaverMode mode)
    {
        var request = new ForceScreenSaverType(mode);
        _buffer.Add(ref request);

        _requestLength++;
    }

    public void FreeColormap(uint colormapId)
    {
        var request = new FreeColormapType(colormapId);
        _buffer.Add(ref request);

        _requestLength++;
    }

    public void FreeColors(uint colormapId, uint planeMask, params uint[] pixels)
    {
        var request = new FreeColorsType(colormapId, planeMask, pixels.Length);
        _buffer.Add(ref request);
        _buffer.AddRange(MemoryMarshal.Cast<uint, byte>(pixels));

        _requestLength++;
    }

    public void FreeCursor(uint cursorId)
    {
        var request = new FreeCursorType(cursorId);
        _buffer.Add(ref request);

        _requestLength++;
    }

    public void FreeGC(uint gc)
    {
        var request = new FreeGCType(gc);
        _buffer.Add(ref request);

        _requestLength++;
    }

    public void FreePixmap(uint pixmapId)
    {
        var request = new FreePixmapType(pixmapId);
        _buffer.Add(ref request);

        _requestLength++;
    }

    public void GrabButton(bool ownerEvents, uint grabWindow, ushort mask, GrabMode pointerMode, GrabMode keyboardMode,
        uint confineTo, uint cursor, Button button, ModifierMask modifiers)
    {
        var request = new GrabButtonType(ownerEvents, grabWindow, mask, pointerMode, keyboardMode, confineTo, cursor,
            button, modifiers);
        _buffer.Add(ref request);

        _requestLength++;
    }

    public void GrabKey(bool exposures, uint grabWindow, ModifierMask mask, byte keycode, GrabMode pointerMode,
        GrabMode keyboardMode)
    {
        var request = new GrabKeyType(exposures, grabWindow, mask, keycode, pointerMode, keyboardMode);
        _buffer.Add(ref request);

        _requestLength++;
    }

    public void GrabServer()
    {
        var request = new GrabServerType();
        _buffer.Add(ref request);
        _requestLength++;
    }

    public void ImageText16(uint drawable, uint gc, short x, short y, ReadOnlySpan<char> text)
    {
        var request = new ImageText16Type(drawable, gc, x, y, text.Length);
        _buffer.Add(ref request);
        _buffer.AddRange(Encoding.BigEndianUnicode.GetBytes(text.ToString()));
        _buffer.AddRange(new byte[text.Length.Padding()]);

        _requestLength++;
    }

    public void ImageText8(uint drawable, uint gc, short x, short y, ReadOnlySpan<byte> text)
    {
        var request = new ImageText8Type(drawable, gc, x, y, text.Length);
        _buffer.Add(ref request);
        _buffer.AddRange(text);
        _buffer.AddRange(new byte[text.Length.Padding()]);
        _requestLength++;
    }

    public void InstallColormap(uint colormapId)
    {
        var request = new InstallColormapType(colormapId);
        _buffer.Add(ref request);

        _requestLength++;
    }

    public void KillClient(uint resource)
    {
        var request = new KillClientType(resource);
        _buffer.Add(ref request);

        _requestLength++;
    }

    public void MapSubwindows(uint window)
    {
        var request = new MapSubWindowsType(window);
        _buffer.Add(ref request);

        _requestLength++;
    }

    public void MapWindow(uint window)
    {
        var request = new MapWindowType(window);
        _buffer.Add(ref request);

        _requestLength++;
    }

    public void NoOperation(params uint[] args)
    {
        var requiredBuffer = 4 + args.Length * 4;
        _buffer.AddRange([(byte)Opcode.NoOperation, 0]);
        _buffer.AddRange(BitConverter.GetBytes(requiredBuffer / 4));
        _buffer.AddRange(MemoryMarshal.Cast<uint, byte>(args));
        _requestLength++;
    }

    public void OpenFont(string fontName, uint fontId)
    {
        var request = new OpenFontType(fontId, fontName.Length);
        _buffer.Add(ref request);
        _buffer.AddRange(Encoding.ASCII.GetBytes(fontName));
        _buffer.AddRange(new byte[fontName.Length.Padding()]);
        _requestLength++;

    }

    public void PolyArc(uint drawable, uint gc, Arc[] arcs)
    {
        var request = new PolyArcType(drawable, gc, arcs.Length);
        _buffer.Add(ref request);
        _buffer.AddRange(MemoryMarshal.Cast<Arc, byte>(arcs));
        _requestLength++;
    }

    public void PolyFillArc(uint drawable, uint gc, Arc[] arcs)
    {
        var request = new PolyFillArcType(drawable, gc, arcs.Length);
        _buffer.Add(ref request);
        _buffer.AddRange(MemoryMarshal.Cast<Arc, byte>(arcs));
        _requestLength++;
    }

    public void PolyFillRectangle(uint drawable, uint gc, Rectangle[] rectangles)
    {
        var request = new PolyFillRectangleType(drawable, gc, rectangles.Length);
        _buffer.Add(ref request);
        _buffer.AddRange(MemoryMarshal.Cast<Rectangle, byte>(rectangles));
        _requestLength++;
    }

    public void PolyLine(CoordinateMode coordinate, uint drawable, uint gc, Point[] points)
    {
        var request = new PolyLineType(coordinate, drawable, gc, points.Length);
        _buffer.Add(ref request);
        _buffer.AddRange(MemoryMarshal.Cast<Point, byte>(points));
        _requestLength++;
    }

    public void PolyPoint(CoordinateMode coordinate, uint drawable, uint gc, Point[] points)
    {
        var request = new PolyPointType(coordinate, drawable, gc, points.Length);
        _buffer.Add(ref request);
        _buffer.AddRange(MemoryMarshal.Cast<Point, byte>(points));
        _requestLength++;
    }

    public void PolyRectangle(uint drawable, uint gc, Rectangle[] rectangles)
    {
        var request = new PolyRectangleType(drawable, gc, rectangles.Length);
        _buffer.Add(ref request);
        _buffer.AddRange(MemoryMarshal.Cast<Rectangle, byte>(rectangles));
        _requestLength++;
    }

    public void PolySegment(uint drawable, uint gc, Segment[] segments)
    {
        var request = new PolySegmentType(drawable, gc, segments.Length);
        _buffer.Add(ref request);
        _buffer.AddRange(MemoryMarshal.Cast<Segment, byte>(segments));
        _requestLength++;
    }

    public void PolyText16(uint drawable, uint gc, ushort x, ushort y, Span<byte> data)
    {
        var request = new PolyText16Type(drawable, gc, x, y, data.Length);
        _buffer.Add(ref request);
        _buffer.AddRange(data);
        _requestLength++;
    }

    public void PolyText8(uint drawable, uint gc, ushort x, ushort y, Span<byte> data)
    {
        var request = new PolyText8Type(drawable, gc, x, y, data.Length);
        _buffer.Add(ref request);
        _buffer.AddRange(data);
        _requestLength++;
    }

    public void PutImage(ImageFormatBitmap format, uint drawable, uint gc, ushort width, ushort height, short x, short y,
        byte leftPad, byte depth, Span<byte> data)
    {
        var request = new PutImageType(format, drawable, gc, width, height, x, y, leftPad, depth, data.Length);
        _buffer.Add(ref request);
        _buffer.AddRange(data);
        _buffer.AddRange(new byte[data.Length.Padding()]);
        _requestLength++;
    }

    public void RecolorCursor(uint cursorId, ushort foreRed, ushort foreGreen, ushort foreBlue, ushort backRed,
        ushort backGreen, ushort backBlue)
    {
        var request = new RecolorCursorType(cursorId, foreRed, foreGreen, foreBlue, backRed, backGreen, backBlue);
        _buffer.Add(ref request);
        _requestLength++;
    }

    public void ReparentWindow(uint window, uint parent, short x, short y)
    {
        var request = new ReparentWindowType(window, parent, x, y);
        _buffer.Add(ref request);
        _requestLength++;
    }

    public void RotateProperties(uint window, ushort delta, params uint[] properties)
    {
        var request = new RotatePropertiesType(window, properties.Length, delta);
        _buffer.Add(ref request);
        _buffer.AddRange(MemoryMarshal.Cast<uint, byte>(properties));
        _requestLength++;
    }

    public void SendEvent(bool propagate, uint destination, uint eventMask, XEvent evnt)
    {
        var request = new SendEventType(propagate, destination, eventMask, evnt);
        _buffer.Add(ref request);
        _requestLength++;
    }

    public void SetAccessControl(AccessControlMode mode)
    {
        var request = new SetAccessControlType(mode);
        _buffer.Add(ref request);
        _requestLength++;
    }

    public void SetClipRectangles(ClipOrdering ordering, uint gc, ushort clipX, ushort clipY, Rectangle[] rectangles)
    {
        var request = new SetClipRectanglesType(ordering, gc, clipX, clipY, rectangles.Length);
        _buffer.Add(ref request);
        _buffer.AddRange(MemoryMarshal.Cast<Rectangle, byte>(rectangles));
        _requestLength++;
    }

    public void SetCloseDownMode(CloseDownMode mode)
    {
        var request = new SetCloseDownModeType(mode);
        _buffer.Add(ref request);
        _requestLength++;
    }

    public void SetDashes(uint gc, ushort dashOffset, byte[] dashes)
    {
        var request = new SetDashesType(gc, dashOffset, dashes.Length);
        _buffer.Add(ref request);
        _buffer.AddRange(dashes);
        _buffer.AddRange(new byte[dashes.Length.Padding()]);
        _requestLength++;
    }

    public void SetFontPath(string[] strPaths)
    {
        var request = new SetFontPathType((ushort)strPaths.Length, strPaths.Sum(a => a.Length).AddPadding());
        _buffer.Add(ref request);
        foreach (var path in strPaths)
            _buffer.AddRange(Encoding.ASCII.GetBytes(path));
        _buffer.AddRange(new byte[strPaths.Sum(a => a.Length).Padding()]);
        _requestLength++;
    }

    public void SetInputFocus(InputFocusMode mode, uint focus, uint time)
    {
        var request = new SetInputFocusType(mode, focus, time);
        _buffer.Add(ref request);
        _requestLength++;
    }

    public void SetScreenSaver(short timeout, short interval, TriState preferBlanking, TriState allowExposures)
    {
        var request = new SetScreenSaverType(timeout, interval, preferBlanking, allowExposures);
        _buffer.Add(ref request);
        _requestLength++;
    }

    public void SetSelectionOwner(uint owner, uint atom, uint timestamp)
    {
        var request = new SetSelectionOwnerType(owner, atom, timestamp);
        _buffer.Add(ref request);
        _requestLength++;
    }

    public void StoreColors(uint colormapId, params ColorItem[] item)
    {
        var request = new StoreColorsType(colormapId, item.Length);
        _buffer.Add(ref request);
        _buffer.AddRange(MemoryMarshal.Cast<ColorItem, byte>(item));
        _buffer.Add(0);
        _requestLength++;
    }

    public void StoreNamedColor(ColorFlag mode, uint colormapId, uint pixels, ReadOnlySpan<byte> name)
    {
        var request = new StoreNamedColorType(mode, colormapId, pixels, name.Length);
        _buffer.Add(ref request);
        _buffer.AddRange(name);
        _buffer.AddRange(new byte[name.Length.Padding()]);
        _requestLength++;
    }

    public void UngrabButton(Button button, uint grabWindow, ModifierMask mask)
    {
        var request = new UngrabButtonType(button, grabWindow, mask);
        _buffer.Add(ref request);
        _requestLength++;
    }

    public void UngrabKey(byte key, uint grabWindow, ModifierMask modifier)
    {
        var request = new UngrabKeyType(key, grabWindow, modifier);
        _buffer.Add(ref request);
        _requestLength++;
    }

    public void UngrabKeyboard(uint time)
    {
        var request = new UngrabKeyboardType(time);
        _buffer.Add(ref request);
        _requestLength++;
    }

    public void UngrabPointer(uint time)
    {
        var request = new UngrabPointerType(time);
        _buffer.Add(ref request);
        _requestLength++;
    }

    public void UngrabServer()
    {
        var request = new UnGrabServerType();
        _buffer.Add(ref request);
        _requestLength++;
    }

    public void UninstallColormap(uint colormapId)
    {
        var request = new UninstallColormapType(colormapId);
        _buffer.Add(ref request);
        _requestLength++;
    }

    public void UnmapSubwindows(uint window)
    {
        var request = new UnMapSubwindowsType(window);
        _buffer.Add(ref request);
        _requestLength++;
    }

    public void UnmapWindow(uint window)
    {
        var request = new UnmapWindowType(window);
        _buffer.Add(ref request);
        _requestLength++;
    }

    public void WarpPointer(uint srcWindow, uint destinationWindow, short srcX, short srcY, ushort srcWidth,
        ushort srcHeight, short destinationX, short destinationY)
    {
        var request = new WarpPointerType(srcWindow, destinationWindow, srcX, srcY, srcWidth, srcHeight, destinationX,
            destinationY);
        _buffer.Add(ref request);
        _requestLength++;
    }
}