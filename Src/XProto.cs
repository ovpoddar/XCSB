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
using Xcsb.Models.Infrastructure;
using Xcsb.Models.Requests;
using Xcsb.Models.Response;
using static System.Net.Mime.MediaTypeNames;

namespace Xcsb;

[SkipLocalsInit]
internal class XProto : BaseProtoClient, IXProto
{
    private readonly HandshakeSuccessResponseBody _connectionResult;
    private bool _disposedValue;
    private int _globalId;

    public HandshakeSuccessResponseBody HandshakeSuccessResponseBody => _connectionResult;

    public IXBufferProto BufferCLient => new XBufferProto(this);

    public XProto(Socket socket, HandshakeSuccessResponseBody connectionResult) : base(socket)
    {
        _connectionResult = connectionResult;
        _globalId = 0;
        sequenceNumber = 0;
    }

    public AllocColorReply AllocColor(uint colorMap, ushort red, ushort green, ushort blue)
    {
        var request = new AllocColorType(colorMap, red, green, blue);
        socket.Send(ref request);

        var (result, error) = ReceivedResponse<AllocColorReply>();
        if (!error.HasValue && result.HasValue)
        {
            sequenceNumber++;
            Debug.Assert(sequenceNumber == result.Value.Sequence);
            return result.Value;
        }
        throw new XEventException(error!.Value);
    }

    public void AllocColorCells()
    {
        throw new NotImplementedException();
        sequenceNumber++;
    }

    public void AllocColorPlanes()
    {
        throw new NotImplementedException();
        sequenceNumber++;
    }

    public void AllocNamedColor()
    {
        throw new NotImplementedException();
        sequenceNumber++;
    }


    public void AllowEvents(EventsMode mode, uint time)
    {
        var request = new AllowEventsType(mode, time);
        socket.Send(ref request);
        sequenceNumber++;
    }

    public void Bell(sbyte percent)
    {
        if (percent is not <= 100 or not >= (-100))
            throw new ArgumentOutOfRangeException(nameof(percent), "value must be between -100 to 100");

        var request = new BellType(percent);
        socket.Send(ref request);
        sequenceNumber++;
    }

    public void ChangeActivePointerGrab(uint cursor, uint time, ushort mask)
    {
        var request = new ChangeActivePointerGrabType(cursor, time, mask);
        socket.Send(ref request);
        sequenceNumber++;
    }

    public void ChangeGC(uint gc, GCMask mask, params uint[] args)
    {
        var request = new ChangeGCType(gc, mask, args.Length);
        var requiredBuffer = 12 + args.Length * 4;
        if (requiredBuffer < GlobalSetting.StackAllocThreshold)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
            MemoryMarshal.Write(scratchBuffer[0..12], request);
            MemoryMarshal.Cast<uint, byte>(args)
                .CopyTo(scratchBuffer[12..requiredBuffer]);
            socket.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            MemoryMarshal.Write(scratchBuffer[0..12], request);
            MemoryMarshal.Cast<uint, byte>(args)
                .CopyTo(scratchBuffer[12..requiredBuffer]);
            socket.SendExact(scratchBuffer[..requiredBuffer]);
        }

