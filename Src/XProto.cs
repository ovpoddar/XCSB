using System;
using System.Buffers;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Net.Sockets;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using Xcsb.Helpers;
using Xcsb.Masks;
using Xcsb.Models;
using Xcsb.Models.Event;
using Xcsb.Models.Handshake;
using Xcsb.Models.Requests;
using Xcsb.Models.Response;
using static System.Net.Mime.MediaTypeNames;

namespace Xcsb;

[SkipLocalsInit]
internal class XProto : IXProto
{
    private readonly Socket _socket;
    private readonly HandshakeSuccessResponseBody _connectionResult;
    private bool _disposedValue;
    private int _globalId;
    private ushort _sequenceNumber;
    private Stack<XEvent> _bufferEvents;

    public HandshakeSuccessResponseBody HandshakeSuccessResponseBody => _connectionResult;

    public IXBufferProto BufferCLient => new XBufferProto(_socket);

    public XProto(Socket socket, HandshakeSuccessResponseBody connectionResult)
    {
        _socket = socket;
        _connectionResult = connectionResult;
        _globalId = 0;
        _sequenceNumber = 0;
        _bufferEvents = new Stack<XEvent>();
    }

    public AllocColorReply AllocColor(
        uint colorMap,
        ushort red,
        ushort green,
        ushort blue)
    {
        _sequenceNumber++;
        var request = new AllocColorType(colorMap, red, green, blue);
        _socket.Send(ref request);

        Span<byte> response = stackalloc byte[Marshal.SizeOf<AllocColorReply>()];
        _socket.ReceiveExact(response);
        return response.ToStruct<AllocColorReply>();
    }

    public void AllocColorCells()
    {
        _sequenceNumber++;
        throw new NotImplementedException();
    }

    public void AllocColorPlanes()
    {
        _sequenceNumber++;
        throw new NotImplementedException();
    }

    public void AllocNamedColor()
    {
        _sequenceNumber++;
        throw new NotImplementedException();
    }

    public void AllowEvents(
        EventsMode mode,
        uint time)
    {
        _sequenceNumber++;
        var request = new AllowEventsType(mode, time);
        CheckError();
        _socket.Send(ref request);
        CheckError();
    }

    public void Bell(sbyte percent)
    {
        _sequenceNumber++;
        if (percent is not <= 100 or not >= (-100))
            throw new ArgumentOutOfRangeException(nameof(percent), "value must be between -100 to 100");

        var request = new BellType(percent);
        CheckError();
        _socket.Send(ref request);
        CheckError();
    }

    public void ChangeActivePointerGrab(
        uint cursor,
        uint time,
        ushort mask)
    {
        _sequenceNumber++;
        var request = new ChangeActivePointerGrabType(cursor, time, mask);
        CheckError();
        _socket.Send(ref request);
        CheckError();
    }

