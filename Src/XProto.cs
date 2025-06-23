using System.Buffers;
using System.Net.Sockets;
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

namespace Xcsb;

[SkipLocalsInit]
internal class XProto : IXProto
{
    private readonly Socket _socket;
    private readonly HandshakeSuccessResponseBody _connectionResult;
    private bool _disposedValue;
    private int _globalId;

    public HandshakeSuccessResponseBody HandshakeSuccessResponseBody => _connectionResult;

    public XProto(Socket socket, HandshakeSuccessResponseBody connectionResult)
    {
        _socket = socket;
        _connectionResult = connectionResult;
        _globalId = 0;
    }

    AllocColorReply IXProto.AllocColor(uint colorMap, ushort red, ushort green, ushort blue)
    {
        var request = new AllocColorType(colorMap, red, green, blue);
        _socket.Send(ref request);

        Span<byte> response = stackalloc byte[Marshal.SizeOf<AllocColorReply>()];
        _socket.ReceiveExact(response);
        return response.ToStruct<AllocColorReply>();
    }

    void IXProto.AllocColorCells()
    {
        throw new NotImplementedException();
    }

    void IXProto.AllocColorPlanes()
    {
        throw new NotImplementedException();
    }

    void IXProto.AllocNamedColor()
    {
        throw new NotImplementedException();
    }

    void IXProto.AllowEvents(EventsMode mode, uint time)
    {
        var request = new AllowEventsType(mode, time);
        _socket.Send(ref request);
    }

    void IXProto.Bell(sbyte percent)
    {
        if (percent is not <= 100 or not >= (-100))
            throw new ArgumentOutOfRangeException(nameof(percent), "value must be between -100 to 100");

        var request = new BellType(percent);
        _socket.Send(ref request);
    }

    void IXProto.ChangeActivePointerGrab(uint cursor, uint time, ushort mask)
    {
        var request = new ChangeActivePointerGrabType(cursor, time, mask);
        _socket.Send(ref request);
    }

    void IXProto.ChangeGC(uint gc, GCMask mask, params uint[] args)
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

    void IXProto.ChangeHosts()
    {
        throw new NotImplementedException();
    }

