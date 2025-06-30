using System;
using System.Buffers;
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

    public HandshakeSuccessResponseBody HandshakeSuccessResponseBody => _connectionResult;

    public IXBufferProto BufferCLient => new XBufferProto(_socket);

    public XProto(Socket socket, HandshakeSuccessResponseBody connectionResult)
    {
        _socket = socket;
        _connectionResult = connectionResult;
        _globalId = 0;
    }

    public AllocColorReply AllocColor(
        uint colorMap,
        ushort red,
        ushort green,
        ushort blue)
    {
        var request = new AllocColorType(colorMap, red, green, blue);
        _socket.Send(ref request);

        Span<byte> response = stackalloc byte[Marshal.SizeOf<AllocColorReply>()];
        _socket.ReceiveExact(response);
        return response.ToStruct<AllocColorReply>();
    }

    public void AllocColorCells()
    {
        throw new NotImplementedException();
    }

    public void AllocColorPlanes()
    {
        throw new NotImplementedException();
    }

    public void AllocNamedColor()
    {
        throw new NotImplementedException();
    }

    public void AllowEvents(
        EventsMode mode,
        uint time)
    {
        var request = new AllowEventsType(mode, time);
        _socket.Send(ref request);
    }

    public void Bell(sbyte percent)
    {
        if (percent is not <= 100 or not >= (-100))
            throw new ArgumentOutOfRangeException(nameof(percent), "value must be between -100 to 100");

        var request = new BellType(percent);
        _socket.Send(ref request);
    }

    public void ChangeActivePointerGrab(
        uint cursor,
        uint time,
        ushort mask)
    {
        var request = new ChangeActivePointerGrabType(cursor, time, mask);
        _socket.Send(ref request);
    }

    public void ChangeGC(
        uint gc,
        GCMask mask,
        params uint[] args)
    {
        var request = new ChangeGCType(gc, mask, args.Length);
        var requiredBuffer = 12 + args.Length * 4;

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
    }

    public void ChangeHosts(
        HostMode mode,
        Family family,
        byte[] address)
    {
        var request = new ChangeHostsType(mode, family, address.Length);
        var requiredBuffer = 8 + address.Length.AddPadding();
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
    }

    public void ChangeKeyboardControl(
        KeyboardControlMask mask,
        params uint[] args)
    {
        var requiredBuffer = 8 + args.Length * 4;
        var request = new ChangeKeyboardControlType(mask, args.Length);
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
    }

    public void ChangeKeyboardMapping(
        byte keycodeCount,
        byte firstKeycode,
        byte keysymsPerKeycode,
        uint[] Keysym)
    {
        var requiredBuffer = 8 + (keycodeCount * keysymsPerKeycode) * 4;
        var request = new ChangeKeyboardMappingType(keycodeCount, firstKeycode, keysymsPerKeycode);
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
    }

    public void ChangePointerControl(
        Acceleration? acceleration,
        ushort? threshold)
    {
        var request = new ChangePointerControlType(
            acceleration?.Numerator ?? 0,
            acceleration?.Denominator ?? 0,
            threshold ?? 0,
            (byte)(acceleration is null ? 0 : 1),
            (byte)(threshold.HasValue ? 1 : 0));
        _socket.Send(ref request);
    }

    void IVoidProto.ChangeProperty<T>(
        PropertyMode mode,
        uint window,
        uint property,
        uint type,
        params T[] args)
    {
        var size = Marshal.SizeOf<T>();
        if (size is not 1 or 2 or 4)
            throw new ArgumentException("type must be byte, sbyte, short, ushort, int, uint");
        var request = new ChangePropertyType(mode, window, property, type, args.Length, (byte)(size * 8));
        var requiredBuffer = 24 + (args.Length.AddPadding() * size);

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
    }

    public void ChangeSaveSet(
        ChangeSaveSetMode changeSaveSetMode,
        uint window)
    {
        var request = new ChangeSaveSetType(changeSaveSetMode, window);
        _socket.Send(ref request);
    }

    public void ChangeWindowAttributes(
        uint window,
        ValueMask mask,
        params uint[] args)
    {
        var request = new ChangeWindowAttributesType(window, mask, args.Length);
        var requiredBuffer = 12 + args.Length * 4;
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
    }

    public void CirculateWindow(
        Direction direction,
        uint window)
    {
        var request = new CirculateWindowType(direction, window);
        _socket.Send(ref request);
    }

    public void ClearArea(
        bool exposures,
        uint window,
        short x,
        short y,
        ushort width,
        ushort height)
    {
        var request = new ClearAreaType(exposures, window, x, y, width, height);
        _socket.Send(ref request);
    }

    public void CloseFont(uint fontId)
    {
        var request = new CloseFontType(fontId);
        _socket.Send(ref request);
    }

    public void ConfigureWindow(
        uint window,
        ConfigureValueMask mask,
        params uint[] args)
    {
        var requiredBuffer = 12 + args.Length * 4;
        var request = new ConfigureWindowType(window, mask, args.Length);
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
    }

    public void ConvertSelection(
        uint requestor,
        uint selection,
        uint target,
        uint property,
        uint timestamp)
    {
        var request = new ConvertSelectionType(requestor, selection, target, property, timestamp);
        _socket.Send(ref request);
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
        var request = new CopyAreaType(srcDrawable, destDrawable, gc, srcX, srcY, destX, destY, width, height);
        _socket.Send(ref request);
    }

    public void CopyColormapAndFree(
        uint colormapId,
        uint srcColormapId)
    {
        var request = new CopyColormapAndFreeType(colormapId, srcColormapId);
        _socket.Send(ref request);
    }

    public void CopyGC(
        uint srcGc,
        uint dstGc,
        GCMask mask)
    {
        var request = new CopyGCType(srcGc, dstGc, mask);
        _socket.Send(ref request);
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
        var request = new CopyPlaneType(srcDrawable, destDrawable, gc, srcX, srcY, destX, destY, width, height, bitPlane);
        _socket.Send(ref request);
    }

    public void CreateColormap(
        ColormapAlloc alloc,
        uint colormapId,
        uint window,
        uint visual)
    {
        var request = new CreateColormapType(alloc, colormapId, window, visual);
        _socket.Send(ref request);
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
        var request = new CreateCursorType(cursorId, source, mask, foreRed, foreGreen, foreBlue, backRed, backGreen, backBlue, x, y);
        _socket.Send(ref request);
    }

    public void CreateGC(
        uint gc,
        uint drawable,
        GCMask mask,
        params uint[] args)
    {
        var request = new CreateGCType(gc, drawable, mask, args.Length);
        var requiredBuffer = 16 + (args.Length * 4);
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
        var request = new CreateGlyphCursorType(cursorId, sourceFont, fontMask, sourceChar, charMask, foreRed, foreGreen, foreBlue,
            backRed, backGreen, backBlue);
        _socket.Send(ref request);
    }

    public void CreatePixmap(
        byte depth,
        uint pixmapId,
        uint drawable,
        ushort width,
        ushort height)
    {
        var request = new CreatePixmapType(depth, pixmapId, drawable, width, height);
        _socket.Send(ref request);
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
        var request = new DeletePropertyType(window, atom);
        _socket.Send(ref request);
    }

    public void DestroySubwindows(uint window)
    {
        var request = new DestroySubWindowsType(window);
        _socket.Send(ref request);
    }

    public void DestroyWindow(uint window)
    {
        var request = new DestroyWindowType(window);
        _socket.Send(ref request);
    }

    public void FillPoly(
        uint drawable,
        uint gc,
        PolyShape shape,
        CoordinateMode coordinate,
        Point[] points)
    {
        var request = new FillPolyType(drawable, gc, shape, coordinate, points.Length);
        var requiredBuffer = 16 + (points.Length * 4);

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
    }

    public void ForceScreenSaver(ForceScreenSaverMode mode)
    {
        var request = new ForceScreenSaverType(mode);
        _socket.Send(ref request);
    }

    public void FreeColormap(uint colormapId)
    {
        var request = new FreeColormapType(colormapId);
        _socket.Send(ref request);
    }

    public void FreeColors(
        uint colormapId,
        uint planeMask,
        params uint[] pixels)
    {
        var requiredBuffer = 12 + pixels.Length * 4;
        var request = new FreeColorsType(colormapId, planeMask, pixels.Length);
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
    }

    public void FreeCursor(uint cursorId)
    {
        var request = new FreeCursorType(cursorId);
        _socket.Send(ref request);
    }

    public void FreeGC(uint gc)
    {
        var request = new FreeGCType(gc);
        _socket.Send(ref request);
    }

    public void FreePixmap(uint pixmapId)
    {
        var request = new FreePixmapType(pixmapId);
        _socket.Send(ref request);
    }

    public void GetAtomName()
    {
        throw new NotImplementedException();
    }

    public InternAtomReply InternAtom(
        bool onlyIfExist,
        string atomName)
    {
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
        return responce.ToStruct<InternAtomReply>();
    }

    public void GetFontPath()
    {
        throw new NotImplementedException();
    }

    public void GetGeometry()
    {
        throw new NotImplementedException();
    }

    public void GetImage()
    {
        throw new NotImplementedException();
    }

    public void GetInputFocus()
    {
        throw new NotImplementedException();
    }

    public void GetKeyboardControl()
    {
        throw new NotImplementedException();
    }

    public void GetKeyboardMapping()
    {
        throw new NotImplementedException();
    }

    public void GetModifierMapping()
    {
        throw new NotImplementedException();
    }

    public void GetMotionEvents()
    {
        throw new NotImplementedException();
    }

    public void GetPointerControl()
    {
        throw new NotImplementedException();
    }

    public void GetPointerMapping()
    {
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
        var request = new GetPropertyType(delete, window, property, type, offset, length);
        _socket.Send(ref request);
        return new GetPropertyReply(_socket);
    }

    public void GetScreenSaver()
    {
        throw new NotImplementedException();
    }

    public void GetSelectionOwner()
    {
        throw new NotImplementedException();
    }

    public void GetWindowAttributes()
    {
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
        var request = new GrabButtonType(ownerEvents, grabWindow, mask, pointerMode, keyboardMode, confineTo,
             cursor, button, modifiers);
        _socket.Send(ref request);
    }

    public void GrabKey(
        bool exposures,
        uint grabWindow,
        ModifierMask mask,
        byte keycode,
        GrabMode pointerMode,
        GrabMode keyboardMode)
    {
        var request = new GrabKeyType(exposures, grabWindow, mask, keycode, pointerMode, keyboardMode);
        _socket.Send(ref request);
    }

    public void GrabKeyboard()
    {
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
        var request = new GrabPointerType(ownerEvents, grabWindow, mask, pointerMode, keyboardMode, confineTo, cursor, timeStamp);
        _socket.Send(ref request);

        Span<byte> response = stackalloc byte[Marshal.SizeOf<GrabPointerReply>()];
        _socket.ReceiveExact(response);
        return response.ToStruct<GrabPointerReply>();
    }

    public void GrabServer()
    {
        var request = new GrabServerType();
        _socket.Send(ref request);
    }

    public void ImageText16(
        uint drawable,
        uint gc,
        short x,
        short y,
        ReadOnlySpan<char> text)
    {
        var request = new ImageText16Type(drawable, gc, x, y, text.Length);
        var requiredBuffer = 16 + (text.Length * 2).AddPadding();
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
    }

    public void ImageText8(
        uint drawable,
        uint gc,
        short x,
        short y,
        ReadOnlySpan<byte> text)
    {
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
        var request = new InstallColormapType(colormapId);
        _socket.Send(ref request);
    }

    public void KillClient(uint resource)
    {
        var request = new KillClientType(resource);
        _socket.Send(ref request);
    }

    public void ListExtensions()
    {
        throw new NotImplementedException();
    }

    public void ListFonts()
    {
        throw new NotImplementedException();
    }

    public void ListFontsWithInfo()
    {
        throw new NotImplementedException();
    }

    public void ListHosts()
    {
        throw new NotImplementedException();
    }

    public void ListInstalledColormaps()
    {
        throw new NotImplementedException();
    }

    public void ListProperties()
    {
        throw new NotImplementedException();
    }

    public void LookupColor()
    {
        throw new NotImplementedException();
    }

    public void MapSubwindows(uint window)
    {
        var request = new MapSubWindowsType(window);
        _socket.Send(ref request);
    }

    public void MapWindow(uint window)
    {
        var request = new MapWindowType(window);
        _socket.Send(ref request);

        CheckError();
    }

    public void NoOperation(params uint[] args)
    {
        var requiredBuffer = 4 + args.Length * 4;
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
    }

    public void OpenFont(
        string fontName,
        uint fontId)
    {
        var request = new OpenFontType(fontId, (ushort)fontName.Length);
        var requestSize = Marshal.SizeOf<OpenFontType>();
        var requiredBuffer = requestSize + fontName.Length.AddPadding();

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
    }

    public void PolyArc(
        uint drawable,
        uint gc,
        Arc[] arcs)
    {
        var request = new PolyArcType(drawable, gc, arcs.Length);
        var requiredBuffer = Marshal.SizeOf<PolyArcType>() + (arcs.Length * 12);

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
    }

    public void PolyFillArc(
        uint drawable,
        uint gc,
        Arc[] arcs)
    {
        var request = new PolyFillArcType(drawable, gc, arcs.Length);
        var requiredBuffer = Marshal.SizeOf<PolyFillArcType>() + (arcs.Length * 12);

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
    }

    public void PolyFillRectangle(
        uint drawable,
        uint gc,
        Rectangle[] rectangles)
    {
        var request = new PolyFillRectangleType(drawable, gc, rectangles.Length);
        var requiredBuffer = Marshal.SizeOf<PolyFillRectangleType>() + (rectangles.Length * 8);

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
    }

    public void PolyLine(
        CoordinateMode coordinate,
        uint drawable,
        uint gc,
        Point[] points)
    {
        var request = new PolyLineType(coordinate, drawable, gc, points.Length);
        var requiredBuffer = 12 + (points.Length * 4);

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
    }

    public void PolyPoint(
        CoordinateMode coordinate,
        uint drawable,
        uint gc,
        Point[] points)
    {
        var request = new PolyPointType(coordinate, drawable, gc, points.Length);
        var requiredBuffer = 12 + (points.Length * 4);

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
    }

    public void PolyRectangle(
        uint drawable,
        uint gc,
        Rectangle[] rectangles)
    {
        var request = new PolyRectangleType(drawable, gc, rectangles.Length);
        var requiredBuffer = 12 + (rectangles.Length * 8);

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
    }

    public void PolySegment(
        uint drawable,
        uint gc,
        Segment[] segments)
    {
        var request = new PolySegmentType(drawable, gc, segments.Length);
        var requiredBuffer = 12 + (segments.Length * 8);

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
    }

    public void PolyText16()
    {
        throw new NotImplementedException();
    }

    public void PolyText8()
    {
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
        var request = new PutImageType(
            format,
            drawable,
            gc, width, height, x, y,
            leftPad, depth,
            data.Length);
        var scratchBufferSize = data.Length.AddPadding() + 24;
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
    }

    public void QueryBestSize()
    {
        throw new NotImplementedException();
    }

    public void QueryColors()
    {
        throw new NotImplementedException();
    }

    public void QueryExtension()
    {
        throw new NotImplementedException();
    }

    public void QueryFont()
    {
        throw new NotImplementedException();
    }

    public void QueryKeymap()
    {
        throw new NotImplementedException();
    }

    public QueryPointerReply QueryPointer(uint window)
    {
        var request = new QueryPointerType(window);
        _socket.Send(ref request);

        Span<byte> response = stackalloc byte[Marshal.SizeOf<QueryPointerReply>()];
        _socket.ReceiveExact(response);
        return response.ToStruct<QueryPointerReply>();
    }

    public void QueryTextExtents()
    {
        throw new NotImplementedException();
    }

    public void QueryTree()
    {
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
        var request = new RecolorCursorType(cursorId, foreRed, foreGreen, foreBlue, backRed, backGreen, backBlue);
        _socket.Send(ref request);
    }

    public void ReparentWindow(
        uint window,
        uint parent,
        short x,
        short y)
    {
        var request = new ReparentWindowType(window, parent, x, y);
        _socket.Send(ref request);
    }

    public void RotateProperties(
        uint window,
        ushort delta,
        params uint[] properties)
    {
        var request = new RotatePropertiesType(window, properties.Length, delta);
        var requiredBuffer = 12 + properties.Length * 4;

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
    }

    public void SendEvent(
        bool propagate,
        uint destination,
        uint eventMask,
        XEvent evnt)
    {
        var request = new SendEventType(propagate, destination, eventMask, evnt);
        _socket.Send(ref request);
    }

    public void SetAccessControl(AccessControlMode mode)
    {
        var request = new SetAccessControlType(mode);
        _socket.Send(ref request);
    }

    public void SetClipRectangles(
        ClipOrdering ordering,
        uint gc,
        ushort clipX,
        ushort clipY,
        Rectangle[] rectangles)
    {
        var request = new SetClipRectanglesType(ordering, gc, clipX, clipY, rectangles.Length);
        var requiredBuffer = 12 + rectangles.Length * Marshal.SizeOf<Rectangle>();

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
    }

    public void SetCloseDownMode(CloseDownMode mode)
    {
        var request = new SetCloseDownModeType(mode);
        _socket.Send(ref request);
    }

    public void SetDashes(
        uint gc,
        ushort dashOffset,
        byte[] dashes)
    {
        var request = new SetDashesType(gc, dashOffset, dashes.Length);
        var requiredBuffer = 12 + dashes.Length.AddPadding();
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
    }

    public void SetFontPath(string[] strPaths)
    {
        var request = new SetFontPathType((ushort)strPaths.Length, strPaths.Sum(a => a.Length).AddPadding());
        var requiredBuffer = 8 + strPaths.Sum(a => a.Length).AddPadding();
        var writIndex = 8;
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
    }

    public void SetInputFocus(
        InputFocusMode mode,
        uint focus,
        uint time)
    {
        var request = new SetInputFocusType(mode, focus, time);
        _socket.Send(ref request);
    }

    public void SetModifierMapping()
    {
        throw new NotImplementedException();
    }

    public void SetPointerMapping()
    {
        throw new NotImplementedException();
    }

    public void SetScreenSaver(
        short timeout,
        short interval,
        TriState preferBlanking,
        TriState allowExposures)
    {
        var request = new SetScreenSaverType(timeout, interval, preferBlanking, allowExposures);
        _socket.Send(ref request);
    }

    public void SetSelectionOwner(
        uint owner,
        uint atom,
        uint timestamp)
    {
        var request = new SetSelectionOwnerType(owner, atom, timestamp);
        _socket.Send(ref request);
    }

    public void StoreColors(
        uint colormapId,
        params ColorItem[] item)
    {
        var request = new StoreColorsType(colormapId, item.Length);
        var requiredBuffer = 8 + 12 * item.Length;
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
    }

    public void StoreNamedColor(
        ColorFlag mode,
        uint colormapId,
        uint pixels,
        ReadOnlySpan<byte> name)
    {
        var request = new StoreNamedColorType(mode, colormapId, pixels, name.Length);
        var requiredBuffer = 16 + name.Length.AddPadding();
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
    }

    public void TranslateCoordinates()
    {
        throw new NotImplementedException();
    }

    public void UngrabButton(
        Button button,
        uint grabWindow,
        ModifierMask mask)
    {
        var request = new UngrabButtonType(button, grabWindow, mask);
        _socket.Send(ref request);
    }

    public void UngrabKey(
        byte key,
        uint grabWindow,
        ModifierMask modifier)
    {
        var request = new UngrabKeyType(key, grabWindow, modifier);
        _socket.Send(ref request);
    }

    public void UngrabKeyboard(uint time)
    {
        var request = new UngrabKeyboardType(time);
        _socket.Send(ref request);
    }

    public void UngrabPointer(uint time)
    {
        var request = new UngrabPointerType(time);
        _socket.Send(ref request);
    }

    public void UngrabServer()
    {
        var request = new UnGrabServerType();
        _socket.Send(ref request);
    }

    public void UninstallColormap(uint colormapId)
    {
        var request = new UninstallColormapType(colormapId);
        _socket.Send(ref request);
    }

    public void UnmapSubwindows(uint window)
    {
        var request = new UnMapSubwindowsType(window);
        _socket.Send(ref request);
    }

    public void UnmapWindow(uint window)
    {
        var request = new UnmapWindowType(window);
        _socket.Send(ref request);
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
        var request = new WarpPointerType(srcWindow, destWindow, srcX, srcY, srcWidth, srcHeight, destX, destY);
        _socket.Send(ref request);
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

    public ref XEvent GetEvent(Span<byte> scratchBuffer)
    {
        var requiredLength = Marshal.SizeOf<XEvent>();
        if (scratchBuffer.Length != requiredLength)
            throw new ArgumentException($"scratchBuffer at least {requiredLength} bytes");
        if (_socket.Poll(-1, SelectMode.SelectRead))
        {
            var totalRead = _socket.Receive(scratchBuffer);
            if (totalRead != 0)
                return ref scratchBuffer.AsStruct<XEvent>();
        }

        scratchBuffer[0] = 0;
        return ref scratchBuffer.AsStruct<XEvent>();
    }

    private void CheckError()
    {
        if (!_socket.Poll(0, SelectMode.SelectRead))
            return;

        Span<byte> buffer = stackalloc byte[Marshal.SizeOf<XEvent>()];
        var received = _socket.Receive(buffer);
        var evnt = buffer.AsStruct<XEvent>();
        if (evnt.EventType == EventType.Error)
            throw new Exception();
    }
}