    public void ChangeGC(
        uint gc,
        GCMask mask,
        params uint[] args)
    {
        _sequenceNumber++;
        var request = new ChangeGCType(gc, mask, args.Length);
        var requiredBuffer = 12 + args.Length * 4;
        CheckError();
        if (requiredBuffer < GlobalSetting.StackAllocThreshold)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
            MemoryMarshal.Write(scratchBuffer[0..12], request);
            MemoryMarshal.Cast<uint, byte>(args)
                .CopyTo(scratchBuffer[12..requiredBuffer]);
            _socket.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            MemoryMarshal.Write(scratchBuffer[0..12], request);
            MemoryMarshal.Cast<uint, byte>(args)
                .CopyTo(scratchBuffer[12..requiredBuffer]);
            _socket.SendExact(scratchBuffer[..requiredBuffer]);
        }
        CheckError();
    }

    public void ChangeHosts(
        HostMode mode,
        Family family,
        byte[] address)
    {
        _sequenceNumber++;
        var request = new ChangeHostsType(mode, family, address.Length);
        var requiredBuffer = 8 + address.Length.AddPadding();
        CheckError();
        if (requiredBuffer < GlobalSetting.StackAllocThreshold)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
            MemoryMarshal.Write(scratchBuffer[0..8], request);
            address.CopyTo(scratchBuffer[8..requiredBuffer]);
            scratchBuffer[^address.Length.Padding()..].Clear();
            _socket.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            MemoryMarshal.Write(scratchBuffer[0..8], request);
            address.CopyTo(scratchBuffer[8..requiredBuffer]);
            scratchBuffer[^address.Length.Padding()..].Clear();
            _socket.SendExact(scratchBuffer[..requiredBuffer]);
        }
        CheckError();
    }

    public void ChangeKeyboardControl(
        KeyboardControlMask mask,
        params uint[] args)
    {
        _sequenceNumber++;
        var requiredBuffer = 8 + args.Length * 4;
        var request = new ChangeKeyboardControlType(mask, args.Length);
        CheckError();
        if (requiredBuffer < GlobalSetting.StackAllocThreshold)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
            MemoryMarshal.Write(scratchBuffer[0..8], request);
            MemoryMarshal.Cast<uint, byte>(args).CopyTo(scratchBuffer[8..requiredBuffer]);
            _socket.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            MemoryMarshal.Write(scratchBuffer[0..8], request);
            MemoryMarshal.Cast<uint, byte>(args).CopyTo(scratchBuffer[8..requiredBuffer]);
            _socket.SendExact(scratchBuffer[..requiredBuffer]);
        }
        CheckError();
    }

    public void ChangeKeyboardMapping(
        byte keycodeCount,
        byte firstKeycode,
        byte keysymsPerKeycode,
        uint[] Keysym)
    {
        _sequenceNumber++;
        var requiredBuffer = 8 + (keycodeCount * keysymsPerKeycode) * 4;
        var request = new ChangeKeyboardMappingType(keycodeCount, firstKeycode, keysymsPerKeycode);
        CheckError();
        if (requiredBuffer < GlobalSetting.StackAllocThreshold)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
            MemoryMarshal.Write(scratchBuffer[0..8], request);
            MemoryMarshal.Cast<uint, byte>(Keysym).CopyTo(scratchBuffer[8..requiredBuffer]);
            _socket.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            MemoryMarshal.Write(scratchBuffer[0..8], request);
            MemoryMarshal.Cast<uint, byte>(Keysym).CopyTo(scratchBuffer[8..requiredBuffer]);
            _socket.SendExact(scratchBuffer[..requiredBuffer]);
        }
        CheckError();
    }

    public void ChangePointerControl(
        Acceleration? acceleration,
        ushort? threshold)
    {
        _sequenceNumber++;
        var request = new ChangePointerControlType(
            acceleration?.Numerator ?? 0,
            acceleration?.Denominator ?? 0,
            threshold ?? 0,
            (byte)(acceleration is null ? 0 : 1),
            (byte)(threshold.HasValue ? 1 : 0));
        CheckError();
        _socket.Send(ref request);
        CheckError();
    }

    void IVoidProto.ChangeProperty<T>(
        PropertyMode mode,
        uint window,
        uint property,
        uint type,
        params T[] args)
    {
        _sequenceNumber++;
        var size = Marshal.SizeOf<T>();
        if (size is not 1 or 2 or 4)
            throw new ArgumentException("type must be byte, sbyte, short, ushort, int, uint");
        var request = new ChangePropertyType(mode, window, property, type, args.Length, (byte)(size * 8));
        var requiredBuffer = 24 + (args.Length.AddPadding() * size);
        CheckError();
        if (requiredBuffer < GlobalSetting.StackAllocThreshold)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
            MemoryMarshal.Write(scratchBuffer[0..24], request);
            MemoryMarshal.Cast<T, byte>(args).CopyTo(scratchBuffer[24..(24 + args.Length * size)]);
            scratchBuffer[(24 + args.Length * size)..requiredBuffer].Clear();
            _socket.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            MemoryMarshal.Write(scratchBuffer[0..24], request);
            MemoryMarshal.Cast<T, byte>(args).CopyTo(scratchBuffer[24..(24 + args.Length * size)]);
            scratchBuffer[(24 + args.Length * size)..requiredBuffer].Clear();
            _socket.SendExact(scratchBuffer[..requiredBuffer]);
        }
        CheckError();
    }

    public void ChangeSaveSet(
        ChangeSaveSetMode changeSaveSetMode,
        uint window)
    {
        _sequenceNumber++;
        var request = new ChangeSaveSetType(changeSaveSetMode, window);
        CheckError();
        _socket.Send(ref request);
        CheckError();
    }

    public void ChangeWindowAttributes(
        uint window,
        ValueMask mask,
        params uint[] args)
    {
        _sequenceNumber++;
        var request = new ChangeWindowAttributesType(window, mask, args.Length);
        var requiredBuffer = 12 + args.Length * 4;
        CheckError();
        if (requiredBuffer < GlobalSetting.StackAllocThreshold)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
            MemoryMarshal.Write(scratchBuffer[0..12], request);
            MemoryMarshal.Cast<uint, byte>(args).CopyTo(scratchBuffer[12..requiredBuffer]);
            _socket.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            MemoryMarshal.Write(scratchBuffer[0..12], request);
            MemoryMarshal.Cast<uint, byte>(args).CopyTo(scratchBuffer[12..requiredBuffer]);
            _socket.SendExact(scratchBuffer[..requiredBuffer]);
        }
        CheckError();
    }

    public void CirculateWindow(
        Direction direction,
        uint window)
    {
        _sequenceNumber++;
        var request = new CirculateWindowType(direction, window);
        CheckError();
        _socket.Send(ref request);
        CheckError();
    }

    public void ClearArea(
        bool exposures,
        uint window,
        short x,
        short y,
        ushort width,
        ushort height)
    {
        _sequenceNumber++;
        var request = new ClearAreaType(exposures, window, x, y, width, height);
        CheckError();
        _socket.Send(ref request);
        CheckError();
    }

    public void CloseFont(uint fontId)
    {
        _sequenceNumber++;
        var request = new CloseFontType(fontId);
        CheckError();
        _socket.Send(ref request);
        CheckError();
    }

    public void ConfigureWindow(
        uint window,
        ConfigureValueMask mask,
        params uint[] args)
    {
        _sequenceNumber++;
        var requiredBuffer = 12 + args.Length * 4;
        var request = new ConfigureWindowType(window, mask, args.Length);
        CheckError();
        if (requiredBuffer < GlobalSetting.StackAllocThreshold)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
            MemoryMarshal.Write(scratchBuffer[0..12], request);
            MemoryMarshal.Cast<uint, byte>(args)
                .CopyTo(scratchBuffer[12..requiredBuffer]);
            _socket.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            MemoryMarshal.Write(scratchBuffer[0..12], request);
            MemoryMarshal.Cast<uint, byte>(args)
                .CopyTo(scratchBuffer[12..requiredBuffer]);
            _socket.SendExact(scratchBuffer[..requiredBuffer]);
        }
        CheckError();
    }

    public void ConvertSelection(
        uint requestor,
        uint selection,
        uint target,
        uint property,
        uint timestamp)
    {
        _sequenceNumber++;
        var request = new ConvertSelectionType(requestor, selection, target, property, timestamp);
        CheckError();
        _socket.Send(ref request);
        CheckError();
    }

    public void CopyArea(
        uint srcDrawable,
        uint destDrawable,
        uint gc,
        ushort srcX,
        ushort srcY,
        ushort destX,
        ushort destY,
        ushort width,
        ushort height)
    {
        _sequenceNumber++;
        var request = new CopyAreaType(srcDrawable, destDrawable, gc, srcX, srcY, destX, destY, width, height);
        CheckError();
        _socket.Send(ref request);
        CheckError();
    }

    public void CopyColormapAndFree(
        uint colormapId,
        uint srcColormapId)
    {
        _sequenceNumber++;
        var request = new CopyColormapAndFreeType(colormapId, srcColormapId);
        CheckError();
        _socket.Send(ref request);
        CheckError();
    }

    public void CopyGC(
        uint srcGc,
        uint dstGc,
        GCMask mask)
    {
        _sequenceNumber++;
        var request = new CopyGCType(srcGc, dstGc, mask);
        CheckError();
        _socket.Send(ref request);
        CheckError();
    }

    public void CopyPlane(
        uint srcDrawable,
        uint destDrawable,
        uint gc,
        ushort srcX,
        ushort srcY,
        ushort destX,
        ushort destY,
        ushort width,
        ushort height,
        uint bitPlane)
    {
        _sequenceNumber++;
        var request = new CopyPlaneType(srcDrawable, destDrawable, gc, srcX, srcY, destX, destY, width, height, bitPlane);
        CheckError();
        _socket.Send(ref request);
        CheckError();
    }

    public void CreateColormap(
        ColormapAlloc alloc,
        uint colormapId,
        uint window,
        uint visual)
    {
        _sequenceNumber++;
        var request = new CreateColormapType(alloc, colormapId, window, visual);
        CheckError();
        _socket.Send(ref request);
        CheckError();
    }

    public void CreateCursor(
        uint cursorId,
        uint source,
        uint mask,
        ushort foreRed,
        ushort foreGreen,
        ushort foreBlue,
        ushort backRed,
        ushort backGreen,
        ushort backBlue,
        ushort x,
        ushort y)
    {
        _sequenceNumber++;
        var request = new CreateCursorType(cursorId, source, mask, foreRed, foreGreen, foreBlue, backRed, backGreen, backBlue, x, y);
        CheckError();
        _socket.Send(ref request);
        CheckError();
    }

    public void CreateGC(
        uint gc,
        uint drawable,
        GCMask mask,
        params uint[] args)
    {
        _sequenceNumber++;
        var request = new CreateGCType(gc, drawable, mask, args.Length);
        var requiredBuffer = 16 + (args.Length * 4);
        CheckError();
        if (requiredBuffer < GlobalSetting.StackAllocThreshold)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
            MemoryMarshal.Write(scratchBuffer[0..16], request);
            MemoryMarshal.Cast<uint, byte>(args).CopyTo(scratchBuffer[16..requiredBuffer]);
            _socket.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            MemoryMarshal.Write(scratchBuffer[0..16], request);
            MemoryMarshal.Cast<uint, byte>(args).CopyTo(scratchBuffer[16..requiredBuffer]);
            _socket.SendExact(scratchBuffer[..requiredBuffer]);
        }
        CheckError();
    }

    public void CreateGlyphCursor(
        uint cursorId,
        uint sourceFont,
        uint fontMask,
        char sourceChar,
        ushort charMask,
        ushort foreRed,
        ushort foreGreen,
        ushort foreBlue,
        ushort backRed,
        ushort backGreen,
        ushort backBlue)
    {
        _sequenceNumber++;
        var request = new CreateGlyphCursorType(cursorId, sourceFont, fontMask, sourceChar, charMask, foreRed, foreGreen, foreBlue,
            backRed, backGreen, backBlue);
        CheckError();
        _socket.Send(ref request);
        CheckError();
    }

    public void CreatePixmap(
        byte depth,
        uint pixmapId,
        uint drawable,
        ushort width,
        ushort height)
    {
        _sequenceNumber++;
        var request = new CreatePixmapType(depth, pixmapId, drawable, width, height);
        CheckError();
        _socket.Send(ref request);
        CheckError();
    }

    public void CreateWindow(
        byte depth,
        uint window,
        uint parent,
        short x,
        short y,
        ushort width,
        ushort height,
        ushort borderWidth,
        ClassType classType,
        uint rootVisualId,
        ValueMask mask,
        params uint[] args)
    {
        _sequenceNumber++;
        var requiredBuffer = 32 + args.Length * 4;
        var request = new CreateWindowType(depth,
            window,
            parent,
            x, y, width, height,
            borderWidth,
            classType,
            rootVisualId,
            mask,
            args.Length);
        CheckError();
        if (requiredBuffer < GlobalSetting.StackAllocThreshold)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
            MemoryMarshal.Write(scratchBuffer[0..32], request);
            MemoryMarshal.Cast<uint, byte>(args)
                .CopyTo(scratchBuffer[32..requiredBuffer]);
            _socket.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            MemoryMarshal.Write(scratchBuffer[0..32], request);
            MemoryMarshal.Cast<uint, byte>(args)
                .CopyTo(scratchBuffer[32..requiredBuffer]);
            _socket.SendExact(scratchBuffer[..requiredBuffer]);
        }
        CheckError();
    }

    public void DeleteProperty(
        uint window,
        uint atom)
    {
        _sequenceNumber++;
        var request = new DeletePropertyType(window, atom);
        CheckError();
        _socket.Send(ref request);
        CheckError();
    }

    public void DestroySubwindows(uint window)
    {
        _sequenceNumber++;
        var request = new DestroySubWindowsType(window);
        CheckError();
        _socket.Send(ref request);
        CheckError();
    }

    public void DestroyWindow(uint window)
    {
        _sequenceNumber++;
        var request = new DestroyWindowType(window);
        CheckError();
        _socket.Send(ref request);
        CheckError();
    }

    public void FillPoly(
        uint drawable,
        uint gc,
        PolyShape shape,
        CoordinateMode coordinate,
        Point[] points)
    {
        _sequenceNumber++;
        var request = new FillPolyType(drawable, gc, shape, coordinate, points.Length);
        var requiredBuffer = 16 + (points.Length * 4);

        CheckError();
        if (requiredBuffer < GlobalSetting.StackAllocThreshold)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
            MemoryMarshal.Write(scratchBuffer[0..16], request);
            MemoryMarshal.Cast<Point, byte>(points)
                .CopyTo(scratchBuffer[16..]);
            _socket.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            MemoryMarshal.Write(scratchBuffer[0..16], request);
            MemoryMarshal.Cast<Point, byte>(points)
                .CopyTo(scratchBuffer[16..requiredBuffer]);
            _socket.SendExact(scratchBuffer[..requiredBuffer]);
        }
        CheckError();
    }

    public void ForceScreenSaver(ForceScreenSaverMode mode)
    {
        _sequenceNumber++;
        var request = new ForceScreenSaverType(mode);

        CheckError();
        _socket.Send(ref request);
        CheckError();
    }

    public void FreeColormap(uint colormapId)
    {
        _sequenceNumber++;
        var request = new FreeColormapType(colormapId);
        CheckError();
        _socket.Send(ref request);
        CheckError();
    }

    public void FreeColors(
        uint colormapId,
        uint planeMask,
        params uint[] pixels)
    {
        _sequenceNumber++;
        var requiredBuffer = 12 + pixels.Length * 4;
        var request = new FreeColorsType(colormapId, planeMask, pixels.Length);
        CheckError();
        if (requiredBuffer < GlobalSetting.StackAllocThreshold)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
            MemoryMarshal.Write(scratchBuffer[0..12], request);
            MemoryMarshal.Cast<uint, byte>(pixels).CopyTo(scratchBuffer[12..requiredBuffer]);
            _socket.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            MemoryMarshal.Write(scratchBuffer[0..12], request);
            MemoryMarshal.Cast<uint, byte>(pixels).CopyTo(scratchBuffer[12..requiredBuffer]);
            _socket.SendExact(scratchBuffer[..requiredBuffer]);
        }
        CheckError();
    }

    public void FreeCursor(uint cursorId)
    {
        _sequenceNumber++;
        var request = new FreeCursorType(cursorId);
        CheckError();
        _socket.Send(ref request);
        CheckError();
    }

    public void FreeGC(uint gc)
    {
        _sequenceNumber++;
        var request = new FreeGCType(gc);
        CheckError();
        _socket.Send(ref request);
        CheckError();
    }

    public void FreePixmap(uint pixmapId)
    {
        _sequenceNumber++;
        var request = new FreePixmapType(pixmapId);
        CheckError();
        _socket.Send(ref request);
        CheckError();
    }

    public void GetAtomName()
    {
        _sequenceNumber++;
        throw new NotImplementedException();
    }

    public InternAtomReply InternAtom(
        bool onlyIfExist,
        string atomName)
    {
        _sequenceNumber++;
        var request = new InternAtomType(onlyIfExist, atomName.Length);
        var requestSize = Marshal.SizeOf<InternAtomType>();
        var requiredBuffer = requestSize + atomName.Length.AddPadding();
        if (requiredBuffer < GlobalSetting.StackAllocThreshold)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
            MemoryMarshal.Write(scratchBuffer[0..requestSize], in request);
            Encoding.ASCII.GetBytes(atomName, scratchBuffer.Slice(requestSize, atomName.Length));
            scratchBuffer[(requestSize + atomName.Length)..requiredBuffer].Clear();

            _socket.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            MemoryMarshal.Write(scratchBuffer[0..requestSize], in request);
            Encoding.ASCII.GetBytes(atomName, scratchBuffer.Slice(requestSize, atomName.Length));
            scratchBuffer[(requestSize + atomName.Length)..requiredBuffer].Clear();
            _socket.SendExact(scratchBuffer[..requiredBuffer]);
        }

        Span<byte> responce = stackalloc byte[Marshal.SizeOf<InternAtomReply>()];
        _socket.ReceiveExact(responce);
        var c = responce.ToStruct<InternAtomReply>();
        Debug.Assert(c.SequenceNumber == _sequenceNumber);
        return c;
    }

    public void GetFontPath()
    {
        _sequenceNumber++;
        throw new NotImplementedException();
    }

    public void GetGeometry()
    {
        _sequenceNumber++;
        throw new NotImplementedException();
    }

    public void GetImage()
    {
        _sequenceNumber++;
        throw new NotImplementedException();
    }

    public void GetInputFocus()
    {
        _sequenceNumber++;
        throw new NotImplementedException();
    }

    public void GetKeyboardControl()
    {
        _sequenceNumber++;
        throw new NotImplementedException();
    }

    public void GetKeyboardMapping()
    {
        _sequenceNumber++;
        throw new NotImplementedException();
    }

    public void GetModifierMapping()
    {
        _sequenceNumber++;
        throw new NotImplementedException();
    }

    public void GetMotionEvents()
    {
        _sequenceNumber++;
        throw new NotImplementedException();
    }

    public void GetPointerControl()
    {
        _sequenceNumber++;
        throw new NotImplementedException();
    }

    public void GetPointerMapping()
    {
        _sequenceNumber++;
        throw new NotImplementedException();
    }

    public GetPropertyReply GetProperty(
        bool delete,
        uint window,
        uint property,
        uint type,
        uint offset,
        uint length)
    {
        _sequenceNumber++;
        var request = new GetPropertyType(delete, window, property, type, offset, length);
        _socket.Send(ref request);
        return new GetPropertyReply(_socket);
    }

    public void GetScreenSaver()
    {
        _sequenceNumber++;
        throw new NotImplementedException();
    }

    public void GetSelectionOwner()
    {
        _sequenceNumber++;
        throw new NotImplementedException();
    }

    public void GetWindowAttributes()
    {
        _sequenceNumber++;
        throw new NotImplementedException();
    }

    public void GrabButton(
        bool ownerEvents,
        uint grabWindow,
        ushort mask,
        GrabMode pointerMode,
        GrabMode keyboardMode,
        uint confineTo,
        uint cursor,
        Button button,
        ModifierMask modifiers)
    {
        _sequenceNumber++;
        var request = new GrabButtonType(ownerEvents, grabWindow, mask, pointerMode, keyboardMode, confineTo, cursor, button, modifiers);
        CheckError();
        _socket.Send(ref request);
        CheckError();
    }

    public void GrabKey(
        bool exposures,
        uint grabWindow,
        ModifierMask mask,
        byte keycode,
        GrabMode pointerMode,
        GrabMode keyboardMode)
    {
        _sequenceNumber++;
        var request = new GrabKeyType(exposures, grabWindow, mask, keycode, pointerMode, keyboardMode);
        CheckError();
        _socket.Send(ref request);
        CheckError();
    }

    public void GrabKeyboard()
    {
        _sequenceNumber++;
        throw new NotImplementedException();
    }

    public GrabPointerReply GrabPointer(
        bool ownerEvents,
        uint grabWindow,
        ushort mask,
        GrabMode pointerMode,
        GrabMode keyboardMode,
        uint confineTo,
        uint cursor,
        uint timeStamp)
    {
        _sequenceNumber++;
        var request = new GrabPointerType(ownerEvents, grabWindow, mask, pointerMode, keyboardMode, confineTo, cursor, timeStamp);
        _socket.Send(ref request);

        Span<byte> response = stackalloc byte[Marshal.SizeOf<GrabPointerReply>()];
        _socket.ReceiveExact(response);
        return response.ToStruct<GrabPointerReply>();
    }

    public void GrabServer()
    {
        _sequenceNumber++;
        var request = new GrabServerType();
        CheckError();
        _socket.Send(ref request);
        CheckError();
    }

    public void ImageText16(
        uint drawable,
        uint gc,
        short x,
        short y,
        ReadOnlySpan<char> text)
    {
        _sequenceNumber++;
        var request = new ImageText16Type(drawable, gc, x, y, text.Length);
        var requiredBuffer = 16 + (text.Length * 2).AddPadding();
        CheckError();
        if (requiredBuffer < GlobalSetting.StackAllocThreshold)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
            MemoryMarshal.Write(scratchBuffer[0..16], request);
            Encoding.BigEndianUnicode.GetBytes(text, scratchBuffer[16..(text.Length * 2 + 16)]);
            scratchBuffer[(16 + text.Length * 2)..requiredBuffer].Clear();
            _socket.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            MemoryMarshal.Write(scratchBuffer[0..16], request);
            Encoding.BigEndianUnicode.GetBytes(text, scratchBuffer[16..(text.Length * 2 + 16)]);
            scratchBuffer[(16 + text.Length * 2)..requiredBuffer].Clear();
            _socket.SendExact(scratchBuffer[..requiredBuffer]);
        }
        CheckError();
    }

    public void ImageText8(
        uint drawable,
        uint gc,
        short x,
        short y,
        ReadOnlySpan<byte> text)
    {
        _sequenceNumber++;
        var request = new ImageText8Type(drawable, gc, x, y, text.Length);
        var requiredBuffer = 16 + text.Length.AddPadding();
        if (requiredBuffer < GlobalSetting.StackAllocThreshold)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
            MemoryMarshal.Write(scratchBuffer[0..16], request);
            text.CopyTo(scratchBuffer[16..(text.Length + 16)]);
            scratchBuffer[(16 + text.Length)..requiredBuffer].Clear();
            _socket.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            MemoryMarshal.Write(scratchBuffer[0..16], request);
            text.CopyTo(scratchBuffer[16..(text.Length + 16)]);
            scratchBuffer[(16 + text.Length)..requiredBuffer].Clear();
            _socket.SendExact(scratchBuffer[..requiredBuffer]);
        }
    }

    public void InstallColormap(uint colormapId)
    {
        _sequenceNumber++;
        var request = new InstallColormapType(colormapId);
        CheckError();
        _socket.Send(ref request);
        CheckError();
    }

    public void KillClient(uint resource)
    {
        _sequenceNumber++;
        var request = new KillClientType(resource);
        CheckError();
        _socket.Send(ref request);
        CheckError();
    }

    public void ListExtensions()
    {
        _sequenceNumber++;
        throw new NotImplementedException();
    }

    public void ListFonts()
    {
        _sequenceNumber++;
        throw new NotImplementedException();
    }

    public void ListFontsWithInfo()
    {
        _sequenceNumber++;
        throw new NotImplementedException();
    }

    public void ListHosts()
    {
        _sequenceNumber++;
        throw new NotImplementedException();
    }

    public void ListInstalledColormaps()
    {
        _sequenceNumber++;
        throw new NotImplementedException();
    }

    public void ListProperties()
    {
        _sequenceNumber++;
        throw new NotImplementedException();
    }

    public void LookupColor()
    {
        _sequenceNumber++;
        throw new NotImplementedException();
    }

    public void MapSubwindows(uint window)
    {
        _sequenceNumber++;
        var request = new MapSubWindowsType(window);
        CheckError();
        _socket.Send(ref request);
        CheckError();
    }

    public void MapWindow(uint window)
    {
        _sequenceNumber++;
        var request = new MapWindowType(window);
        CheckError();
        _socket.Send(ref request);
        CheckError();
    }

    public void NoOperation(params uint[] args)
    {
        _sequenceNumber++;
        var requiredBuffer = 4 + args.Length * 4;
        CheckError();
        if (requiredBuffer < GlobalSetting.StackAllocThreshold)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
            scratchBuffer[0] = (byte)Opcode.NoOperation;
            scratchBuffer[1] = 0;
            MemoryMarshal.Write(scratchBuffer[2..4], (ushort)(requiredBuffer / 4));
            args.AsSpan().CopyTo(MemoryMarshal.Cast<byte, uint>(scratchBuffer[4..requiredBuffer]));
            _socket.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            scratchBuffer[0] = (byte)Opcode.NoOperation;
            scratchBuffer[1] = 0;
            MemoryMarshal.Write(scratchBuffer[2..4], (ushort)(requiredBuffer / 4));
            args.AsSpan().CopyTo(MemoryMarshal.Cast<byte, uint>(scratchBuffer[4..requiredBuffer]));
            _socket.SendExact(scratchBuffer[..requiredBuffer]);
        }
        CheckError();
    }

    public void OpenFont(
        string fontName,
        uint fontId)
    {
        _sequenceNumber++;
        var request = new OpenFontType(fontId, (ushort)fontName.Length);
        var requestSize = Marshal.SizeOf<OpenFontType>();
        var requiredBuffer = requestSize + fontName.Length.AddPadding();
        CheckError();
        if (requiredBuffer < GlobalSetting.StackAllocThreshold)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
            MemoryMarshal.Write(scratchBuffer[0..requestSize], in request);
            Encoding.ASCII.GetBytes(fontName, scratchBuffer.Slice(requestSize, fontName.Length));
            scratchBuffer[(requestSize + fontName.Length)..requiredBuffer].Clear();

            _socket.SendExact(scratchBuffer);
        }
        else
        {
            requiredBuffer -= requestSize;

            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            Encoding.ASCII.GetBytes(fontName, scratchBuffer.Slice(fontName.Length));
            scratchBuffer[(fontName.Length)..requiredBuffer].Clear();

            _socket.Send(ref request);
            _socket.SendExact(scratchBuffer[..requiredBuffer]);
        }
        CheckError();
    }

    public void PolyArc(
        uint drawable,
        uint gc,
        Arc[] arcs)
    {
        _sequenceNumber++;
        var request = new PolyArcType(drawable, gc, arcs.Length);
        var requiredBuffer = Marshal.SizeOf<PolyArcType>() + (arcs.Length * 12);
        CheckError();
        if (requiredBuffer < GlobalSetting.StackAllocThreshold)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
            MemoryMarshal.Write(scratchBuffer[0..12], request);
            MemoryMarshal.Cast<Arc, byte>(arcs)
                .CopyTo(scratchBuffer[12..requiredBuffer]);
            _socket.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            MemoryMarshal.Write(scratchBuffer[0..12], request);
            MemoryMarshal.Cast<Arc, byte>(arcs)
                .CopyTo(scratchBuffer[12..requiredBuffer]);
            _socket.SendExact(scratchBuffer[..requiredBuffer]);
        }
        CheckError();
    }

    public void PolyFillArc(
        uint drawable,
        uint gc,
        Arc[] arcs)
    {
        _sequenceNumber++;
        var request = new PolyFillArcType(drawable, gc, arcs.Length);
        var requiredBuffer = Marshal.SizeOf<PolyFillArcType>() + (arcs.Length * 12);
        CheckError();
        if (requiredBuffer < GlobalSetting.StackAllocThreshold)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
            MemoryMarshal.Write(scratchBuffer[0..12], request);
            MemoryMarshal.Cast<Arc, byte>(arcs)
                .CopyTo(scratchBuffer[12..requiredBuffer]);
            _socket.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            MemoryMarshal.Write(scratchBuffer[0..12], request);
            MemoryMarshal.Cast<Arc, byte>(arcs)
                .CopyTo(scratchBuffer[12..requiredBuffer]);
            _socket.SendExact(scratchBuffer[..requiredBuffer]);
        }
        CheckError();
    }

    public void PolyFillRectangle(
        uint drawable,
        uint gc,
        Rectangle[] rectangles)
    {
        _sequenceNumber++;
        var request = new PolyFillRectangleType(drawable, gc, rectangles.Length);
        var requiredBuffer = Marshal.SizeOf<PolyFillRectangleType>() + (rectangles.Length * 8);
        CheckError();
        if (requiredBuffer < GlobalSetting.StackAllocThreshold)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
            MemoryMarshal.Write(scratchBuffer[0..12], request);
            MemoryMarshal.Cast<Rectangle, byte>(rectangles)
                .CopyTo(scratchBuffer[12..(12 + rectangles.Length * 8)]);
            _socket.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            MemoryMarshal.Write(scratchBuffer[0..12], request);
            MemoryMarshal.Cast<Rectangle, byte>(rectangles)
                .CopyTo(scratchBuffer[12..(12 + rectangles.Length * 8)]);
            _socket.SendExact(scratchBuffer[..requiredBuffer]);
        }
        CheckError();
    }

    public void PolyLine(
        CoordinateMode coordinate,
        uint drawable,
        uint gc,
        Point[] points)
    {
        _sequenceNumber++;
        var request = new PolyLineType(coordinate, drawable, gc, points.Length);
        var requiredBuffer = 12 + (points.Length * 4);
        CheckError();
        if (requiredBuffer < GlobalSetting.StackAllocThreshold)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
            MemoryMarshal.Write(scratchBuffer[0..12], request);
            MemoryMarshal.Cast<Point, byte>(points)
                .CopyTo(scratchBuffer[12..requiredBuffer]);
            _socket.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            MemoryMarshal.Write(scratchBuffer[0..12], request);
            MemoryMarshal.Cast<Point, byte>(points)
                .CopyTo(scratchBuffer[12..requiredBuffer]);
            _socket.SendExact(scratchBuffer[..requiredBuffer]);
        }
        CheckError();
    }

    public void PolyPoint(
        CoordinateMode coordinate,
        uint drawable,
        uint gc,
        Point[] points)
    {
        _sequenceNumber++;
        var request = new PolyPointType(coordinate, drawable, gc, points.Length);
        var requiredBuffer = 12 + (points.Length * 4);
        CheckError();
        if (requiredBuffer < GlobalSetting.StackAllocThreshold)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
            MemoryMarshal.Write(scratchBuffer[0..12], request);
            MemoryMarshal.Cast<Point, byte>(points)
                .CopyTo(scratchBuffer[12..requiredBuffer]);
            _socket.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            MemoryMarshal.Write(scratchBuffer[0..12], request);
            MemoryMarshal.Cast<Point, byte>(points)
                .CopyTo(scratchBuffer[12..requiredBuffer]);
            _socket.SendExact(scratchBuffer[..requiredBuffer]);
        }
        CheckError();
    }

    public void PolyRectangle(
        uint drawable,
        uint gc,
        Rectangle[] rectangles)
    {
        _sequenceNumber++;
        var request = new PolyRectangleType(drawable, gc, rectangles.Length);
        var requiredBuffer = 12 + (rectangles.Length * 8);
        CheckError();
        if (requiredBuffer < GlobalSetting.StackAllocThreshold)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
            MemoryMarshal.Write(scratchBuffer[0..12], request);
            MemoryMarshal.Cast<Rectangle, byte>(rectangles)
                .CopyTo(scratchBuffer[12..requiredBuffer]);
            _socket.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            MemoryMarshal.Write(scratchBuffer[0..12], request);
            MemoryMarshal.Cast<Rectangle, byte>(rectangles)
                .CopyTo(scratchBuffer[12..requiredBuffer]);
            _socket.SendExact(scratchBuffer[..requiredBuffer]);
        }
        CheckError();
    }

    public void PolySegment(
        uint drawable,
        uint gc,
        Segment[] segments)
    {
        _sequenceNumber++;
        var request = new PolySegmentType(drawable, gc, segments.Length);
        var requiredBuffer = 12 + (segments.Length * 8);
        CheckError();
        if (requiredBuffer < GlobalSetting.StackAllocThreshold)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
            MemoryMarshal.Write(scratchBuffer[0..12], request);
            MemoryMarshal.Cast<Segment, byte>(segments)
                .CopyTo(scratchBuffer[12..requiredBuffer]);
            _socket.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            MemoryMarshal.Write(scratchBuffer[0..12], request);
            MemoryMarshal.Cast<Segment, byte>(segments)
                .CopyTo(scratchBuffer[12..requiredBuffer]);
            _socket.SendExact(scratchBuffer[..requiredBuffer]);
        }
        CheckError();
    }

    public void PolyText16()
    {
        _sequenceNumber++;
        throw new NotImplementedException();
    }

    public void PolyText8()
    {
        _sequenceNumber++;
        throw new NotImplementedException();
    }

    public void PutImage(
        ImageFormat format,
        uint drawable,
        uint gc,
        ushort width,
        ushort height,
        short x,
        short y,
        byte leftPad,
        byte depth,
        Span<byte> data)
    {
        _sequenceNumber++;
        var request = new PutImageType(
            format,
            drawable,
            gc, width, height, x, y,
            leftPad, depth,
            data.Length);
        var scratchBufferSize = data.Length.AddPadding() + 24;
        CheckError();
        if (scratchBufferSize < GlobalSetting.StackAllocThreshold)
        {
            Span<byte> scratchBuffer = stackalloc byte[scratchBufferSize];
            MemoryMarshal.Write(scratchBuffer[..24], in request);
            data.CopyTo(scratchBuffer[24..(24 + data.Length)]);
            scratchBuffer[(24 + data.Length)..].Clear();
            _socket.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(scratchBufferSize);
            MemoryMarshal.Write(scratchBuffer[..24], in request);
            data.CopyTo(scratchBuffer[24..(24 + data.Length)]);
            scratchBuffer[(24 + data.Length)..].Clear();
            _socket.SendExact(scratchBuffer[..scratchBufferSize]);
        }
        CheckError();
    }

    public void QueryBestSize()
    {
        _sequenceNumber++;
        throw new NotImplementedException();
    }

    public void QueryColors()
    {
        _sequenceNumber++;
        throw new NotImplementedException();
    }

    public void QueryExtension()
    {
        _sequenceNumber++;
        throw new NotImplementedException();
    }

    public void QueryFont()
    {
        _sequenceNumber++;
        throw new NotImplementedException();
    }

    public void QueryKeymap()
    {
        _sequenceNumber++;
        throw new NotImplementedException();
    }

    public QueryPointerReply QueryPointer(uint window)
    {
        _sequenceNumber++;
        var request = new QueryPointerType(window);
        _socket.Send(ref request);

        Span<byte> response = stackalloc byte[Marshal.SizeOf<QueryPointerReply>()];
        _socket.ReceiveExact(response);
        return response.ToStruct<QueryPointerReply>();
    }

    public void QueryTextExtents()
    {
        _sequenceNumber++;
        throw new NotImplementedException();
    }

    public void QueryTree()
    {
        _sequenceNumber++;
        throw new NotImplementedException();
    }

    public void RecolorCursor(
        uint cursorId,
        ushort foreRed,
        ushort foreGreen,
        ushort foreBlue,
        ushort backRed,
        ushort backGreen,
        ushort backBlue)
    {
        _sequenceNumber++;
        var request = new RecolorCursorType(cursorId, foreRed, foreGreen, foreBlue, backRed, backGreen, backBlue);
        CheckError();
        _socket.Send(ref request);
        CheckError();
    }

    public void ReparentWindow(
        uint window,
        uint parent,
        short x,
        short y)
    {
        _sequenceNumber++;
        var request = new ReparentWindowType(window, parent, x, y);
        CheckError();
        _socket.Send(ref request);
        CheckError();
    }

    public void RotateProperties(
        uint window,
        ushort delta,
        params uint[] properties)
    {
        _sequenceNumber++;
        var request = new RotatePropertiesType(window, properties.Length, delta);
        var requiredBuffer = 12 + properties.Length * 4;
        CheckError();
        if (requiredBuffer < GlobalSetting.StackAllocThreshold)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
            MemoryMarshal.Write(scratchBuffer[0..12], request);
            MemoryMarshal.Cast<uint, byte>(properties).
                CopyTo(scratchBuffer[12..requiredBuffer]);
            _socket.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            MemoryMarshal.Write(scratchBuffer[0..12], request);
            MemoryMarshal.Cast<uint, byte>(properties)
                .CopyTo(scratchBuffer[12..requiredBuffer]);
            _socket.SendExact(scratchBuffer[..requiredBuffer]);
        }
        CheckError();
    }

    public void SendEvent(
        bool propagate,
        uint destination,
        uint eventMask,
        XEvent evnt)
    {
        _sequenceNumber++;
        var request = new SendEventType(propagate, destination, eventMask, evnt);
        CheckError();
        _socket.Send(ref request);
        CheckError();
    }

    public void SetAccessControl(AccessControlMode mode)
    {
        _sequenceNumber++;
        var request = new SetAccessControlType(mode);
        CheckError();
        _socket.Send(ref request);
        CheckError();
    }

    public void SetClipRectangles(
        ClipOrdering ordering,
        uint gc,
        ushort clipX,
        ushort clipY,
        Rectangle[] rectangles)
    {
        _sequenceNumber++;
        var request = new SetClipRectanglesType(ordering, gc, clipX, clipY, rectangles.Length);
        var requiredBuffer = 12 + rectangles.Length * Marshal.SizeOf<Rectangle>();
        CheckError();
        if (requiredBuffer < GlobalSetting.StackAllocThreshold)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
            MemoryMarshal.Write(scratchBuffer[0..12], request);
            MemoryMarshal.Cast<Rectangle, byte>(rectangles).
                CopyTo(scratchBuffer[12..requiredBuffer]);
            _socket.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            MemoryMarshal.Write(scratchBuffer[0..12], request);
            MemoryMarshal.Cast<Rectangle, byte>(rectangles).
                CopyTo(scratchBuffer[12..requiredBuffer]);
            _socket.SendExact(scratchBuffer[..requiredBuffer]);
        }
        CheckError();
    }

    public void SetCloseDownMode(CloseDownMode mode)
    {
        _sequenceNumber++;
        var request = new SetCloseDownModeType(mode);
        CheckError();
        _socket.Send(ref request);
        CheckError();
    }

    public void SetDashes(
        uint gc,
        ushort dashOffset,
        byte[] dashes)
    {
        _sequenceNumber++;
        var request = new SetDashesType(gc, dashOffset, dashes.Length);
        var requiredBuffer = 12 + dashes.Length.AddPadding();
        CheckError();
        if (requiredBuffer < GlobalSetting.StackAllocThreshold)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
            MemoryMarshal.Write(scratchBuffer[0..12], request);
            dashes.CopyTo(scratchBuffer[12..requiredBuffer]);
            scratchBuffer[^dashes.Length.Padding()..].Clear();
            _socket.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            MemoryMarshal.Write(scratchBuffer[0..12], request);
            dashes.CopyTo(scratchBuffer[12..requiredBuffer]);
            scratchBuffer[^dashes.Length.Padding()..].Clear();
            _socket.SendExact(scratchBuffer[..requiredBuffer]);
        }
        CheckError();
    }

    public void SetFontPath(string[] strPaths)
    {
        _sequenceNumber++;
        var request = new SetFontPathType((ushort)strPaths.Length, strPaths.Sum(a => a.Length).AddPadding());
        var requiredBuffer = 8 + strPaths.Sum(a => a.Length).AddPadding();
        var writIndex = 8;
        CheckError();
        if (requiredBuffer < GlobalSetting.StackAllocThreshold)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
            MemoryMarshal.Write(scratchBuffer[0..8], request);
            foreach (var item in strPaths)
            {
                Encoding.ASCII.GetBytes(item, scratchBuffer.Slice(writIndex, item.Length));
                writIndex += item.Length;
            }
            scratchBuffer[^strPaths.Sum(a => a.Length).Padding()..].Clear();
            _socket.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            MemoryMarshal.Write(scratchBuffer[0..8], request);
            foreach (var item in strPaths)
            {
                Encoding.ASCII.GetBytes(item, scratchBuffer.Slice(writIndex, item.Length));
                writIndex += item.Length;
            }
            scratchBuffer[^strPaths.Sum(a => a.Length).Padding()..].Clear();
            _socket.SendExact(scratchBuffer[..requiredBuffer]);
        }
        CheckError();
    }

    public void SetInputFocus(
        InputFocusMode mode,
        uint focus,
        uint time)
    {
        _sequenceNumber++;
        var request = new SetInputFocusType(mode, focus, time);
        CheckError();
        _socket.Send(ref request);
        CheckError();
    }

    public void SetModifierMapping()
    {
        _sequenceNumber++;
        throw new NotImplementedException();
    }

    public void SetPointerMapping()
    {
        _sequenceNumber++;
        throw new NotImplementedException();
    }

    public void SetScreenSaver(
        short timeout,
        short interval,
        TriState preferBlanking,
        TriState allowExposures)
    {
        _sequenceNumber++;
        var request = new SetScreenSaverType(timeout, interval, preferBlanking, allowExposures);
        CheckError();
        _socket.Send(ref request);
        CheckError();
    }

    public void SetSelectionOwner(
        uint owner,
        uint atom,
        uint timestamp)
    {
        _sequenceNumber++;
        var request = new SetSelectionOwnerType(owner, atom, timestamp);
        CheckError();
        _socket.Send(ref request);
        CheckError();
    }

    public void StoreColors(
        uint colormapId,
        params ColorItem[] item)
    {
        _sequenceNumber++;
        var request = new StoreColorsType(colormapId, item.Length);
        var requiredBuffer = 8 + 12 * item.Length;
        CheckError();
        if (requiredBuffer < GlobalSetting.StackAllocThreshold)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
            MemoryMarshal.Write(scratchBuffer[0..8], request);
            item.CopyTo(MemoryMarshal.Cast<byte, ColorItem>(scratchBuffer[8..requiredBuffer]));
            scratchBuffer[^1] = 0;
            _socket.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            MemoryMarshal.Write(scratchBuffer[0..8], request);
            item.CopyTo(MemoryMarshal.Cast<byte, ColorItem>(scratchBuffer[8..requiredBuffer]));
            scratchBuffer[requiredBuffer - 1] = 0;
            _socket.SendExact(scratchBuffer[..requiredBuffer]);
        }
        CheckError();
    }

    public void StoreNamedColor(
        ColorFlag mode,
        uint colormapId,
        uint pixels,
        ReadOnlySpan<byte> name)
    {
        _sequenceNumber++;
        var request = new StoreNamedColorType(mode, colormapId, pixels, name.Length);
        var requiredBuffer = 16 + name.Length.AddPadding();
        CheckError();
        if (requiredBuffer < GlobalSetting.StackAllocThreshold)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
            MemoryMarshal.Write(scratchBuffer[0..16], request);
            name.CopyTo(scratchBuffer[16..(name.Length + 16)]);
            scratchBuffer[^name.Length.Padding()..].Clear();
            _socket.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            MemoryMarshal.Write(scratchBuffer[0..16], request);
            name.CopyTo(scratchBuffer[16..(name.Length + 16)]);
            scratchBuffer[^name.Length.Padding()..].Clear();
            _socket.SendExact(scratchBuffer[..requiredBuffer]);
        }
        CheckError();
    }

    public void TranslateCoordinates()
    {
        _sequenceNumber++;
        throw new NotImplementedException();
    }

    public void UngrabButton(
        Button button,
        uint grabWindow,
        ModifierMask mask)
    {
        _sequenceNumber++;
        var request = new UngrabButtonType(button, grabWindow, mask);
        CheckError();
        _socket.Send(ref request);
        CheckError();
    }

    public void UngrabKey(
        byte key,
        uint grabWindow,
        ModifierMask modifier)
    {
        _sequenceNumber++;
        var request = new UngrabKeyType(key, grabWindow, modifier);
        CheckError();
        _socket.Send(ref request);
        CheckError();
    }

    public void UngrabKeyboard(uint time)
    {
        _sequenceNumber++;
        var request = new UngrabKeyboardType(time);
        CheckError();
        _socket.Send(ref request);
        CheckError();
    }

    public void UngrabPointer(uint time)
    {
        _sequenceNumber++;
        var request = new UngrabPointerType(time);
        CheckError();
        _socket.Send(ref request);
        CheckError();
    }

    public void UngrabServer()
    {
        _sequenceNumber++;
        var request = new UnGrabServerType();
        CheckError();
        _socket.Send(ref request);
        CheckError();
    }

    public void UninstallColormap(uint colormapId)
    {
        _sequenceNumber++;
        var request = new UninstallColormapType(colormapId);
        CheckError();
        _socket.Send(ref request);
        CheckError();
    }

    public void UnmapSubwindows(uint window)
    {
        _sequenceNumber++;
        var request = new UnMapSubwindowsType(window);
        CheckError();
        _socket.Send(ref request);
        CheckError();
    }

    public void UnmapWindow(uint window)
    {
        _sequenceNumber++;
        var request = new UnmapWindowType(window);
        CheckError();
        _socket.Send(ref request);
        CheckError();
    }

    public void WarpPointer(
        uint srcWindow,
        uint destWindow,
        short srcX,
        short srcY,
        ushort srcWidth,
        ushort srcHeight,
        short destX,
        short destY)
    {
        _sequenceNumber++;
        var request = new WarpPointerType(srcWindow, destWindow, srcX, srcY, srcWidth, srcHeight, destX, destY);
        CheckError();
        _socket.Send(ref request);
        CheckError();
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
                if (_socket.Connected) _socket.Close();

            _disposedValue = true;
        }
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    public uint NewId() =>
        (uint)(_connectionResult.ResourceIDMask & _globalId++ | _connectionResult.ResourceIDBase);

    public XEvent GetEvent()
    {
        if (_bufferEvents.TryPop(out var result))
        {
            return result;
        }

        Span<byte> scratchBuffer = stackalloc byte[Marshal.SizeOf<XEvent>()];
        if (_socket.Poll(-1, SelectMode.SelectRead))
        {
            var totalRead = _socket.Receive(scratchBuffer);
            if (totalRead != 0)
                return scratchBuffer.ToStruct<XEvent>();
        }

        scratchBuffer[0] = 0;
        return scratchBuffer.ToStruct<XEvent>();
    }

    private void CheckError([CallerMemberName] string name = "")
    {
        if (_socket.Available == 0)
            return;

        if (_socket.Available % Marshal.SizeOf<XEvent>() != 0)
            throw new UnreachableException(); // if here then some request is not 32 bytes

        Span<byte> buffer = stackalloc byte[Marshal.SizeOf<XEvent>()];
        while (_socket.Available != 0)
        {
            _socket.ReceiveExact(buffer);
            ref var content = ref buffer.AsStruct<XEvent>();
            if ((int)content.EventType == 1)
            {
                //todo: reply found 
                // could be ignore.
            }
            else if (content.EventType == EventType.Error)
            {
                throw new Exception($"{content.ErrorEvent.ErrorCode} {name}");
            }
            else
            {
                _bufferEvents.Push(content);
            }
        }
    }
}