    void IXProto.ChangeKeyboardControl(KeyboardControlMask mask, params uint[] args)
    {
        var requiredBuffer = 8 + args.Length * 4;
        if (requiredBuffer < GlobalSetting.StackAllocThreshold)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
            scratchBuffer[0] = (byte)Opcode.ChangeKeyboardControl;
            scratchBuffer[1] = 0;
            MemoryMarshal.Write(scratchBuffer[2..4], (ushort)(requiredBuffer / 4));
            MemoryMarshal.Write(scratchBuffer[4..8], mask);
            args.AsSpan().CopyTo(MemoryMarshal.Cast<byte, uint>(scratchBuffer[8..requiredBuffer]));
            _socket.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            scratchBuffer[0] = (byte)Opcode.ChangeKeyboardControl;
            scratchBuffer[1] = 0;
            MemoryMarshal.Write(scratchBuffer[2..4], (ushort)(requiredBuffer / 4));
            MemoryMarshal.Write(scratchBuffer[4..8], mask);
            args.AsSpan().CopyTo(MemoryMarshal.Cast<byte, uint>(scratchBuffer[8..requiredBuffer]));
            _socket.SendExact(scratchBuffer[..requiredBuffer]);
        }
    }

    void IXProto.ChangeKeyboardMapping()
    {
        throw new NotImplementedException();
    }

    void IXProto.ChangePointerControl(Acceleration? acceleration, ushort? threshold)
    {
        var request = new ChangePointerControlType(
            acceleration?.Numerator ?? 0,
            acceleration?.Denominator ?? 0,
            threshold ?? 0,
            (byte)(acceleration is null ? 0 : 1),
            (byte)(threshold.HasValue ? 1 : 0));
        _socket.Send(ref request);
    }

    void IXProto.ChangeProperty<T>(PropertyMode mode, uint window, uint property, uint type, params T[] args)
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

    void IXProto.ChangeSaveSet(ChangeSaveSetMode changeSaveSetMode, uint window)
    {
        var request = new ChangeSaveSetType(changeSaveSetMode, window);
        _socket.Send(ref request);
    }

    void IXProto.ChangeWindowAttributes(uint window, ValueMask mask, params uint[] args)
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

    void IXProto.CirculateWindow(Direction direction, uint window)
    {
        var request = new CirculateWindowType(direction, window);
        _socket.Send(ref request);
    }

    void IXProto.ClearArea(bool exposures, uint window, short x, short y, ushort width, ushort height)
    {
        var request = new ClearAreaType(exposures, window, x, y, width, height);
        _socket.Send(ref request);
    }

    void IXProto.CloseFont(uint fontId)
    {
        var request = new CloseFontType(fontId);
        _socket.Send(ref request);
    }

    void IXProto.ConfigureWindow(uint window, ConfigureValueMask mask, params uint[] args)
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

    void IXProto.ConvertSelection(uint requestor, uint selection, uint target, uint property, uint timestamp)
    {
        var request = new ConvertSelectionType(requestor, selection, target, property, timestamp);
        _socket.Send(ref request);
    }

    void IXProto.CopyArea(
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

    void IXProto.CopyColormapAndFree(uint colormapId, uint srcColormapId)
    {
        var request = new CopyColormapAndFreeType(colormapId, srcColormapId);
        _socket.Send(ref request);
    }

    void IXProto.CopyGC(uint srcGc, uint dstGc, GCMask mask)
    {
        var request = new CopyGCType(srcGc, dstGc, mask);
        _socket.Send(ref request);
    }

    void IXProto.CopyPlane(uint srcDrawable, uint destDrawable, uint gc, ushort srcX, ushort srcY, ushort destX, ushort destY, ushort width, ushort height, uint bitPlane)
    {
        var request = new CopyPlaneType(srcDrawable, destDrawable, gc, srcX, srcY, destX, destY, width, height, bitPlane);
        _socket.Send(ref request);
    }

    void IXProto.CreateColormap(ColormapAlloc alloc, uint colormapId, uint window, uint visual)
    {
        var request = new CreateColormapType(alloc, colormapId, window, visual);
        _socket.Send(ref request);
    }

    void IXProto.CreateCursor(uint cursorId, uint source, uint mask, ushort foreRed, ushort foreGreen, ushort foreBlue, ushort backRed, ushort backGreen, ushort backBlue, ushort x, ushort y)
    {
        var request = new CreateCursorType(cursorId, source, mask, foreRed, foreGreen, foreBlue, backRed, backGreen, backBlue, x, y);
        _socket.Send(ref request);
    }

    void IXProto.CreateGC(uint gc, uint drawable, GCMask mask, params uint[] args)
    {
        var request = new CreateGCType(gc, drawable, mask, args.Length);
        var requiredBuffer = 16 + (args.Length * 4);
        if (requiredBuffer < GlobalSetting.StackAllocThreshold)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
            MemoryMarshal.Write(scratchBuffer[0..16], request);
            MemoryMarshal.Cast<uint, byte>(args).CopyTo(scratchBuffer[16..(16 + (args.Length * 4))]);
            scratchBuffer[(16 + (args.Length * 4))..requiredBuffer].Clear();
            _socket.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            MemoryMarshal.Write(scratchBuffer[0..16], request);
            MemoryMarshal.Cast<uint, byte>(args).CopyTo(scratchBuffer[16..(16 + (args.Length * 4))]);
            scratchBuffer[(16 + (args.Length * 4))..requiredBuffer].Clear();
            _socket.SendExact(scratchBuffer[..requiredBuffer]);
        }
    }

    void IXProto.CreateGlyphCursor(uint cursorId,
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

    void IXProto.CreatePixmap(byte depth, uint pixmapId, uint drawable, ushort width, ushort height)
    {
        var request = new CreatePixmapType(depth, pixmapId, drawable, width, height);
        _socket.Send(ref request);
    }

    void IXProto.CreateWindow(byte depth, uint window,
        uint parent,
        short x,
        short y,
        ushort width,
        ushort height,
        ushort borderWidth,
        ClassType classType,
        uint rootVisualId,
        ValueMask mask,
        params uint[] args
    )
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
    }

    void IXProto.DeleteProperty(uint window, uint atom)
    {
        var request = new DeletePropertyType(window, atom);
        _socket.Send(ref request);
    }

    void IXProto.DestroySubwindows(uint window)
    {

        var request = new DestroySubWindowsType(window);
        _socket.Send(ref request);
    }

    void IXProto.DestroyWindow(uint window)
    {
        var request = new DestroyWindowType(window);
        _socket.Send(ref request);
    }

    void IXProto.FillPoly(uint drawable, uint gc, PolyShape shape, CoordinateMode coordinate, Point[] points)
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

    void IXProto.ForceScreenSaver(ForceScreenSaverMode mode)
    {
        var request = new ForceScreenSaverType(mode);
        _socket.Send(ref request);
    }

    void IXProto.FreeColormap(uint colormapId)
    {
        var request = new FreeColormapType(colormapId);
        _socket.Send(ref request);
    }

    void IXProto.FreeColors(uint colormapId, uint planeMask, params uint[] pixels)
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

    void IXProto.FreeCursor(uint cursorId)
    {
        var request = new FreeCursorType(cursorId);
        _socket.Send(ref request);
    }

    void IXProto.FreeGC(uint gc)
    {
        var request = new FreeGCType(gc);
        _socket.Send(ref request);
    }

    void IXProto.FreePixmap(uint pixmapId)
    {
        var request = new FreePixmapType(pixmapId);
        _socket.Send(ref request);
    }

    void IXProto.GetAtomName()
    {
        throw new NotImplementedException();
    }

    InternAtomReply IXProto.InternAtom(bool onlyIfExist, string atomName)
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

    void IXProto.GetFontPath()
    {
        throw new NotImplementedException();
    }

    void IXProto.GetGeometry()
    {
        throw new NotImplementedException();
    }

    void IXProto.GetImage()
    {
        throw new NotImplementedException();
    }

    void IXProto.GetInputFocus()
    {
        throw new NotImplementedException();
    }

    void IXProto.GetKeyboardControl()
    {
        throw new NotImplementedException();
    }

    void IXProto.GetKeyboardMapping()
    {
        throw new NotImplementedException();
    }

    void IXProto.GetModifierMapping()
    {
        throw new NotImplementedException();
    }

    void IXProto.GetMotionEvents()
    {
        throw new NotImplementedException();
    }

    void IXProto.GetPointerControl()
    {
        throw new NotImplementedException();
    }

    void IXProto.GetPointerMapping()
    {
        throw new NotImplementedException();
    }

    GetPropertyReply IXProto.GetProperty(bool delete, uint window, uint property, uint type, uint offset, uint length)
    {
        var request = new GetPropertyType(delete, window, property, type, offset, length);
        _socket.Send(ref request);
        return new GetPropertyReply(_socket);
    }

    void IXProto.GetScreenSaver()
    {
        throw new NotImplementedException();
    }

    void IXProto.GetSelectionOwner()
    {
        throw new NotImplementedException();
    }

    void IXProto.GetWindowAttributes()
    {
        throw new NotImplementedException();
    }

    void IXProto.GrabButton()
    {
        throw new NotImplementedException();
    }

    void IXProto.GrabKey()
    {
        throw new NotImplementedException();
    }

    void IXProto.GrabKeyboard()
    {
        throw new NotImplementedException();
    }

    GrabPointerReply IXProto.GrabPointer(bool ownerEvents, uint grabWindow, ushort mask, GrabMode pointerMode, GrabMode keyboardMode, uint confineTo, uint cursor, uint timeStamp)
    {
        var request = new GrabPointerType(ownerEvents, grabWindow, mask, pointerMode, keyboardMode, confineTo, cursor, timeStamp);
        _socket.Send(ref request);

        Span<byte> response = stackalloc byte[Marshal.SizeOf<GrabPointerReply>()];
        _socket.ReceiveExact(response);
        return response.ToStruct<GrabPointerReply>();
    }

    void IXProto.GrabServer()
    {
        var request = new GrabServerType();
        _socket.Send(ref request);
    }

    void IXProto.ImageText16(uint drawable, uint gc, short x, short y, ReadOnlySpan<char> text)
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

    void IXProto.ImageText8(uint drawable, uint gc, short x, short y, ReadOnlySpan<byte> text)
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

    void IXProto.InstallColormap(uint colormapId)
    {
        var request = new InstallColormapType(colormapId);
        _socket.Send(ref request);
    }

    void IXProto.KillClient(uint resource)
    {
        var request = new KillClientType(resource);
        _socket.Send(ref request);
    }

    void IXProto.ListExtensions()
    {
        throw new NotImplementedException();
    }

    void IXProto.ListFonts()
    {
        throw new NotImplementedException();
    }

    void IXProto.ListFontsWithInfo()
    {
        throw new NotImplementedException();
    }

    void IXProto.ListHosts()
    {
        throw new NotImplementedException();
    }

    void IXProto.ListInstalledColormaps()
    {
        throw new NotImplementedException();
    }

    void IXProto.ListProperties()
    {
        throw new NotImplementedException();
    }

    void IXProto.LookupColor()
    {
        throw new NotImplementedException();
    }

    void IXProto.MapSubwindows(uint window)
    {
        var request = new MapSubWindowsType(window);
        _socket.Send(ref request);
    }

    void IXProto.MapWindow(uint window)
    {
        var request = new MapWindowType(window);
        _socket.Send(ref request);
    }

    void IXProto.NoOperation(params uint[] args)
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

    void IXProto.OpenFont(string fontName, uint fontId)
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

    void IXProto.PolyArc(uint drawable, uint gc, Arc[] arcs)
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

    void IXProto.PolyFillArc(uint drawable, uint gc, Arc[] arcs)
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

    void IXProto.PolyFillRectangle(uint drawable, uint gc, Rectangle[] rectangles)
    {
        var request = new PolyFillRectangleType(drawable, gc, rectangles.Length);
        var requiredBuffer = Marshal.SizeOf<PolyFillRectangleType>() + (rectangles.Length * 8);

        if (requiredBuffer < GlobalSetting.StackAllocThreshold)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
            MemoryMarshal.Write(scratchBuffer[0..12], request);
            MemoryMarshal.Cast<Rectangle, byte>(rectangles)
                .CopyTo(scratchBuffer[12..(12 + rectangles.Length * 8)]);
            scratchBuffer[(12 + rectangles.Length * 8)..requiredBuffer].Clear();
            _socket.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            MemoryMarshal.Write(scratchBuffer[0..12], request);
            MemoryMarshal.Cast<Rectangle, byte>(rectangles)
                .CopyTo(scratchBuffer[12..(12 + rectangles.Length * 8)]);
            scratchBuffer[(12 + rectangles.Length * 8)..requiredBuffer].Clear();
            _socket.SendExact(scratchBuffer[..requiredBuffer]);
        }
    }

    void IXProto.PolyLine(CoordinateMode coordinate, uint drawable, uint gc, Point[] points)
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

    void IXProto.PolyPoint(CoordinateMode coordinate, uint drawable, uint gc, Point[] points)
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

    void IXProto.PolyRectangle(uint drawable, uint gc, Rectangle[] rectangles)
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

    void IXProto.PolySegment(uint drawable, uint gc, Segment[] segments)
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

    void IXProto.PolyText16()
    {
        throw new NotImplementedException();
    }

    void IXProto.PolyText8()
    {
        throw new NotImplementedException();
    }

    void IXProto.PutImage(ImageFormat format,
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
        // todo: optamice
        var request = new PutImageType(
            format,
            drawable,
            gc, width, height, x, y,
            leftPad, depth,
            data.Length);
        _socket.Send(ref request);
        var scratchBufferSize = data.Length.AddPadding();
        if (scratchBufferSize < GlobalSetting.StackAllocThreshold)
        {
            Span<byte> scratchBuffer = stackalloc byte[scratchBufferSize];
            data.CopyTo(scratchBuffer[..data.Length]);
            scratchBuffer[data.Length..].Clear();
            _socket.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(scratchBufferSize);
            data.CopyTo(scratchBuffer[..data.Length]);
            scratchBuffer[data.Length..].Clear();
            _socket.SendExact(scratchBuffer[..scratchBufferSize]);
        }
    }

    void IXProto.QueryBestSize()
    {
        throw new NotImplementedException();
    }

    void IXProto.QueryColors()
    {
        throw new NotImplementedException();
    }

    void IXProto.QueryExtension()
    {
        throw new NotImplementedException();
    }

    void IXProto.QueryFont()
    {
        throw new NotImplementedException();
    }

    void IXProto.QueryKeymap()
    {
        throw new NotImplementedException();
    }

    QueryPointerReply IXProto.QueryPointer(uint window)
    {
        var request = new QueryPointerType(window);
        _socket.Send(ref request);

        Span<byte> response = stackalloc byte[Marshal.SizeOf<QueryPointerReply>()];
        _socket.ReceiveExact(response);
        return response.ToStruct<QueryPointerReply>();
    }

    void IXProto.QueryTextExtents()
    {
        throw new NotImplementedException();
    }

    void IXProto.QueryTree()
    {
        throw new NotImplementedException();
    }

    void IXProto.RecolorCursor(uint cursorId, ushort foreRed, ushort foreGreen, ushort foreBlue, ushort backRed, ushort backGreen, ushort backBlue)
    {
        var request = new RecolorCursorType(cursorId, foreRed, foreGreen, foreBlue, backRed, backGreen, backBlue);
        _socket.Send(ref request);
    }

    void IXProto.ReparentWindow(uint window, uint parent, short x, short y)
    {
        var request = new ReparentWindowType(window, parent, x, y);
        _socket.Send(ref request);
    }

    void IXProto.RotateProperties(uint window, ushort delta, params uint[] properties)
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

    void IXProto.SendEvent(bool propagate, uint destination, uint eventMask, XEvent evnt)
    {
        var request = new SendEventType(propagate, destination, eventMask, evnt);
        _socket.Send(ref request);
    }

    void IXProto.SetAccessControl(AccessControlMode mode)
    {
        var request = new SetAccessControlType(mode);
        _socket.Send(ref request);
    }

    void IXProto.SetClipRectangles(ClipOrdering ordering, uint gc, ushort clipX, ushort clipY, Rectangle[] rectangles)
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

    void IXProto.SetCloseDownMode(CloseDownMode mode)
    {
        var request = new SetCloseDownModeType(mode);
        _socket.Send(ref request);
    }

    void IXProto.SetDashes(uint gc, ushort dashOffset, byte[] dashes)
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

    void IXProto.SetFontPath()
    {
        throw new NotImplementedException();
    }

    void IXProto.SetInputFocus(InputFocusMode mode, uint focus, uint time)
    {
        var request = new SetInputFocusType(mode, focus, time);
        _socket.Send(ref request);
    }

    void IXProto.SetModifierMapping()
    {
        throw new NotImplementedException();
    }

    void IXProto.SetPointerMapping()
    {
        throw new NotImplementedException();
    }

    void IXProto.SetScreenSaver(short timeout, short interval, TriState preferBlanking, TriState allowExposures)
    {
        var request = new SetScreenSaverType(timeout, interval, preferBlanking, allowExposures);
        _socket.Send(ref request);
    }

    void IXProto.SetSelectionOwner(uint owner, uint atom, uint timestamp)
    {
        var request = new SetSelectionOwnerType(owner, atom, timestamp);
        _socket.Send(ref request);
    }

    void IXProto.StoreColors(uint colormapId, params ColorItem[] item)
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

    void IXProto.StoreNamedColor(ColorFlag mode, uint colormapId, uint pixels, ReadOnlySpan<byte> name)
    {
        var requiredBuffer = 16 + name.Length.AddPadding();
        if (requiredBuffer < GlobalSetting.StackAllocThreshold)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
            scratchBuffer[0] = (byte)Opcode.StoreNamedColor;
            scratchBuffer[1] = (byte)mode;
            MemoryMarshal.Write(scratchBuffer[2..4], (ushort)(requiredBuffer / 4));
            MemoryMarshal.Write(scratchBuffer[4..8], colormapId);
            MemoryMarshal.Write(scratchBuffer[8..12], pixels);
            MemoryMarshal.Write(scratchBuffer[12..14], (ushort)name.Length);
            MemoryMarshal.Write(scratchBuffer[14..16], 0);
            name.CopyTo(scratchBuffer[16..(name.Length + 16)]);
            scratchBuffer[^name.Length.Padding()..].Clear();
            _socket.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            scratchBuffer[0] = (byte)Opcode.StoreNamedColor;
            scratchBuffer[1] = (byte)mode;
            MemoryMarshal.Write(scratchBuffer[2..4], (ushort)(requiredBuffer / 4));
            MemoryMarshal.Write(scratchBuffer[4..8], colormapId);
            MemoryMarshal.Write(scratchBuffer[8..12], pixels);
            MemoryMarshal.Write(scratchBuffer[12..14], (ushort)name.Length);
            MemoryMarshal.Write(scratchBuffer[14..16], 0);
            name.CopyTo(scratchBuffer[16..(name.Length + 16)]);
            scratchBuffer[^name.Length.Padding()..].Clear();
            _socket.SendExact(scratchBuffer[..requiredBuffer]);
        }
    }

    void IXProto.TranslateCoordinates()
    {
        throw new NotImplementedException();
    }

    void IXProto.UngrabButton()
    {
        throw new NotImplementedException();
    }

    void IXProto.UngrabKey()
    {
        throw new NotImplementedException();
    }

    void IXProto.UngrabKeyboard()
    {
        throw new NotImplementedException();
    }

    void IXProto.UngrabPointer(uint time)
    {
        var request = new UngrabPointerType(time);
        _socket.Send(ref request);
    }

    void IXProto.UngrabServer()
    {
        var request = new UnGrabServerType();
        _socket.Send(ref request);
    }

    void IXProto.UninstallColormap(uint colormapId)
    {
        var request = new UninstallColormapType(colormapId);
        _socket.Send(ref request);
    }

    void IXProto.UnmapSubwindows(uint window)
    {
        var request = new UnMapSubwindowsType(window);
        _socket.Send(ref request);
    }

    void IXProto.UnmapWindow(uint window)
    {
        var request = new UnmapWindowType(window);
        _socket.Send(ref request);
    }

    void IXProto.WarpPointer(uint srcWindow, uint destWindow, short srcX, short srcY, ushort srcWidth, ushort srcHeight, short destX, short destY)
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

    void IDisposable.Dispose()
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
}