        sequenceNumber++;
    }

    public void ChangeHosts(HostMode mode, Family family, byte[] address)
    {
        var request = new ChangeHostsType(mode, family, address.Length);
        var requiredBuffer = 8 + address.Length.AddPadding();
        if (requiredBuffer < GlobalSetting.StackAllocThreshold)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
            MemoryMarshal.Write(scratchBuffer[0..8], request);
            address.CopyTo(scratchBuffer[8..requiredBuffer]);
            scratchBuffer[^address.Length.Padding()..].Clear();
            socket.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            MemoryMarshal.Write(scratchBuffer[0..8], request);
            address.CopyTo(scratchBuffer[8..requiredBuffer]);
            scratchBuffer[^address.Length.Padding()..].Clear();
            socket.SendExact(scratchBuffer[..requiredBuffer]);
        }

        sequenceNumber++;
    }

    public void ChangeKeyboardControl(KeyboardControlMask mask, params uint[] args)
    {
        var requiredBuffer = 8 + args.Length * 4;
        var request = new ChangeKeyboardControlType(mask, args.Length);
        if (requiredBuffer < GlobalSetting.StackAllocThreshold)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
            MemoryMarshal.Write(scratchBuffer[0..8], request);
            MemoryMarshal.Cast<uint, byte>(args).CopyTo(scratchBuffer[8..requiredBuffer]);
            socket.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            MemoryMarshal.Write(scratchBuffer[0..8], request);
            MemoryMarshal.Cast<uint, byte>(args).CopyTo(scratchBuffer[8..requiredBuffer]);
            socket.SendExact(scratchBuffer[..requiredBuffer]);
        }

        sequenceNumber++;
    }

    public void ChangeKeyboardMapping(byte keycodeCount, byte firstKeycode, byte keysymsPerKeycode, uint[] Keysym)
    {
        var requiredBuffer = 8 + (keycodeCount * keysymsPerKeycode) * 4;
        var request = new ChangeKeyboardMappingType(keycodeCount, firstKeycode, keysymsPerKeycode);
        if (requiredBuffer < GlobalSetting.StackAllocThreshold)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
            MemoryMarshal.Write(scratchBuffer[0..8], request);
            MemoryMarshal.Cast<uint, byte>(Keysym).CopyTo(scratchBuffer[8..requiredBuffer]);
            socket.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            MemoryMarshal.Write(scratchBuffer[0..8], request);
            MemoryMarshal.Cast<uint, byte>(Keysym).CopyTo(scratchBuffer[8..requiredBuffer]);
            socket.SendExact(scratchBuffer[..requiredBuffer]);
        }

        sequenceNumber++;
    }

    public void ChangePointerControl(Acceleration? acceleration, ushort? threshold)
    {
        var request = new ChangePointerControlType(acceleration?.Numerator ?? 0, acceleration?.Denominator ?? 0,
            threshold ?? 0, (byte)(acceleration is null ? 0 : 1), (byte)(threshold.HasValue ? 1 : 0));
        socket.Send(ref request);
        sequenceNumber++;
    }


    public void ChangeProperty<T>(PropertyMode mode, uint window, uint property, uint type, params T[] args) where T : struct, INumber<T>
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
            socket.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            MemoryMarshal.Write(scratchBuffer[0..24], request);
            MemoryMarshal.Cast<T, byte>(args).CopyTo(scratchBuffer[24..(24 + args.Length * size)]);
            scratchBuffer[(24 + args.Length * size)..requiredBuffer].Clear();
            socket.SendExact(scratchBuffer[..requiredBuffer]);
        }

        sequenceNumber++;
    }

    public void ChangeSaveSet(ChangeSaveSetMode changeSaveSetMode, uint window)
    {
        var request = new ChangeSaveSetType(changeSaveSetMode, window);
        socket.Send(ref request);
        sequenceNumber++;
    }

    public void ChangeWindowAttributes(uint window, ValueMask mask, params uint[] args)
    {
        var request = new ChangeWindowAttributesType(window, mask, args.Length);
        var requiredBuffer = 12 + args.Length * 4;
        if (requiredBuffer < GlobalSetting.StackAllocThreshold)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
            MemoryMarshal.Write(scratchBuffer[0..12], request);
            MemoryMarshal.Cast<uint, byte>(args).CopyTo(scratchBuffer[12..requiredBuffer]);
            socket.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            MemoryMarshal.Write(scratchBuffer[0..12], request);
            MemoryMarshal.Cast<uint, byte>(args).CopyTo(scratchBuffer[12..requiredBuffer]);
            socket.SendExact(scratchBuffer[..requiredBuffer]);
        }

        sequenceNumber++;
    }

    public void CirculateWindow(Direction direction, uint window)
    {
        var request = new CirculateWindowType(direction, window);
        socket.Send(ref request);
        sequenceNumber++;
    }

    public void ClearArea(bool exposures, uint window, short x, short y, ushort width, ushort height)
    {
        var request = new ClearAreaType(exposures, window, x, y, width, height);
        socket.Send(ref request);
        sequenceNumber++;
    }

    public void CloseFont(uint fontId)
    {
        var request = new CloseFontType(fontId);
        socket.Send(ref request);
        sequenceNumber++;
    }

    public void ConfigureWindow(uint window, ConfigureValueMask mask, params uint[] args)
    {
        var requiredBuffer = 12 + args.Length * 4;
        var request = new ConfigureWindowType(window, mask, args.Length);
        if (requiredBuffer < GlobalSetting.StackAllocThreshold)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
            MemoryMarshal.Write(scratchBuffer[0..12], request);
            MemoryMarshal.Cast<uint, byte>(args)
                .CopyTo(scratchBuffer[12..requiredBuffer]);
            socket.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            MemoryMarshal.Write(scratchBuffer[0..12], request);
            MemoryMarshal.Cast<uint, byte>(args)
                .CopyTo(scratchBuffer[12..requiredBuffer]);
            socket.SendExact(scratchBuffer[..requiredBuffer]);
        }

        sequenceNumber++;
    }

    public void ConvertSelection(uint requestor, uint selection, uint target, uint property, uint timestamp)
    {
        var request = new ConvertSelectionType(requestor, selection, target, property, timestamp);
        socket.Send(ref request);
        sequenceNumber++;
    }

    public void CopyArea(uint srcDrawable, uint destDrawable, uint gc, ushort srcX, ushort srcY, ushort destX,
        ushort destY, ushort width, ushort height)
    {
        var request = new CopyAreaType(srcDrawable, destDrawable, gc, srcX, srcY, destX, destY, width, height);
        socket.Send(ref request);
        sequenceNumber++;
    }

    public void CopyColormapAndFree(uint colormapId, uint srcColormapId)
    {
        var request = new CopyColormapAndFreeType(colormapId, srcColormapId);
        socket.Send(ref request);
        sequenceNumber++;
    }

    public void CopyGC(uint srcGc, uint dstGc, GCMask mask)
    {
        var request = new CopyGCType(srcGc, dstGc, mask);
        socket.Send(ref request);
        sequenceNumber++;
    }

    public void CopyPlane(uint srcDrawable, uint destDrawable, uint gc, ushort srcX, ushort srcY, ushort destX,
        ushort destY, ushort width, ushort height, uint bitPlane)
    {
        var request = new CopyPlaneType(srcDrawable, destDrawable, gc, srcX, srcY, destX, destY, width, height,
            bitPlane);
        socket.Send(ref request);
        sequenceNumber++;
    }

    public void CreateColormap(ColormapAlloc alloc, uint colormapId, uint window, uint visual)
    {
        var request = new CreateColormapType(alloc, colormapId, window, visual);
        socket.Send(ref request);
        sequenceNumber++;
    }

    public void CreateCursor(uint cursorId, uint source, uint mask, ushort foreRed, ushort foreGreen, ushort foreBlue,
        ushort backRed, ushort backGreen, ushort backBlue, ushort x, ushort y)
    {
        var request = new CreateCursorType(cursorId, source, mask, foreRed, foreGreen, foreBlue, backRed, backGreen,
            backBlue, x, y);
        socket.Send(ref request);
        sequenceNumber++;
    }

    public void CreateGC(uint gc, uint drawable, GCMask mask, params uint[] args)
    {
        var request = new CreateGCType(gc, drawable, mask, args.Length);
        var requiredBuffer = 16 + (args.Length * 4);
        if (requiredBuffer < GlobalSetting.StackAllocThreshold)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
            MemoryMarshal.Write(scratchBuffer[0..16], request);
            MemoryMarshal.Cast<uint, byte>(args).CopyTo(scratchBuffer[16..requiredBuffer]);
            socket.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            MemoryMarshal.Write(scratchBuffer[0..16], request);
            MemoryMarshal.Cast<uint, byte>(args).CopyTo(scratchBuffer[16..requiredBuffer]);
            socket.SendExact(scratchBuffer[..requiredBuffer]);
        }

        sequenceNumber++;
    }

    public void CreateGlyphCursor(uint cursorId, uint sourceFont, uint fontMask, char sourceChar, ushort charMask,
        ushort foreRed, ushort foreGreen, ushort foreBlue, ushort backRed, ushort backGreen, ushort backBlue)
    {
        var request = new CreateGlyphCursorType(cursorId, sourceFont, fontMask, sourceChar, charMask, foreRed,
            foreGreen, foreBlue, backRed, backGreen, backBlue);
        socket.Send(ref request);
        sequenceNumber++;
    }

    public void CreatePixmap(byte depth, uint pixmapId, uint drawable, ushort width, ushort height)
    {
        var request = new CreatePixmapType(depth, pixmapId, drawable, width, height);
        socket.Send(ref request);
        sequenceNumber++;
    }

    public void CreateWindow(byte depth, uint window, uint parent, short x, short y, ushort width, ushort height,
        ushort borderWidth, ClassType classType, uint rootVisualId, ValueMask mask, params uint[] args)
    {
        var requiredBuffer = 32 + args.Length * 4;
        var request = new CreateWindowType(depth, window, parent, x, y, width, height, borderWidth, classType,
            rootVisualId, mask, args.Length);
        if (requiredBuffer < GlobalSetting.StackAllocThreshold)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
            MemoryMarshal.Write(scratchBuffer[0..32], request);
            MemoryMarshal.Cast<uint, byte>(args)
                .CopyTo(scratchBuffer[32..requiredBuffer]);
            socket.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            MemoryMarshal.Write(scratchBuffer[0..32], request);
            MemoryMarshal.Cast<uint, byte>(args)
                .CopyTo(scratchBuffer[32..requiredBuffer]);
            socket.SendExact(scratchBuffer[..requiredBuffer]);
        }

        sequenceNumber++;
    }

    public void DeleteProperty(uint window, uint atom)
    {
        var request = new DeletePropertyType(window, atom);
        socket.Send(ref request);
        sequenceNumber++;
    }

    public void DestroySubwindows(uint window)
    {
        var request = new DestroySubWindowsType(window);
        socket.Send(ref request);
        sequenceNumber++;
    }

    public void DestroyWindow(uint window)
    {
        var request = new DestroyWindowType(window);
        socket.Send(ref request);
        sequenceNumber++;
    }

    public void FillPoly(uint drawable, uint gc, PolyShape shape, CoordinateMode coordinate, Point[] points)
    {
        var request = new FillPolyType(drawable, gc, shape, coordinate, points.Length);
        var requiredBuffer = 16 + (points.Length * 4);
        if (requiredBuffer < GlobalSetting.StackAllocThreshold)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
            MemoryMarshal.Write(scratchBuffer[0..16], request);
            MemoryMarshal.Cast<Point, byte>(points)
                .CopyTo(scratchBuffer[16..]);
            socket.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            MemoryMarshal.Write(scratchBuffer[0..16], request);
            MemoryMarshal.Cast<Point, byte>(points)
                .CopyTo(scratchBuffer[16..requiredBuffer]);
            socket.SendExact(scratchBuffer[..requiredBuffer]);
        }

        sequenceNumber++;
    }

    public void ForceScreenSaver(ForceScreenSaverMode mode)
    {
        var request = new ForceScreenSaverType(mode);
        socket.Send(ref request);
        sequenceNumber++;
    }

    public void FreeColormap(uint colormapId)
    {
        var request = new FreeColormapType(colormapId);
        socket.Send(ref request);
        sequenceNumber++;
    }

    public void FreeColors(uint colormapId, uint planeMask, params uint[] pixels)
    {
        var requiredBuffer = 12 + pixels.Length * 4;
        var request = new FreeColorsType(colormapId, planeMask, pixels.Length);
        if (requiredBuffer < GlobalSetting.StackAllocThreshold)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
            MemoryMarshal.Write(scratchBuffer[0..12], request);
            MemoryMarshal.Cast<uint, byte>(pixels).CopyTo(scratchBuffer[12..requiredBuffer]);
            socket.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            MemoryMarshal.Write(scratchBuffer[0..12], request);
            MemoryMarshal.Cast<uint, byte>(pixels).CopyTo(scratchBuffer[12..requiredBuffer]);
            socket.SendExact(scratchBuffer[..requiredBuffer]);
        }

        sequenceNumber++;
    }

    public void FreeCursor(uint cursorId)
    {
        var request = new FreeCursorType(cursorId);
        socket.Send(ref request);
        sequenceNumber++;
    }

    public void FreeGC(uint gc)
    {
        var request = new FreeGCType(gc);
        socket.Send(ref request);
        sequenceNumber++;
    }

    public void FreePixmap(uint pixmapId)
    {
        var request = new FreePixmapType(pixmapId);
        socket.Send(ref request);
        sequenceNumber++;
    }

    public void GetAtomName()
    {
        throw new NotImplementedException();
        sequenceNumber++;
    }


    public InternAtomReply InternAtom(bool onlyIfExist, string atomName)
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

            socket.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            MemoryMarshal.Write(scratchBuffer[0..requestSize], in request);
            Encoding.ASCII.GetBytes(atomName, scratchBuffer.Slice(requestSize, atomName.Length));
            scratchBuffer[(requestSize + atomName.Length)..requiredBuffer].Clear();
            socket.SendExact(scratchBuffer[..requiredBuffer]);
        }

        var (result, error) = ReceivedResponse<InternAtomReply>();
        if (!error.HasValue && result.HasValue)
        {
            sequenceNumber++;
            Debug.Assert(sequenceNumber == result.Value.Sequence);
            return result.Value;
        }
        throw new XEventException(error!.Value);
    }

    public void GetFontPath()
    {
        throw new NotImplementedException();
        sequenceNumber++;
    }


    public void GetGeometry()
    {
        throw new NotImplementedException();
        sequenceNumber++;
    }


    public void GetImage()
    {
        throw new NotImplementedException();
        sequenceNumber++;
    }


    public void GetInputFocus()
    {
        throw new NotImplementedException();
        sequenceNumber++;
    }


    public void GetKeyboardControl()
    {
        throw new NotImplementedException();
        sequenceNumber++;
    }


    public void GetKeyboardMapping()
    {
        throw new NotImplementedException();
        sequenceNumber++;
    }


    public void GetModifierMapping()
    {
        throw new NotImplementedException();
        sequenceNumber++;
    }


    public void GetMotionEvents()
    {
        throw new NotImplementedException();
        sequenceNumber++;
    }


    public void GetPointerControl()
    {
        throw new NotImplementedException();
        sequenceNumber++;
    }


    public void GetPointerMapping()
    {
        throw new NotImplementedException();
        sequenceNumber++;
    }


    public GetPropertyReply GetProperty(bool delete, uint window, uint property, uint type, uint offset, uint length)
    {
        var request = new GetPropertyType(delete, window, property, type, offset, length);
        socket.Send(ref request);
        sequenceNumber++;
        return new GetPropertyReply(socket);
    }

    public void GetScreenSaver()
    {
        throw new NotImplementedException();
        sequenceNumber++;
    }


    public void GetSelectionOwner()
    {
        throw new NotImplementedException();
        sequenceNumber++;
    }


    public void GetWindowAttributes()
    {
        throw new NotImplementedException();
        sequenceNumber++;
    }


    public void GrabButton(bool ownerEvents, uint grabWindow, ushort mask, GrabMode pointerMode, GrabMode keyboardMode,
        uint confineTo, uint cursor, Button button, ModifierMask modifiers)
    {
        var request = new GrabButtonType(ownerEvents, grabWindow, mask, pointerMode, keyboardMode, confineTo, cursor,
            button, modifiers);
        socket.Send(ref request);
        sequenceNumber++;
    }

    public void GrabKey(bool exposures, uint grabWindow, ModifierMask mask, byte keycode, GrabMode pointerMode,
        GrabMode keyboardMode)
    {
        var request = new GrabKeyType(exposures, grabWindow, mask, keycode, pointerMode, keyboardMode);
        socket.Send(ref request);
        sequenceNumber++;
    }

    public void GrabKeyboard()
    {
        throw new NotImplementedException();
        sequenceNumber++;
    }


    public GrabPointerReply GrabPointer(bool ownerEvents, uint grabWindow, ushort mask, GrabMode pointerMode,
        GrabMode keyboardMode, uint confineTo, uint cursor, uint timeStamp)
    {
        var request = new GrabPointerType(ownerEvents, grabWindow, mask, pointerMode, keyboardMode, confineTo, cursor,
            timeStamp);
        socket.Send(ref request);

        var (result, error) = ReceivedResponse<GrabPointerReply>();
        if (!error.HasValue && result.HasValue)
        {
            sequenceNumber++;
            Debug.Assert(sequenceNumber == result.Value.Sequence);
            return result.Value;
        }
        throw new XEventException(error!.Value);
    }

    public void GrabServer()
    {
        var request = new GrabServerType();
        socket.Send(ref request);
        sequenceNumber++;
    }

    public void ImageText16(uint drawable, uint gc, short x, short y, ReadOnlySpan<char> text)
    {
        var request = new ImageText16Type(drawable, gc, x, y, text.Length);
        var requiredBuffer = 16 + (text.Length * 2).AddPadding();
        if (requiredBuffer < GlobalSetting.StackAllocThreshold)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
            MemoryMarshal.Write(scratchBuffer[0..16], request);
            Encoding.BigEndianUnicode.GetBytes(text, scratchBuffer[16..(text.Length * 2 + 16)]);
            scratchBuffer[(16 + text.Length * 2)..requiredBuffer].Clear();
            socket.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            MemoryMarshal.Write(scratchBuffer[0..16], request);
            Encoding.BigEndianUnicode.GetBytes(text, scratchBuffer[16..(text.Length * 2 + 16)]);
            scratchBuffer[(16 + text.Length * 2)..requiredBuffer].Clear();
            socket.SendExact(scratchBuffer[..requiredBuffer]);
        }

        sequenceNumber++;
    }

    public void ImageText8(uint drawable, uint gc, short x, short y, ReadOnlySpan<byte> text)
    {
        var request = new ImageText8Type(drawable, gc, x, y, text.Length);
        var requiredBuffer = 16 + text.Length.AddPadding();
        if (requiredBuffer < GlobalSetting.StackAllocThreshold)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
            MemoryMarshal.Write(scratchBuffer[0..16], request);
            text.CopyTo(scratchBuffer[16..(text.Length + 16)]);
            scratchBuffer[(16 + text.Length)..requiredBuffer].Clear();
            socket.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            MemoryMarshal.Write(scratchBuffer[0..16], request);
            text.CopyTo(scratchBuffer[16..(text.Length + 16)]);
            scratchBuffer[(16 + text.Length)..requiredBuffer].Clear();
            socket.SendExact(scratchBuffer[..requiredBuffer]);
        }

        sequenceNumber++;
    }

    public void InstallColormap(uint colormapId)
    {
        var request = new InstallColormapType(colormapId);
        socket.Send(ref request);
        sequenceNumber++;
    }

    public void KillClient(uint resource)
    {
        var request = new KillClientType(resource);
        socket.Send(ref request);
        sequenceNumber++;
    }

    public void ListExtensions()
    {
        throw new NotImplementedException();
        sequenceNumber++;
    }


    public void ListFonts()
    {
        throw new NotImplementedException();
        sequenceNumber++;
    }


    public void ListFontsWithInfo()
    {
        throw new NotImplementedException();
        sequenceNumber++;
    }


    public void ListHosts()
    {
        throw new NotImplementedException();
        sequenceNumber++;
    }


    public void ListInstalledColormaps()
    {
        throw new NotImplementedException();
        sequenceNumber++;
    }


    public void ListProperties()
    {
        throw new NotImplementedException();
        sequenceNumber++;
    }


    public void LookupColor()
    {
        throw new NotImplementedException();
        sequenceNumber++;
    }


    public void MapSubwindows(uint window)
    {
        var request = new MapSubWindowsType(window);
        socket.Send(ref request);
        sequenceNumber++;
    }

    public void MapWindow(uint window)
    {
        var request = new MapWindowType(window);
        socket.Send(ref request);
        sequenceNumber++;
    }

    public void NoOperation(params uint[] args)
    {
        var request = new NoOperationType(args.Length);
        var requiredBuffer = 4 + args.Length * 4;
        if (requiredBuffer < GlobalSetting.StackAllocThreshold)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
            MemoryMarshal.Write(scratchBuffer[0..4], request);
            args.AsSpan().CopyTo(MemoryMarshal.Cast<byte, uint>(scratchBuffer[4..requiredBuffer]));
            socket.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            MemoryMarshal.Write(scratchBuffer[0..4], request);
            args.AsSpan().CopyTo(MemoryMarshal.Cast<byte, uint>(scratchBuffer[4..requiredBuffer]));
            socket.SendExact(scratchBuffer[..requiredBuffer]);
        }

        sequenceNumber++;
    }

    public void OpenFont(string fontName, uint fontId)
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

            socket.SendExact(scratchBuffer);
        }
        else
        {
            requiredBuffer -= requestSize;

            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            Encoding.ASCII.GetBytes(fontName, scratchBuffer.Slice(fontName.Length));
            scratchBuffer[(fontName.Length)..requiredBuffer].Clear();

            socket.Send(ref request);
            socket.SendExact(scratchBuffer[..requiredBuffer]);
        }

        sequenceNumber++;
    }

    public void PolyArc(uint drawable, uint gc, Arc[] arcs)
    {
        var request = new PolyArcType(drawable, gc, arcs.Length);
        var requiredBuffer = Marshal.SizeOf<PolyArcType>() + (arcs.Length * 12);
        if (requiredBuffer < GlobalSetting.StackAllocThreshold)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
            MemoryMarshal.Write(scratchBuffer[0..12], request);
            MemoryMarshal.Cast<Arc, byte>(arcs)
                .CopyTo(scratchBuffer[12..requiredBuffer]);
            socket.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            MemoryMarshal.Write(scratchBuffer[0..12], request);
            MemoryMarshal.Cast<Arc, byte>(arcs)
                .CopyTo(scratchBuffer[12..requiredBuffer]);
            socket.SendExact(scratchBuffer[..requiredBuffer]);
        }

        sequenceNumber++;
    }

    public void PolyFillArc(uint drawable, uint gc, Arc[] arcs)
    {
        var request = new PolyFillArcType(drawable, gc, arcs.Length);
        var requiredBuffer = Marshal.SizeOf<PolyFillArcType>() + (arcs.Length * 12);
        if (requiredBuffer < GlobalSetting.StackAllocThreshold)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
            MemoryMarshal.Write(scratchBuffer[0..12], request);
            MemoryMarshal.Cast<Arc, byte>(arcs)
                .CopyTo(scratchBuffer[12..requiredBuffer]);
            socket.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            MemoryMarshal.Write(scratchBuffer[0..12], request);
            MemoryMarshal.Cast<Arc, byte>(arcs)
                .CopyTo(scratchBuffer[12..requiredBuffer]);
            socket.SendExact(scratchBuffer[..requiredBuffer]);
        }

        sequenceNumber++;
    }

    public void PolyFillRectangle(uint drawable, uint gc, Rectangle[] rectangles)
    {
        var request = new PolyFillRectangleType(drawable, gc, rectangles.Length);
        var requiredBuffer = Marshal.SizeOf<PolyFillRectangleType>() + (rectangles.Length * 8);
        if (requiredBuffer < GlobalSetting.StackAllocThreshold)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
            MemoryMarshal.Write(scratchBuffer[0..12], request);
            MemoryMarshal.Cast<Rectangle, byte>(rectangles)
                .CopyTo(scratchBuffer[12..(12 + rectangles.Length * 8)]);
            socket.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            MemoryMarshal.Write(scratchBuffer[0..12], request);
            MemoryMarshal.Cast<Rectangle, byte>(rectangles)
                .CopyTo(scratchBuffer[12..(12 + rectangles.Length * 8)]);
            socket.SendExact(scratchBuffer[..requiredBuffer]);
        }

        sequenceNumber++;
    }

    public void PolyLine(CoordinateMode coordinate, uint drawable, uint gc, Point[] points)
    {
        var request = new PolyLineType(coordinate, drawable, gc, points.Length);
        var requiredBuffer = 12 + (points.Length * 4);
        if (requiredBuffer < GlobalSetting.StackAllocThreshold)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
            MemoryMarshal.Write(scratchBuffer[0..12], request);
            MemoryMarshal.Cast<Point, byte>(points)
                .CopyTo(scratchBuffer[12..requiredBuffer]);
            socket.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            MemoryMarshal.Write(scratchBuffer[0..12], request);
            MemoryMarshal.Cast<Point, byte>(points)
                .CopyTo(scratchBuffer[12..requiredBuffer]);
            socket.SendExact(scratchBuffer[..requiredBuffer]);
        }

        sequenceNumber++;
    }

    public void PolyPoint(CoordinateMode coordinate, uint drawable, uint gc, Point[] points)
    {
        var request = new PolyPointType(coordinate, drawable, gc, points.Length);
        var requiredBuffer = 12 + (points.Length * 4);
        if (requiredBuffer < GlobalSetting.StackAllocThreshold)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
            MemoryMarshal.Write(scratchBuffer[0..12], request);
            MemoryMarshal.Cast<Point, byte>(points)
                .CopyTo(scratchBuffer[12..requiredBuffer]);
            socket.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            MemoryMarshal.Write(scratchBuffer[0..12], request);
            MemoryMarshal.Cast<Point, byte>(points)
                .CopyTo(scratchBuffer[12..requiredBuffer]);
            socket.SendExact(scratchBuffer[..requiredBuffer]);
        }

        sequenceNumber++;
    }

    public void PolyRectangle(uint drawable, uint gc, Rectangle[] rectangles)
    {
        var request = new PolyRectangleType(drawable, gc, rectangles.Length);
        var requiredBuffer = 12 + (rectangles.Length * 8);
        if (requiredBuffer < GlobalSetting.StackAllocThreshold)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
            MemoryMarshal.Write(scratchBuffer[0..12], request);
            MemoryMarshal.Cast<Rectangle, byte>(rectangles)
                .CopyTo(scratchBuffer[12..requiredBuffer]);
            socket.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            MemoryMarshal.Write(scratchBuffer[0..12], request);
            MemoryMarshal.Cast<Rectangle, byte>(rectangles)
                .CopyTo(scratchBuffer[12..requiredBuffer]);
            socket.SendExact(scratchBuffer[..requiredBuffer]);
        }

        sequenceNumber++;
    }

    public void PolySegment(uint drawable, uint gc, Segment[] segments)
    {
        var request = new PolySegmentType(drawable, gc, segments.Length);
        var requiredBuffer = 12 + (segments.Length * 8);
        if (requiredBuffer < GlobalSetting.StackAllocThreshold)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
            MemoryMarshal.Write(scratchBuffer[0..12], request);
            MemoryMarshal.Cast<Segment, byte>(segments)
                .CopyTo(scratchBuffer[12..requiredBuffer]);
            socket.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            MemoryMarshal.Write(scratchBuffer[0..12], request);
            MemoryMarshal.Cast<Segment, byte>(segments)
                .CopyTo(scratchBuffer[12..requiredBuffer]);
            socket.SendExact(scratchBuffer[..requiredBuffer]);
        }

        sequenceNumber++;
    }

    public void PolyText16(uint drawable, uint gc, ushort x, ushort y, Span<byte> data)
    {
        var request = new PolyText16Type(drawable, gc, x, y, data.Length);
        var scratchBufferSize = 16 + data.Length.AddPadding();
        if (scratchBufferSize < GlobalSetting.StackAllocThreshold)
        {
            Span<byte> scratchBuffer = stackalloc byte[scratchBufferSize];
            MemoryMarshal.Write(scratchBuffer[..16], in request);
            data.CopyTo(scratchBuffer[16..]);
            scratchBuffer[^data.Length.Padding()..].Clear();
            socket.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(scratchBufferSize);
            MemoryMarshal.Write(scratchBuffer[..16], in request);
            data.CopyTo(scratchBuffer[16..]);
            scratchBuffer[^data.Length.Padding()..].Clear();
            socket.SendExact(scratchBuffer[..scratchBufferSize]);
        }
        sequenceNumber++;
    }


    public void PolyText8(uint drawable, uint gc, ushort x, ushort y, Span<byte> data)
    {
        var request = new PolyText8Type(drawable, gc, x, y, data.Length);
        var scratchBufferSize = 16 + data.Length.AddPadding();
        if (scratchBufferSize < GlobalSetting.StackAllocThreshold)
        {
            Span<byte> scratchBuffer = stackalloc byte[scratchBufferSize];
            MemoryMarshal.Write(scratchBuffer[..16], in request);
            data.CopyTo(scratchBuffer[16..]);
            scratchBuffer[^data.Length.Padding()..].Clear();
            socket.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(scratchBufferSize);
            MemoryMarshal.Write(scratchBuffer[..16], in request);
            data.CopyTo(scratchBuffer[16..]);
            scratchBuffer[^data.Length.Padding()..].Clear();
            socket.SendExact(scratchBuffer[..scratchBufferSize]);
        }
        sequenceNumber++;
    }


    public void PutImage(ImageFormat format, uint drawable, uint gc, ushort width, ushort height, short x, short y,
        byte leftPad, byte depth, Span<byte> data)
    {
        var request = new PutImageType(format, drawable, gc, width, height, x, y, leftPad, depth, data.Length);
        var scratchBufferSize = data.Length.AddPadding() + 24;
        if (scratchBufferSize < GlobalSetting.StackAllocThreshold)
        {
            Span<byte> scratchBuffer = stackalloc byte[scratchBufferSize];
            MemoryMarshal.Write(scratchBuffer[..24], in request);
            data.CopyTo(scratchBuffer[24..(24 + data.Length)]);
            scratchBuffer[(24 + data.Length)..].Clear();
            socket.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(scratchBufferSize);
            MemoryMarshal.Write(scratchBuffer[..24], in request);
            data.CopyTo(scratchBuffer[24..(24 + data.Length)]);
            scratchBuffer[(24 + data.Length)..].Clear();
            socket.SendExact(scratchBuffer[..scratchBufferSize]);
        }

        sequenceNumber++;
    }

    public void QueryBestSize()
    {
        throw new NotImplementedException();
        sequenceNumber++;
    }


    public void QueryColors()
    {
        throw new NotImplementedException();
        sequenceNumber++;
    }


    public void QueryExtension()
    {
        throw new NotImplementedException();
        sequenceNumber++;
    }


    public void QueryFont()
    {
        throw new NotImplementedException();
        sequenceNumber++;
    }


    public void QueryKeymap()
    {
        throw new NotImplementedException();
        sequenceNumber++;
    }


    public QueryPointerReply QueryPointer(uint window)
    {
        var request = new QueryPointerType(window);
        socket.Send(ref request);
        
        var (result, error) = ReceivedResponse<QueryPointerReply>();
        if (!error.HasValue && result.HasValue)
        {
            sequenceNumber++;
            Debug.Assert(sequenceNumber == result.Value.Sequence);
            return result.Value;
        }
        throw new XEventException(error!.Value);
    }

    public void QueryTextExtents()
    {
        throw new NotImplementedException();
        sequenceNumber++;
    }


    public void QueryTree()
    {
        throw new NotImplementedException();
        sequenceNumber++;
    }


    public void RecolorCursor(uint cursorId, ushort foreRed, ushort foreGreen, ushort foreBlue, ushort backRed,
        ushort backGreen, ushort backBlue)
    {
        var request = new RecolorCursorType(cursorId, foreRed, foreGreen, foreBlue, backRed, backGreen, backBlue);
        socket.Send(ref request);
        sequenceNumber++;
    }

    public void ReparentWindow(uint window, uint parent, short x, short y)
    {
        var request = new ReparentWindowType(window, parent, x, y);
        socket.Send(ref request);
        sequenceNumber++;
    }

    public void RotateProperties(uint window, ushort delta, params uint[] properties)
    {
        var request = new RotatePropertiesType(window, properties.Length, delta);
        var requiredBuffer = 12 + properties.Length * 4;
        if (requiredBuffer < GlobalSetting.StackAllocThreshold)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
            MemoryMarshal.Write(scratchBuffer[0..12], request);
            MemoryMarshal.Cast<uint, byte>(properties).CopyTo(scratchBuffer[12..requiredBuffer]);
            socket.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            MemoryMarshal.Write(scratchBuffer[0..12], request);
            MemoryMarshal.Cast<uint, byte>(properties)
                .CopyTo(scratchBuffer[12..requiredBuffer]);
            socket.SendExact(scratchBuffer[..requiredBuffer]);
        }

        sequenceNumber++;
    }

    public void SendEvent(bool propagate, uint destination, uint eventMask, XEvent evnt)
    {
        var request = new SendEventType(propagate, destination, eventMask, evnt);
        socket.Send(ref request);
        sequenceNumber++;
    }

    public void SetAccessControl(AccessControlMode mode)
    {
        var request = new SetAccessControlType(mode);
        socket.Send(ref request);
        sequenceNumber++;
    }

    public void SetClipRectangles(ClipOrdering ordering, uint gc, ushort clipX, ushort clipY, Rectangle[] rectangles)
    {
        var request = new SetClipRectanglesType(ordering, gc, clipX, clipY, rectangles.Length);
        var requiredBuffer = 12 + rectangles.Length * Marshal.SizeOf<Rectangle>();
        if (requiredBuffer < GlobalSetting.StackAllocThreshold)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
            MemoryMarshal.Write(scratchBuffer[0..12], request);
            MemoryMarshal.Cast<Rectangle, byte>(rectangles).CopyTo(scratchBuffer[12..requiredBuffer]);
            socket.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            MemoryMarshal.Write(scratchBuffer[0..12], request);
            MemoryMarshal.Cast<Rectangle, byte>(rectangles).CopyTo(scratchBuffer[12..requiredBuffer]);
            socket.SendExact(scratchBuffer[..requiredBuffer]);
        }

        sequenceNumber++;
    }

    public void SetCloseDownMode(CloseDownMode mode)
    {
        var request = new SetCloseDownModeType(mode);
        socket.Send(ref request);
        sequenceNumber++;
    }

    public void SetDashes(uint gc, ushort dashOffset, byte[] dashes)
    {
        var request = new SetDashesType(gc, dashOffset, dashes.Length);
        var requiredBuffer = 12 + dashes.Length.AddPadding();
        if (requiredBuffer < GlobalSetting.StackAllocThreshold)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
            MemoryMarshal.Write(scratchBuffer[0..12], request);
            dashes.CopyTo(scratchBuffer[12..requiredBuffer]);
            scratchBuffer[^dashes.Length.Padding()..].Clear();
            socket.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            MemoryMarshal.Write(scratchBuffer[0..12], request);
            dashes.CopyTo(scratchBuffer[12..requiredBuffer]);
            scratchBuffer[^dashes.Length.Padding()..].Clear();
            socket.SendExact(scratchBuffer[..requiredBuffer]);
        }

        sequenceNumber++;
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
            socket.SendExact(scratchBuffer);
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
            socket.SendExact(scratchBuffer[..requiredBuffer]);
        }

        sequenceNumber++;
    }

    public void SetInputFocus(InputFocusMode mode, uint focus, uint time)
    {
        var request = new SetInputFocusType(mode, focus, time);
        socket.Send(ref request);
        sequenceNumber++;
    }

    public void SetModifierMapping()
    {
        throw new NotImplementedException();
        sequenceNumber++;
    }


    public void SetPointerMapping()
    {
        throw new NotImplementedException();
        sequenceNumber++;
    }


    public void SetScreenSaver(short timeout, short interval, TriState preferBlanking, TriState allowExposures)
    {
        var request = new SetScreenSaverType(timeout, interval, preferBlanking, allowExposures);
        socket.Send(ref request);
        sequenceNumber++;
    }

    public void SetSelectionOwner(uint owner, uint atom, uint timestamp)
    {
        var request = new SetSelectionOwnerType(owner, atom, timestamp);
        socket.Send(ref request);
        sequenceNumber++;
    }

    public void StoreColors(uint colormapId, params ColorItem[] item)
    {
        var request = new StoreColorsType(colormapId, item.Length);
        var requiredBuffer = 8 + 12 * item.Length;
        if (requiredBuffer < GlobalSetting.StackAllocThreshold)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
            MemoryMarshal.Write(scratchBuffer[0..8], request);
            item.CopyTo(MemoryMarshal.Cast<byte, ColorItem>(scratchBuffer[8..requiredBuffer]));
            scratchBuffer[^1] = 0;
            socket.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            MemoryMarshal.Write(scratchBuffer[0..8], request);
            item.CopyTo(MemoryMarshal.Cast<byte, ColorItem>(scratchBuffer[8..requiredBuffer]));
            scratchBuffer[requiredBuffer - 1] = 0;
            socket.SendExact(scratchBuffer[..requiredBuffer]);
        }

        sequenceNumber++;
    }

    public void StoreNamedColor(ColorFlag mode, uint colormapId, uint pixels, ReadOnlySpan<byte> name)
    {
        var request = new StoreNamedColorType(mode, colormapId, pixels, name.Length);
        var requiredBuffer = 16 + name.Length.AddPadding();
        if (requiredBuffer < GlobalSetting.StackAllocThreshold)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
            MemoryMarshal.Write(scratchBuffer[0..16], request);
            name.CopyTo(scratchBuffer[16..(name.Length + 16)]);
            scratchBuffer[^name.Length.Padding()..].Clear();
            socket.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            MemoryMarshal.Write(scratchBuffer[0..16], request);
            name.CopyTo(scratchBuffer[16..(name.Length + 16)]);
            scratchBuffer[^name.Length.Padding()..].Clear();
            socket.SendExact(scratchBuffer[..requiredBuffer]);
        }

        sequenceNumber++;
    }

    public void TranslateCoordinates()
    {
        throw new NotImplementedException();
        sequenceNumber++;
    }


    public void UngrabButton(Button button, uint grabWindow, ModifierMask mask)
    {
        var request = new UngrabButtonType(button, grabWindow, mask);
        socket.Send(ref request);
        sequenceNumber++;
    }

    public void UngrabKey(byte key, uint grabWindow, ModifierMask modifier)
    {
        var request = new UngrabKeyType(key, grabWindow, modifier);
        socket.Send(ref request);
        sequenceNumber++;
    }

    public void UngrabKeyboard(uint time)
    {
        var request = new UngrabKeyboardType(time);
        socket.Send(ref request);
        sequenceNumber++;
    }

    public void UngrabPointer(uint time)
    {
        var request = new UngrabPointerType(time);
        socket.Send(ref request);
        sequenceNumber++;
    }

    public void UngrabServer()
    {
        var request = new UnGrabServerType();
        socket.Send(ref request);
        sequenceNumber++;
    }

    public void UninstallColormap(uint colormapId)
    {
        var request = new UninstallColormapType(colormapId);
        socket.Send(ref request);
        sequenceNumber++;
    }

    public void UnmapSubwindows(uint window)
    {
        var request = new UnMapSubwindowsType(window);
        socket.Send(ref request);
        sequenceNumber++;
    }

    public void UnmapWindow(uint window)
    {
        var request = new UnmapWindowType(window);
        socket.Send(ref request);
        sequenceNumber++;
    }

    public void WarpPointer(uint srcWindow, uint destWindow, short srcX, short srcY, ushort srcWidth, ushort srcHeight,
        short destX, short destY)
    {
        var request = new WarpPointerType(srcWindow, destWindow, srcX, srcY, srcWidth, srcHeight, destX, destY);
        socket.Send(ref request);
        sequenceNumber++;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
                if (socket.Connected)
                    socket.Close();

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
        if (bufferEvents.TryPop(out var result))
        {
            return result;
        }

        Span<byte> scratchBuffer = stackalloc byte[Marshal.SizeOf<XEvent>()];
        if (socket.Poll(-1, SelectMode.SelectRead))
        {
            var totalRead = socket.Receive(scratchBuffer);
            if (totalRead != 0)
                return scratchBuffer.ToStruct<XEvent>();
        }

        scratchBuffer.Clear();
        return scratchBuffer.ToStruct<XEvent>();
    }

    private void CheckError([CallerMemberName] string name = "")
    {
        var error = this.Received();
        if (error.HasValue) throw new XEventException(error.Value);
    }

    public void CreateWindowChecked(byte depth, uint window, uint parent, short x, short y, ushort width, ushort height,
        ushort borderWidth, ClassType classType, uint rootVisualId, ValueMask mask, params uint[] args)
    {
        this.CreateWindow(depth, window, parent, x, y, width, height, borderWidth, classType, rootVisualId, mask, args);
        CheckError();
    }

    public void ChangeWindowAttributesChecked(uint window, ValueMask mask, params uint[] args)
    {
        this.ChangeWindowAttributes(window, mask, args);
        CheckError();
    }

    public void DestroyWindowChecked(uint window)
    {
        this.DestroyWindow(window);
        CheckError();
    }

    public void DestroySubwindowsChecked(uint window)
    {
        this.DestroySubwindows(window);
        CheckError();
    }

    public void ChangeSaveSetChecked(ChangeSaveSetMode changeSaveSetMode, uint window)
    {
        this.ChangeSaveSet(changeSaveSetMode, window);
        CheckError();
    }

    public void ReparentWindowChecked(uint window, uint parent, short x, short y)
    {
        this.ReparentWindow(window, parent, x, y);
        CheckError();
    }

    public void MapWindowChecked(uint window)
    {
        this.MapWindow(window);
        CheckError();
    }

    public void MapSubwindowsChecked(uint window)
    {
        this.MapSubwindows(window);
        CheckError();
    }

    public void UnmapWindowChecked(uint window)
    {
        this.UnmapWindow(window);
        CheckError();
    }

    public void UnmapSubwindowsChecked(uint window)
    {
        this.UnmapSubwindows(window);
        CheckError();
    }

    public void ConfigureWindowChecked(uint window, ConfigureValueMask mask, params uint[] args)
    {
        this.ConfigureWindow(window, mask, args);
        CheckError();
    }

    public void CirculateWindowChecked(Direction direction, uint window)
    {
        this.CirculateWindow(direction, window);
        CheckError();
    }

    public void ChangePropertyChecked<T>(PropertyMode mode, uint window, uint property, uint type, params T[] args)
        where T : struct, INumber<T>
    {
        this.ChangeProperty(mode, window, property, type, args);
        CheckError();
    }

    public void DeletePropertyChecked(uint window, uint atom)
    {
        this.DeleteProperty(window, atom);
        CheckError();
    }

    public void RotatePropertiesChecked(uint window, ushort delta, params uint[] properties)
    {
        this.RotateProperties(window, delta, properties);
        CheckError();
    }

    public void SetSelectionOwnerChecked(uint owner, uint atom, uint timestamp)
    {
        this.SetSelectionOwner(owner, atom, timestamp);
        CheckError();
    }

    public void ConvertSelectionChecked(uint requestor, uint selection, uint target, uint property, uint timestamp)
    {
        this.ConvertSelection(requestor, selection, target, property, timestamp);
        CheckError();
    }

    public void SendEventChecked(bool propagate, uint destination, uint eventMask, XEvent evnt)
    {
        this.SendEvent(propagate, destination, eventMask, evnt);
        CheckError();
    }

    public void UngrabPointerChecked(uint time)
    {
        this.UngrabPointer(time);
        CheckError();
    }

    public void GrabButtonChecked(bool ownerEvents, uint grabWindow, ushort mask, GrabMode pointerMode,
        GrabMode keyboardMode, uint confineTo, uint cursor, Button button, ModifierMask modifiers)
    {
        this.GrabButton(ownerEvents, grabWindow, mask, pointerMode, keyboardMode, confineTo, cursor, button, modifiers);
        CheckError();
    }

    public void UngrabButtonChecked(Button button, uint grabWindow, ModifierMask mask)
    {
        this.UngrabButton(button, grabWindow, mask);
        CheckError();
    }

    public void ChangeActivePointerGrabChecked(uint cursor, uint time, ushort mask)
    {
        this.ChangeActivePointerGrab(cursor, time, mask);
        CheckError();
    }

    public void UngrabKeyboardChecked(uint time)
    {
        this.UngrabKeyboard(time);
        CheckError();
    }

    public void GrabKeyChecked(bool exposures, uint grabWindow, ModifierMask mask, byte keycode, GrabMode pointerMode,
        GrabMode keyboardMode)
    {
        this.GrabKey(exposures, grabWindow, mask, keycode, pointerMode, keyboardMode);
        CheckError();
    }

    public void UngrabKeyChecked(byte key, uint grabWindow, ModifierMask modifier)
    {
        this.UngrabKey(key, grabWindow, modifier);
        CheckError();
    }

    public void AllowEventsChecked(EventsMode mode, uint time)
    {
        this.AllowEvents(mode, time);
        CheckError();
    }

    public void GrabServerChecked()
    {
        this.GrabServer();
        CheckError();
    }

    public void UngrabServerChecked()
    {
        this.UngrabServer();
        CheckError();
    }

    public void WarpPointerChecked(uint srcWindow, uint destWindow, short srcX, short srcY, ushort srcWidth,
        ushort srcHeight, short destX, short destY)
    {
        this.WarpPointer(srcWindow, destWindow, srcX, srcY, srcWidth, srcHeight, destX, destY);
        CheckError();
    }

    public void SetInputFocusChecked(InputFocusMode mode, uint focus, uint time)
    {
        this.SetInputFocus(mode, focus, time);
        CheckError();
    }

    public void OpenFontChecked(string fontName, uint fontId)
    {
        this.OpenFont(fontName, fontId);
        CheckError();
    }

    public void CloseFontChecked(uint fontId)
    {
        this.CloseFont(fontId);
        CheckError();
    }

    public void SetFontPathChecked(string[] strPaths)
    {
        this.SetFontPath(strPaths);
        CheckError();
    }

    public void CreatePixmapChecked(byte depth, uint pixmapId, uint drawable, ushort width, ushort height)
    {
        this.CreatePixmap(depth, pixmapId, drawable, width, height);
        CheckError();
    }

    public void FreePixmapChecked(uint pixmapId)
    {
        this.FreePixmap(pixmapId);
        CheckError();
    }

    public void CreateGCChecked(uint gc, uint drawable, GCMask mask, params uint[] args)
    {
        this.CreateGC(gc, drawable, mask, args);
        CheckError();
    }

    public void ChangeGCChecked(uint gc, GCMask mask, params uint[] args)
    {
        this.ChangeGC(gc, mask, args);
        CheckError();
    }

    public void CopyGCChecked(uint srcGc, uint dstGc, GCMask mask)
    {
        this.CopyGC(srcGc, dstGc, mask);
        CheckError();
    }

    public void SetDashesChecked(uint gc, ushort dashOffset, byte[] dashes)
    {
        this.SetDashes(gc, dashOffset, dashes);
        CheckError();
    }

    public void SetClipRectanglesChecked(ClipOrdering ordering, uint gc, ushort clipX, ushort clipY,
        Rectangle[] rectangles)
    {
        this.SetClipRectangles(ordering, gc, clipX, clipY, rectangles);
        CheckError();
    }

    public void FreeGCChecked(uint gc)
    {
        this.FreeGC(gc);
        CheckError();
    }

    public void ClearAreaChecked(bool exposures, uint window, short x, short y, ushort width, ushort height)
    {
        this.ClearArea(exposures, window, x, y, width, height);
        CheckError();
    }

    public void CopyAreaChecked(uint srcDrawable, uint destDrawable, uint gc, ushort srcX, ushort srcY, ushort destX,
        ushort destY, ushort width, ushort height)
    {
        this.CopyArea(srcDrawable, destDrawable, gc, srcX, srcY, destX, destY, width, height);
        CheckError();
    }

    public void CopyPlaneChecked(uint srcDrawable, uint destDrawable, uint gc, ushort srcX, ushort srcY, ushort destX,
        ushort destY, ushort width, ushort height, uint bitPlane)
    {
        this.CopyPlane(srcDrawable, destDrawable, gc, srcX, srcY, destX, destY, width, height, bitPlane);
        CheckError();
    }

    public void PolyPointChecked(CoordinateMode coordinate, uint drawable, uint gc, Point[] points)
    {
        this.PolyPoint(coordinate, drawable, gc, points);
        CheckError();
    }

    public void PolyLineChecked(CoordinateMode coordinate, uint drawable, uint gc, Point[] points)
    {
        this.PolyLine(coordinate, drawable, gc, points);
        CheckError();
    }

    public void PolySegmentChecked(uint drawable, uint gc, Segment[] segments)
    {
        this.PolySegment(drawable, gc, segments);
        CheckError();
    }

    public void PolyRectangleChecked(uint drawable, uint gc, Rectangle[] rectangles)
    {
        this.PolyRectangle(drawable, gc, rectangles);
        CheckError();
    }

    public void PolyArcChecked(uint drawable, uint gc, Arc[] arcs)
    {
        this.PolyArc(drawable, gc, arcs);
        CheckError();
    }

    public void FillPolyChecked(uint drawable, uint gc, PolyShape shape, CoordinateMode coordinate, Point[] points)
    {
        this.FillPoly(drawable, gc, shape, coordinate, points);
        CheckError();
    }

    public void PolyFillRectangleChecked(uint drawable, uint gc, Rectangle[] rectangles)
    {
        this.PolyFillRectangle(drawable, gc, rectangles);
        CheckError();
    }

    public void PolyFillArcChecked(uint drawable, uint gc, Arc[] arcs)
    {
        this.PolyFillArc(drawable, gc, arcs);
        CheckError();
    }

    public void PutImageChecked(ImageFormat format, uint drawable, uint gc, ushort width, ushort height, short x,
        short y, byte leftPad, byte depth, Span<byte> data)
    {
        this.PutImage(format, drawable, gc, width, height, x, y, leftPad, depth, data);
        CheckError();
    }

    public void ImageText8Checked(uint drawable, uint gc, short x, short y, ReadOnlySpan<byte> text)
    {
        this.ImageText8(drawable, gc, x, y, text);
        CheckError();
    }

    public void ImageText16Checked(uint drawable, uint gc, short x, short y, ReadOnlySpan<char> text)
    {
        this.ImageText16(drawable, gc, x, y, text);
        CheckError();
    }

    public void CreateColormapChecked(ColormapAlloc alloc, uint colormapId, uint window, uint visual)
    {
        this.CreateColormap(alloc, colormapId, window, visual);
        CheckError();
    }

    public void FreeColormapChecked(uint colormapId)
    {
        this.FreeColormap(colormapId);
        CheckError();
    }

    public void CopyColormapAndFreeChecked(uint colormapId, uint srcColormapId)
    {
        this.CopyColormapAndFree(colormapId, srcColormapId);
        CheckError();
    }

    public void InstallColormapChecked(uint colormapId)
    {
        this.InstallColormap(colormapId);
        CheckError();
    }

    public void UninstallColormapChecked(uint colormapId)
    {
        this.UninstallColormap(colormapId);
        CheckError();
    }

    public void FreeColorsChecked(uint colormapId, uint planeMask, params uint[] pixels)
    {
        this.FreeColors(colormapId, planeMask, pixels);
        CheckError();
    }

    public void StoreColorsChecked(uint colormapId, params ColorItem[] item)
    {
        this.StoreColors(colormapId, item);
        CheckError();
    }

    public void StoreNamedColorChecked(ColorFlag mode, uint colormapId, uint pixels, ReadOnlySpan<byte> name)
    {
        this.StoreNamedColor(mode, colormapId, pixels, name);
        CheckError();
    }

    public void CreateCursorChecked(uint cursorId, uint source, uint mask, ushort foreRed, ushort foreGreen,
        ushort foreBlue, ushort backRed, ushort backGreen, ushort backBlue, ushort x, ushort y)
    {
        this.CreateCursor(cursorId, source, mask, foreRed, foreGreen, foreBlue, backRed, backGreen, backBlue, x, y);
        CheckError();
    }

    public void CreateGlyphCursorChecked(uint cursorId, uint sourceFont, uint fontMask, char sourceChar,
        ushort charMask, ushort foreRed, ushort foreGreen, ushort foreBlue, ushort backRed, ushort backGreen,
        ushort backBlue)
    {
        this.CreateGlyphCursor(cursorId, sourceFont, fontMask, sourceChar, charMask, foreRed, foreGreen, foreBlue,
            backRed, backGreen, backBlue);
        CheckError();
    }

    public void FreeCursorChecked(uint cursorId)
    {
        this.FreeCursor(cursorId);
        CheckError();
    }

    public void RecolorCursorChecked(uint cursorId, ushort foreRed, ushort foreGreen, ushort foreBlue, ushort backRed,
        ushort backGreen, ushort backBlue)
    {
        this.RecolorCursor(cursorId, foreRed, foreGreen, foreBlue, backRed, backGreen, backBlue);
        CheckError();
    }

    public void ChangeKeyboardMappingChecked(byte keycodeCount, byte firstKeycode, byte keysymsPerKeycode,
        uint[] Keysym)
    {
        this.ChangeKeyboardMapping(keycodeCount, firstKeycode, keysymsPerKeycode, Keysym);
        CheckError();
    }

    public void BellChecked(sbyte percent)
    {
        this.Bell(percent);
        CheckError();
    }

    public void ChangeKeyboardControlChecked(KeyboardControlMask mask, params uint[] args)
    {
        this.ChangeKeyboardControl(mask, args);
        CheckError();
    }

    public void ChangePointerControlChecked(Acceleration acceleration, ushort? threshold)
    {
        this.ChangePointerControl(acceleration, threshold);
        CheckError();
    }

    public void SetScreenSaverChecked(short timeout, short interval, TriState preferBlanking, TriState allowExposures)
    {
        this.SetScreenSaver(timeout, interval, preferBlanking, allowExposures);
        CheckError();
    }

    public void ForceScreenSaverChecked(ForceScreenSaverMode mode)
    {
        this.ForceScreenSaver(mode);
        CheckError();
    }

    public void ChangeHostsChecked(HostMode mode, Family family, byte[] address)
    {
        this.ChangeHosts(mode, family, address);
        CheckError();
    }

    public void SetAccessControlChecked(AccessControlMode mode)
    {
        this.SetAccessControl(mode);
        CheckError();
    }

    public void SetCloseDownModeChecked(CloseDownMode mode)
    {
        this.SetCloseDownMode(mode);
        CheckError();
    }

    public void KillClientChecked(uint resource)
    {
        this.KillClient(resource);
        CheckError();
    }

    public void NoOperationChecked(params uint[] args)
    {
        this.NoOperation(args);
        CheckError();
    }

    public void PolyText8Checked(uint drawable, uint gc, ushort x, ushort y, Span<byte> data)
    {
        this.PolyText8(drawable, gc, x, y, data);
        CheckError();
    }

    public void PolyText16Checked(uint drawable, uint gc, ushort x, ushort y, Span<byte> data)
    {
        this.PolyText16(drawable, gc, x, y, data);
        CheckError();
    }

}