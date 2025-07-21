using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Net.Sockets;
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
using Xcsb.Models.Response.Internals;
#if !NETSTANDARD
using System.Numerics;
#endif

namespace Xcsb;

#if !NETSTANDARD
[SkipLocalsInit]
#endif
internal class XProto : BaseProtoClient, IXProto
{
    private bool _disposedValue;
    private int _globalId;
    private XBufferProto? _xBufferProto;

    public HandshakeSuccessResponseBody HandshakeSuccessResponseBody { get; }

    public IXBufferProto BufferClient => _xBufferProto ??= new XBufferProto(this);

    public XProto(Socket socket, HandshakeSuccessResponseBody connectionResult) : base(socket)
    {
        HandshakeSuccessResponseBody = connectionResult;
        _globalId = 0;
        sequenceNumber = 0;
    }


    public AllocColorReply AllocColor(uint colorMap, ushort red, ushort green, ushort blue)
    {
        var request = new AllocColorType(colorMap, red, green, blue);
        socket.Send(ref request);

        var (result, error) = ReceivedResponse<AllocColorReply>();
        if (error.HasValue || !result.HasValue)
            throw new XEventException(error!.Value);

        sequenceNumber++;
        Debug.Assert(sequenceNumber == result.Value.Sequence);
        return result.Value;
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
        if (percent is > 100 or < -100)
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
            scratchBuffer.WriteRequest(
                ref request,
                12,
                MemoryMarshal.Cast<uint, byte>(args));
            socket.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            var workingBuffer = scratchBuffer[..requiredBuffer];
            workingBuffer.WriteRequest(ref request,
                12,
                MemoryMarshal.Cast<uint, byte>(args));
            socket.SendExact(workingBuffer);
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
            scratchBuffer.WriteRequest(
                ref request,
                8,
                address);
            socket.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            var workingBuffer = scratchBuffer[..requiredBuffer];
            workingBuffer.WriteRequest(
                ref request,
                8,
                address);
            socket.SendExact(workingBuffer);
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
            scratchBuffer.WriteRequest(
                ref request,
                8,
                MemoryMarshal.Cast<uint, byte>(args));
            socket.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            var workingBuffer = scratchBuffer[..requiredBuffer];
            workingBuffer.WriteRequest(
                ref request,
                8,
                MemoryMarshal.Cast<uint, byte>(args));
            socket.SendExact(workingBuffer);
        }

        sequenceNumber++;
    }

    public void ChangeKeyboardMapping(byte keycodeCount, byte firstKeycode, byte keysymsPerKeycode, uint[] keysym)
    {
        var requiredBuffer = 8 + keycodeCount * keysymsPerKeycode * 4;
        var request = new ChangeKeyboardMappingType(keycodeCount, firstKeycode, keysymsPerKeycode);
        if (requiredBuffer < GlobalSetting.StackAllocThreshold)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
            scratchBuffer.WriteRequest(
                ref request,
                8,
                MemoryMarshal.Cast<uint, byte>(keysym));
            socket.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            var workingBuffer = scratchBuffer[..requiredBuffer];
            workingBuffer.WriteRequest(
                ref request,
                8,
                MemoryMarshal.Cast<uint, byte>(keysym));
            socket.SendExact(workingBuffer);
        }

        sequenceNumber++;
    }

    public void ChangePointerControl(Acceleration? acceleration, ushort? threshold)
    {
        var request = new ChangePointerControlType(acceleration?.Numerator ?? 0, acceleration?.Denominator ?? 0,
            threshold ?? 0, (byte)(acceleration is null ? 0 : 1),
            (byte)(threshold.HasValue ? 1 : 0));
        socket.Send(ref request);
        sequenceNumber++;
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
        var requiredBuffer = 24 + size * args.Length.AddPadding();
        if (requiredBuffer < GlobalSetting.StackAllocThreshold)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
            scratchBuffer.WriteRequest(
                ref request,
                24,
                MemoryMarshal.Cast<T, byte>(args));
            socket.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            var workingBuffer = scratchBuffer[..requiredBuffer];
            workingBuffer.WriteRequest(
                ref request,
                24,
                MemoryMarshal.Cast<T, byte>(args));
            socket.SendExact(workingBuffer);
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
            scratchBuffer.WriteRequest(
                ref request,
                12,
                MemoryMarshal.Cast<uint, byte>(args));
            socket.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            var workingBuffer = scratchBuffer[..requiredBuffer];
            workingBuffer.WriteRequest(
                ref request,
                12,
                MemoryMarshal.Cast<uint, byte>(args));
            socket.SendExact(workingBuffer);
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
            scratchBuffer.WriteRequest(
                ref request,
                12,
                MemoryMarshal.Cast<uint, byte>(args));
            socket.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            var workingBuffer = scratchBuffer[..requiredBuffer];
            workingBuffer.WriteRequest(
                ref request,
                12,
                MemoryMarshal.Cast<uint, byte>(args));
            socket.SendExact(workingBuffer);
        }

        sequenceNumber++;
    }

    public void ConvertSelection(uint requestor, uint selection, uint target, uint property, uint timestamp)
    {
        var request = new ConvertSelectionType(requestor, selection, target, property, timestamp);
        socket.Send(ref request);
        sequenceNumber++;
    }

    public void CopyArea(uint srcDrawable, uint destinationDrawable, uint gc, ushort srcX, ushort srcY,
        ushort destinationX, ushort destinationY, ushort width, ushort height)
    {
        var request = new CopyAreaType(srcDrawable, destinationDrawable, gc, srcX, srcY, destinationX, destinationY,
            width, height);
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

    public void CopyPlane(uint srcDrawable, uint destinationDrawable, uint gc, ushort srcX, ushort srcY,
        ushort destinationX, ushort destinationY, ushort width, ushort height, uint bitPlane)
    {
        var request = new CopyPlaneType(srcDrawable, destinationDrawable, gc, srcX, srcY, destinationX, destinationY,
            width, height,
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
        var requiredBuffer = 16 + args.Length * 4;
        if (requiredBuffer < GlobalSetting.StackAllocThreshold)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
            scratchBuffer.WriteRequest(
                ref request,
                16,
                MemoryMarshal.Cast<uint, byte>(args));
            socket.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            var workingBuffer = scratchBuffer[..requiredBuffer];
            workingBuffer.WriteRequest(
                ref request,
                16,
                MemoryMarshal.Cast<uint, byte>(args));
            socket.SendExact(workingBuffer);
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
            scratchBuffer.WriteRequest(
                ref request,
                32,
                MemoryMarshal.Cast<uint, byte>(args));
            socket.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            var workingBuffer = scratchBuffer[..requiredBuffer];
            workingBuffer.WriteRequest(
                ref request,
                32,
                MemoryMarshal.Cast<uint, byte>(args));
            socket.SendExact(workingBuffer);
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
        var requiredBuffer = 16 + points.Length * 4;
        if (requiredBuffer < GlobalSetting.StackAllocThreshold)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
            scratchBuffer.WriteRequest(
                ref request,
                16,
                MemoryMarshal.Cast<Point, byte>(points));
            socket.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            var workingBuffer = scratchBuffer[..requiredBuffer];
            workingBuffer.WriteRequest(
                ref request,
                16,
                MemoryMarshal.Cast<Point, byte>(points));
            socket.SendExact(workingBuffer);
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
            scratchBuffer.WriteRequest(
                ref request,
                12,
                MemoryMarshal.Cast<uint, byte>(pixels));
            socket.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            var workingBuffer = scratchBuffer[..requiredBuffer];
            workingBuffer.WriteRequest(
                ref request,
                12,
                MemoryMarshal.Cast<uint, byte>(pixels));
            socket.SendExact(workingBuffer);
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

    public GetAtomNameReply GetAtomName(uint atom)
    {
        var request = new GetAtomNameType(atom);
        socket.Send(ref request);
        var (result, error) = ReceivedResponse<GetAtomNameResponse>();
        if (error.HasValue || !result.HasValue)
            throw new XEventException(error!.Value);

        sequenceNumber++;
        return new GetAtomNameReply(result.Value, socket);
    }


    public InternAtomReply InternAtom(bool onlyIfExist, string atomName)
    {
        var request = new InternAtomType(onlyIfExist, atomName.Length);
        var requiredBuffer = 8 + atomName.Length.AddPadding();
        if (requiredBuffer < GlobalSetting.StackAllocThreshold)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
#if NETSTANDARD
            MemoryMarshal.Write(scratchBuffer[0..8], ref request);
#else
            MemoryMarshal.Write(scratchBuffer[..8], in request);
#endif
            Encoding.ASCII.GetBytes(atomName, scratchBuffer[8..(atomName.Length + 8)]);
            scratchBuffer[(atomName.Length + 8)..requiredBuffer].Clear();
            socket.SendExact(scratchBuffer);
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
            socket.SendExact(scratchBuffer[..requiredBuffer]);
        }

        var (result, error) = ReceivedResponse<InternAtomReply>();
        if (error.HasValue || !result.HasValue)
            throw new XEventException(error!.Value);

        sequenceNumber++;
        Debug.Assert(sequenceNumber == result.Value.Sequence);
        return result.Value;
    }

    public void GetFontPath()
    {
        throw new NotImplementedException();
        sequenceNumber++;
    }


    public GetGeometryReply GetGeometry(uint drawable)
    {
        var request = new GetGeometryType(drawable);
        socket.Send(ref request);
        var (result, error) = ReceivedResponse<GetGeometryReply>();
        if (error.HasValue || !result.HasValue)
            throw new XEventException(error!.Value);

        sequenceNumber++;
        return result.Value;
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
        var (result, error) = ReceivedResponse<GetPropertyResponse>();
        if (error.HasValue || !result.HasValue)
            throw new XEventException(error!.Value);

        sequenceNumber++;
        return new GetPropertyReply(result.Value, socket);
    }

    public void GetScreenSaver()
    {
        throw new NotImplementedException();
        sequenceNumber++;
    }


    public GetSelectionOwnerReply GetSelectionOwner(uint atom)
    {
        var request = new GetSelectionOwnerType(atom);
        socket.Send(ref request);
        var (result, error) = ReceivedResponse<GetSelectionOwnerReply>();
        if (error.HasValue || !result.HasValue)
            throw new XEventException(error!.Value);

        sequenceNumber++;
        return result.Value;
    }


    public GetWindowAttributesReply GetWindowAttributes(uint window)
    {
        var request = new GetWindowAttributesType(window);
        socket.Send(ref request);
        var (result, error) = ReceivedResponse<GetWindowAttributesReply>();
        if (error.HasValue || !result.HasValue)
            throw new XEventException(error!.Value);

        sequenceNumber++;
        return result.Value;
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
        if (error.HasValue || !result.HasValue)
            throw new XEventException(error!.Value);

        sequenceNumber++;
        Debug.Assert(sequenceNumber == result.Value.Sequence);
        return result.Value;
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

#if NETSTANDARD
            MemoryMarshal.Write(scratchBuffer[0..16], ref request);
#else
            MemoryMarshal.Write(scratchBuffer[..16], in request);
#endif
            Encoding.BigEndianUnicode.GetBytes(text, scratchBuffer[16..(text.Length * 2 + 16)]);
            scratchBuffer[(text.Length * 2 + 16)..requiredBuffer].Clear();
            socket.SendExact(scratchBuffer);
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
            scratchBuffer.WriteRequest(
                ref request,
                16,
                text
            );
            socket.SendExact(scratchBuffer);
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
            socket.SendExact(workingBuffer);
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


    public ListPropertiesReply ListProperties(uint window)
    {
        var request = new ListPropertiesType(window);
        socket.Send(ref request);
        var (result, error) = ReceivedResponse<ListPropertiesResponse>();
        if (error.HasValue || !result.HasValue)
            throw new XEventException(error!.Value);

        sequenceNumber++;
        return new ListPropertiesReply(result.Value, socket);
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
            scratchBuffer.WriteRequest(
                ref request,
                4,
                MemoryMarshal.Cast<uint, byte>(args));
            socket.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            var workingBuffer = scratchBuffer[..requiredBuffer];
            workingBuffer.WriteRequest(
                ref request,
                4,
                MemoryMarshal.Cast<uint, byte>(args));
            socket.SendExact(workingBuffer);
        }

        sequenceNumber++;
    }

    public void OpenFont(string fontName, uint fontId)
    {
        var request = new OpenFontType(fontId, (ushort)fontName.Length);
        var requiredBuffer = 12 + fontName.Length.AddPadding();
        if (requiredBuffer < GlobalSetting.StackAllocThreshold)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
#if NETSTANDARD
            MemoryMarshal.Write(scratchBuffer[0..12], ref request);
#else
            MemoryMarshal.Write(scratchBuffer[..12], in request);
#endif
            Encoding.ASCII.GetBytes(fontName, scratchBuffer[12..(fontName.Length + 12)]);
            scratchBuffer[(fontName.Length + 12)..requiredBuffer].Clear();
            socket.SendExact(scratchBuffer);
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

            socket.SendExact(scratchBuffer[..requiredBuffer]);
        }

        sequenceNumber++;
    }

    public void PolyArc(uint drawable, uint gc, Arc[] arcs)
    {
        var request = new PolyArcType(drawable, gc, arcs.Length);
        var requiredBuffer = 12 + arcs.Length * 12;
        if (requiredBuffer < GlobalSetting.StackAllocThreshold)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
            scratchBuffer.WriteRequest(
                ref request,
                12,
                MemoryMarshal.Cast<Arc, byte>(arcs));
            socket.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            var workingBuffer = scratchBuffer[..requiredBuffer];
            workingBuffer.WriteRequest(
                ref request,
                12,
                MemoryMarshal.Cast<Arc, byte>(arcs));
            socket.SendExact(workingBuffer);
        }

        sequenceNumber++;
    }

    public void PolyFillArc(uint drawable, uint gc, Arc[] arcs)
    {
        var request = new PolyFillArcType(drawable, gc, arcs.Length);
        var requiredBuffer = Marshal.SizeOf<PolyFillArcType>() + arcs.Length * 12;
        if (requiredBuffer < GlobalSetting.StackAllocThreshold)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
            scratchBuffer.WriteRequest(
                ref request,
                12,
                MemoryMarshal.Cast<Arc, byte>(arcs));
            socket.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            var workingBuffer = scratchBuffer[..requiredBuffer];
            workingBuffer.WriteRequest(
                ref request,
                12,
                MemoryMarshal.Cast<Arc, byte>(arcs));
            socket.SendExact(workingBuffer);
        }

        sequenceNumber++;
    }

    public void PolyFillRectangle(uint drawable, uint gc, Rectangle[] rectangles)
    {
        var request = new PolyFillRectangleType(drawable, gc, rectangles.Length);
        var requiredBuffer = Marshal.SizeOf<PolyFillRectangleType>() + rectangles.Length * 8;
        if (requiredBuffer < GlobalSetting.StackAllocThreshold)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
            scratchBuffer.WriteRequest(
                ref request,
                12,
                MemoryMarshal.Cast<Rectangle, byte>(rectangles));
            socket.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            var workingBuffer = scratchBuffer[..requiredBuffer];
            workingBuffer.WriteRequest(
                ref request,
                12,
                MemoryMarshal.Cast<Rectangle, byte>(rectangles));
            socket.SendExact(workingBuffer);
        }

        sequenceNumber++;
    }

    public void PolyLine(CoordinateMode coordinate, uint drawable, uint gc, Point[] points)
    {
        var request = new PolyLineType(coordinate, drawable, gc, points.Length);
        var requiredBuffer = 12 + points.Length * 4;
        if (requiredBuffer < GlobalSetting.StackAllocThreshold)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
            scratchBuffer.WriteRequest(
                ref request,
                12,
                MemoryMarshal.Cast<Point, byte>(points));
            socket.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            var workingBuffer = scratchBuffer[..requiredBuffer];
            workingBuffer.WriteRequest(
                ref request,
                12,
                MemoryMarshal.Cast<Point, byte>(points));
            socket.SendExact(workingBuffer);
        }

        sequenceNumber++;
    }

    public void PolyPoint(CoordinateMode coordinate, uint drawable, uint gc, Point[] points)
    {
        var request = new PolyPointType(coordinate, drawable, gc, points.Length);
        var requiredBuffer = 12 + points.Length * 4;
        if (requiredBuffer < GlobalSetting.StackAllocThreshold)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
            scratchBuffer.WriteRequest(
                ref request,
                12,
                MemoryMarshal.Cast<Point, byte>(points));
            socket.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            var workingBuffer = scratchBuffer[..requiredBuffer];
            workingBuffer.WriteRequest(
                ref request,
                12,
                MemoryMarshal.Cast<Point, byte>(points));
            socket.SendExact(workingBuffer);
        }

        sequenceNumber++;
    }

    public void PolyRectangle(uint drawable, uint gc, Rectangle[] rectangles)
    {
        var request = new PolyRectangleType(drawable, gc, rectangles.Length);
        var requiredBuffer = 12 + rectangles.Length * 8;
        if (requiredBuffer < GlobalSetting.StackAllocThreshold)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
            scratchBuffer.WriteRequest(
                ref request,
                12,
                MemoryMarshal.Cast<Rectangle, byte>(rectangles));
            socket.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            var workingBuffer = scratchBuffer[..requiredBuffer];
            workingBuffer.WriteRequest(
                ref request,
                12,
                MemoryMarshal.Cast<Rectangle, byte>(rectangles));
            socket.SendExact(workingBuffer);
        }

        sequenceNumber++;
    }

    public void PolySegment(uint drawable, uint gc, Segment[] segments)
    {
        var request = new PolySegmentType(drawable, gc, segments.Length);
        var requiredBuffer = 12 + segments.Length * 8;
        if (requiredBuffer < GlobalSetting.StackAllocThreshold)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
            scratchBuffer.WriteRequest(
                ref request,
                12,
                MemoryMarshal.Cast<Segment, byte>(segments));
            socket.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            var workingBuffer = scratchBuffer[..requiredBuffer];
            workingBuffer.WriteRequest(
                ref request,
                12,
                MemoryMarshal.Cast<Segment, byte>(segments));
            socket.SendExact(workingBuffer);
        }

        sequenceNumber++;
    }

    public void PolyText16(uint drawable, uint gc, ushort x, ushort y, Span<byte> data)
    {
        var request = new PolyText16Type(drawable, gc, x, y, data.Length);
        var requiredBuffer = 16 + data.Length.AddPadding();
        if (requiredBuffer < GlobalSetting.StackAllocThreshold)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
            scratchBuffer.WriteRequest(
                ref request,
                16,
                data);
            socket.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            var workingBuffer = scratchBuffer[..requiredBuffer];
            workingBuffer.WriteRequest(
                ref request,
                16,
                data);
            socket.SendExact(workingBuffer);
        }

        sequenceNumber++;
    }


    public void PolyText8(uint drawable, uint gc, ushort x, ushort y, Span<byte> data)
    {
        var request = new PolyText8Type(drawable, gc, x, y, data.Length);
        var requiredBuffer = 16 + data.Length.AddPadding();
        if (requiredBuffer < GlobalSetting.StackAllocThreshold)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
            scratchBuffer.WriteRequest(
                ref request,
                16,
                data);
            socket.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            var workingBuffer = scratchBuffer[..requiredBuffer];
            workingBuffer.WriteRequest(
                ref request,
                16,
                data);
            socket.SendExact(workingBuffer);
        }

        sequenceNumber++;
    }


    public void PutImage(ImageFormat format, uint drawable, uint gc, ushort width, ushort height, short x, short y,
        byte leftPad, byte depth, Span<byte> data)
    {
        var request = new PutImageType(format, drawable, gc, width, height, x, y, leftPad, depth, data.Length);
        var requiredBuffer = data.Length.AddPadding() + 24;
        if (requiredBuffer < GlobalSetting.StackAllocThreshold)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
            scratchBuffer.WriteRequest(
                ref request,
                24,
                data);
            socket.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            var workingBuffer = scratchBuffer[..requiredBuffer];
            workingBuffer.WriteRequest(
                ref request,
                24,
                data);
            socket.SendExact(workingBuffer);
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
        if (error.HasValue || !result.HasValue)
            throw new XEventException(error!.Value);

        sequenceNumber++;
        Debug.Assert(sequenceNumber == result.Value.Sequence);
        return result.Value;
    }

    public void QueryTextExtents()
    {
        throw new NotImplementedException();
        sequenceNumber++;
    }


    public QueryTreeReply QueryTree(uint window)
    {
        var request = new QueryTreeType(window);
        socket.Send(ref request);
        var (result, error) = ReceivedResponse<QueryTreeResponse>();
        if (error.HasValue || !result.HasValue)
            throw new XEventException(error!.Value);

        sequenceNumber++;
        return new QueryTreeReply(result.Value, socket);
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
            scratchBuffer.WriteRequest(
                ref request,
                12,
                MemoryMarshal.Cast<uint, byte>(properties));
            socket.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            var workingBuffer = scratchBuffer[..requiredBuffer];
            workingBuffer.WriteRequest(
                ref request,
                12,
                MemoryMarshal.Cast<uint, byte>(properties));
            socket.SendExact(workingBuffer);
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
            scratchBuffer.WriteRequest(
                ref request,
                12,
                MemoryMarshal.Cast<Rectangle, byte>(rectangles));
            socket.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            var workingBuffer = scratchBuffer[..requiredBuffer];
            workingBuffer.WriteRequest(
                ref request,
                12,
                MemoryMarshal.Cast<Rectangle, byte>(rectangles));
            socket.SendExact(workingBuffer);
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
            scratchBuffer.WriteRequest(
                ref request,
                12,
                dashes);
            socket.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            var workingBuffer = scratchBuffer[..requiredBuffer];
            workingBuffer.WriteRequest(
                ref request,
                12,
                dashes);
            socket.SendExact(workingBuffer);
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
#if NETSTANDARD
            MemoryMarshal.Write(scratchBuffer[0..8], ref request);
#else
            MemoryMarshal.Write(scratchBuffer[..8], in request);
#endif
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
#if NETSTANDARD
            MemoryMarshal.Write(scratchBuffer[0..8], ref request);
#else
            MemoryMarshal.Write(scratchBuffer[..8], in request);
#endif
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
            scratchBuffer.WriteRequest(
                ref request,
                8,
                MemoryMarshal.Cast<ColorItem, byte>(item));
            scratchBuffer[requiredBuffer - 1] = 0;
            socket.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            var workingBuffer = scratchBuffer[..requiredBuffer];
            workingBuffer.WriteRequest(
                ref request,
                8,
                MemoryMarshal.Cast<ColorItem, byte>(item));
            scratchBuffer[requiredBuffer - 1] = 0;
            socket.SendExact(workingBuffer);
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
            scratchBuffer.WriteRequest(
                ref request,
                16,
                name);
            socket.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            var workingBuffer = scratchBuffer[..requiredBuffer];
            workingBuffer.WriteRequest(
                ref request,
                16,
                name);
            socket.SendExact(workingBuffer);
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

    public void WarpPointer(uint srcWindow, uint destinationWindow, short srcX, short srcY, ushort srcWidth,
        ushort srcHeight, short destinationX, short destinationY)
    {
        var request = new WarpPointerType(srcWindow, destinationWindow, srcX, srcY, srcWidth, srcHeight, destinationX,
            destinationY);
        socket.Send(ref request);
        sequenceNumber++;
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public void WaitForEvent()
    {
        if (!IsEventAvailable())
            socket.Poll(-1, SelectMode.SelectRead);
    }

    public uint NewId()
    {
        return (uint)((HandshakeSuccessResponseBody.ResourceIDMask & _globalId++) |
                      HandshakeSuccessResponseBody.ResourceIDBase);
    }

    public XEvent? GetEvent()
    {
        if (bufferEvents.TryPop(out var result))
            return result;
        Span<byte> scratchBuffer = stackalloc byte[Marshal.SizeOf<XEvent>()];

        if (socket.Poll(-1, SelectMode.SelectRead))
        {
            var totalRead = socket.Receive(scratchBuffer);
            if (totalRead == 0)
                return null;
        }

        return scratchBuffer.ToStruct<XEvent>();
    }

    public bool IsEventAvailable() =>
        bufferEvents.Any() || socket.Available >= Unsafe.SizeOf<XEvent>();

    public void CreateWindowChecked(byte depth, uint window, uint parent, short x, short y, ushort width, ushort height,
        ushort borderWidth, ClassType classType, uint rootVisualId, ValueMask mask, params uint[] args)
    {
        CreateWindow(depth, window, parent, x, y, width, height, borderWidth, classType, rootVisualId, mask, args);
        CheckError();
    }

    public void ChangeWindowAttributesChecked(uint window, ValueMask mask, params uint[] args)
    {
        ChangeWindowAttributes(window, mask, args);
        CheckError();
    }

    public void DestroyWindowChecked(uint window)
    {
        DestroyWindow(window);
        CheckError();
    }

    public void DestroySubwindowsChecked(uint window)
    {
        DestroySubwindows(window);
        CheckError();
    }

    public void ChangeSaveSetChecked(ChangeSaveSetMode changeSaveSetMode, uint window)
    {
        ChangeSaveSet(changeSaveSetMode, window);
        CheckError();
    }

    public void ReparentWindowChecked(uint window, uint parent, short x, short y)
    {
        ReparentWindow(window, parent, x, y);
        CheckError();
    }

    public void MapWindowChecked(uint window)
    {
        MapWindow(window);
        CheckError();
    }

    public void MapSubwindowsChecked(uint window)
    {
        MapSubwindows(window);
        CheckError();
    }

    public void UnmapWindowChecked(uint window)
    {
        UnmapWindow(window);
        CheckError();
    }

    public void UnmapSubwindowsChecked(uint window)
    {
        UnmapSubwindows(window);
        CheckError();
    }

    public void ConfigureWindowChecked(uint window, ConfigureValueMask mask, params uint[] args)
    {
        ConfigureWindow(window, mask, args);
        CheckError();
    }

    public void CirculateWindowChecked(Direction direction, uint window)
    {
        CirculateWindow(direction, window);
        CheckError();
    }

    public void ChangePropertyChecked<T>(PropertyMode mode, uint window, uint property, uint type, params T[] args)
        where T : struct
#if !NETSTANDARD
        , INumber<T>
#endif
    {
        ChangeProperty(mode, window, property, type, args);
        CheckError();
    }

    public void DeletePropertyChecked(uint window, uint atom)
    {
        DeleteProperty(window, atom);
        CheckError();
    }

    public void RotatePropertiesChecked(uint window, ushort delta, params uint[] properties)
    {
        RotateProperties(window, delta, properties);
        CheckError();
    }

    public void SetSelectionOwnerChecked(uint owner, uint atom, uint timestamp)
    {
        SetSelectionOwner(owner, atom, timestamp);
        CheckError();
    }

    public void ConvertSelectionChecked(uint requestor, uint selection, uint target, uint property, uint timestamp)
    {
        ConvertSelection(requestor, selection, target, property, timestamp);
        CheckError();
    }

    public void SendEventChecked(bool propagate, uint destination, uint eventMask, XEvent evnt)
    {
        SendEvent(propagate, destination, eventMask, evnt);
        CheckError();
    }

    public void UngrabPointerChecked(uint time)
    {
        UngrabPointer(time);
        CheckError();
    }

    public void GrabButtonChecked(bool ownerEvents, uint grabWindow, ushort mask, GrabMode pointerMode,
        GrabMode keyboardMode, uint confineTo, uint cursor, Button button, ModifierMask modifiers)
    {
        GrabButton(ownerEvents, grabWindow, mask, pointerMode, keyboardMode, confineTo, cursor, button, modifiers);
        CheckError();
    }

    public void UngrabButtonChecked(Button button, uint grabWindow, ModifierMask mask)
    {
        UngrabButton(button, grabWindow, mask);
        CheckError();
    }

    public void ChangeActivePointerGrabChecked(uint cursor, uint time, ushort mask)
    {
        ChangeActivePointerGrab(cursor, time, mask);
        CheckError();
    }

    public void UngrabKeyboardChecked(uint time)
    {
        UngrabKeyboard(time);
        CheckError();
    }

    public void GrabKeyChecked(bool exposures, uint grabWindow, ModifierMask mask, byte keycode, GrabMode pointerMode,
        GrabMode keyboardMode)
    {
        GrabKey(exposures, grabWindow, mask, keycode, pointerMode, keyboardMode);
        CheckError();
    }

    public void UngrabKeyChecked(byte key, uint grabWindow, ModifierMask modifier)
    {
        UngrabKey(key, grabWindow, modifier);
        CheckError();
    }

    public void AllowEventsChecked(EventsMode mode, uint time)
    {
        AllowEvents(mode, time);
        CheckError();
    }

    public void GrabServerChecked()
    {
        GrabServer();
        CheckError();
    }

    public void UngrabServerChecked()
    {
        UngrabServer();
        CheckError();
    }

    public void WarpPointerChecked(uint srcWindow, uint destinationWindow, short srcX, short srcY, ushort srcWidth,
        ushort srcHeight, short destinationX, short destinationY)
    {
        WarpPointer(srcWindow, destinationWindow, srcX, srcY, srcWidth, srcHeight, destinationX, destinationY);
        CheckError();
    }

    public void SetInputFocusChecked(InputFocusMode mode, uint focus, uint time)
    {
        SetInputFocus(mode, focus, time);
        CheckError();
    }

    public void OpenFontChecked(string fontName, uint fontId)
    {
        OpenFont(fontName, fontId);
        CheckError();
    }

    public void CloseFontChecked(uint fontId)
    {
        CloseFont(fontId);
        CheckError();
    }

    public void SetFontPathChecked(string[] strPaths)
    {
        SetFontPath(strPaths);
        CheckError();
    }

    public void CreatePixmapChecked(byte depth, uint pixmapId, uint drawable, ushort width, ushort height)
    {
        CreatePixmap(depth, pixmapId, drawable, width, height);
        CheckError();
    }

    public void FreePixmapChecked(uint pixmapId)
    {
        FreePixmap(pixmapId);
        CheckError();
    }

    public void CreateGCChecked(uint gc, uint drawable, GCMask mask, params uint[] args)
    {
        CreateGC(gc, drawable, mask, args);
        CheckError();
    }

    public void ChangeGCChecked(uint gc, GCMask mask, params uint[] args)
    {
        ChangeGC(gc, mask, args);
        CheckError();
    }

    public void CopyGCChecked(uint srcGc, uint dstGc, GCMask mask)
    {
        CopyGC(srcGc, dstGc, mask);
        CheckError();
    }

    public void SetDashesChecked(uint gc, ushort dashOffset, byte[] dashes)
    {
        SetDashes(gc, dashOffset, dashes);
        CheckError();
    }

    public void SetClipRectanglesChecked(ClipOrdering ordering, uint gc, ushort clipX, ushort clipY,
        Rectangle[] rectangles)
    {
        SetClipRectangles(ordering, gc, clipX, clipY, rectangles);
        CheckError();
    }

    public void FreeGCChecked(uint gc)
    {
        FreeGC(gc);
        CheckError();
    }

    public void ClearAreaChecked(bool exposures, uint window, short x, short y, ushort width, ushort height)
    {
        ClearArea(exposures, window, x, y, width, height);
        CheckError();
    }

    public void CopyAreaChecked(uint srcDrawable, uint destinationDrawable, uint gc, ushort srcX, ushort srcY,
        ushort destinationX, ushort destinationY, ushort width, ushort height)
    {
        CopyArea(srcDrawable, destinationDrawable, gc, srcX, srcY, destinationX, destinationY, width, height);
        CheckError();
    }

    public void CopyPlaneChecked(uint srcDrawable, uint destinationDrawable, uint gc, ushort srcX, ushort srcY,
        ushort destinationX, ushort destinationY, ushort width, ushort height, uint bitPlane)
    {
        CopyPlane(srcDrawable, destinationDrawable, gc, srcX, srcY, destinationX, destinationY, width, height,
            bitPlane);
        CheckError();
    }

    public void PolyPointChecked(CoordinateMode coordinate, uint drawable, uint gc, Point[] points)
    {
        PolyPoint(coordinate, drawable, gc, points);
        CheckError();
    }

    public void PolyLineChecked(CoordinateMode coordinate, uint drawable, uint gc, Point[] points)
    {
        PolyLine(coordinate, drawable, gc, points);
        CheckError();
    }

    public void PolySegmentChecked(uint drawable, uint gc, Segment[] segments)
    {
        PolySegment(drawable, gc, segments);
        CheckError();
    }

    public void PolyRectangleChecked(uint drawable, uint gc, Rectangle[] rectangles)
    {
        PolyRectangle(drawable, gc, rectangles);
        CheckError();
    }

    public void PolyArcChecked(uint drawable, uint gc, Arc[] arcs)
    {
        PolyArc(drawable, gc, arcs);
        CheckError();
    }

    public void FillPolyChecked(uint drawable, uint gc, PolyShape shape, CoordinateMode coordinate, Point[] points)
    {
        FillPoly(drawable, gc, shape, coordinate, points);
        CheckError();
    }

    public void PolyFillRectangleChecked(uint drawable, uint gc, Rectangle[] rectangles)
    {
        PolyFillRectangle(drawable, gc, rectangles);
        CheckError();
    }

    public void PolyFillArcChecked(uint drawable, uint gc, Arc[] arcs)
    {
        PolyFillArc(drawable, gc, arcs);
        CheckError();
    }

    public void PutImageChecked(ImageFormat format, uint drawable, uint gc, ushort width, ushort height, short x,
        short y, byte leftPad, byte depth, Span<byte> data)
    {
        PutImage(format, drawable, gc, width, height, x, y, leftPad, depth, data);
        CheckError();
    }

    public void ImageText8Checked(uint drawable, uint gc, short x, short y, ReadOnlySpan<byte> text)
    {
        ImageText8(drawable, gc, x, y, text);
        CheckError();
    }

    public void ImageText16Checked(uint drawable, uint gc, short x, short y, ReadOnlySpan<char> text)
    {
        ImageText16(drawable, gc, x, y, text);
        CheckError();
    }

    public void CreateColormapChecked(ColormapAlloc alloc, uint colormapId, uint window, uint visual)
    {
        CreateColormap(alloc, colormapId, window, visual);
        CheckError();
    }

    public void FreeColormapChecked(uint colormapId)
    {
        FreeColormap(colormapId);
        CheckError();
    }

    public void CopyColormapAndFreeChecked(uint colormapId, uint srcColormapId)
    {
        CopyColormapAndFree(colormapId, srcColormapId);
        CheckError();
    }

    public void InstallColormapChecked(uint colormapId)
    {
        InstallColormap(colormapId);
        CheckError();
    }

    public void UninstallColormapChecked(uint colormapId)
    {
        UninstallColormap(colormapId);
        CheckError();
    }

    public void FreeColorsChecked(uint colormapId, uint planeMask, params uint[] pixels)
    {
        FreeColors(colormapId, planeMask, pixels);
        CheckError();
    }

    public void StoreColorsChecked(uint colormapId, params ColorItem[] item)
    {
        StoreColors(colormapId, item);
        CheckError();
    }

    public void StoreNamedColorChecked(ColorFlag mode, uint colormapId, uint pixels, ReadOnlySpan<byte> name)
    {
        StoreNamedColor(mode, colormapId, pixels, name);
        CheckError();
    }

    public void CreateCursorChecked(uint cursorId, uint source, uint mask, ushort foreRed, ushort foreGreen,
        ushort foreBlue, ushort backRed, ushort backGreen, ushort backBlue, ushort x, ushort y)
    {
        CreateCursor(cursorId, source, mask, foreRed, foreGreen, foreBlue, backRed, backGreen, backBlue, x, y);
        CheckError();
    }

    public void CreateGlyphCursorChecked(uint cursorId, uint sourceFont, uint fontMask, char sourceChar,
        ushort charMask, ushort foreRed, ushort foreGreen, ushort foreBlue, ushort backRed, ushort backGreen,
        ushort backBlue)
    {
        CreateGlyphCursor(cursorId, sourceFont, fontMask, sourceChar, charMask, foreRed, foreGreen, foreBlue,
            backRed, backGreen, backBlue);
        CheckError();
    }

    public void FreeCursorChecked(uint cursorId)
    {
        FreeCursor(cursorId);
        CheckError();
    }

    public void RecolorCursorChecked(uint cursorId, ushort foreRed, ushort foreGreen, ushort foreBlue, ushort backRed,
        ushort backGreen, ushort backBlue)
    {
        RecolorCursor(cursorId, foreRed, foreGreen, foreBlue, backRed, backGreen, backBlue);
        CheckError();
    }

    public void ChangeKeyboardMappingChecked(byte keycodeCount, byte firstKeycode, byte keysymsPerKeycode,
        uint[] keysym)
    {
        ChangeKeyboardMapping(keycodeCount, firstKeycode, keysymsPerKeycode, keysym);
        CheckError();
    }

    public void BellChecked(sbyte percent)
    {
        Bell(percent);
        CheckError();
    }

    public void ChangeKeyboardControlChecked(KeyboardControlMask mask, params uint[] args)
    {
        ChangeKeyboardControl(mask, args);
        CheckError();
    }

    public void ChangePointerControlChecked(Acceleration acceleration, ushort? threshold)
    {
        ChangePointerControl(acceleration, threshold);
        CheckError();
    }

    public void SetScreenSaverChecked(short timeout, short interval, TriState preferBlanking, TriState allowExposures)
    {
        SetScreenSaver(timeout, interval, preferBlanking, allowExposures);
        CheckError();
    }

    public void ForceScreenSaverChecked(ForceScreenSaverMode mode)
    {
        ForceScreenSaver(mode);
        CheckError();
    }

    public void ChangeHostsChecked(HostMode mode, Family family, byte[] address)
    {
        ChangeHosts(mode, family, address);
        CheckError();
    }

    public void SetAccessControlChecked(AccessControlMode mode)
    {
        SetAccessControl(mode);
        CheckError();
    }

    public void SetCloseDownModeChecked(CloseDownMode mode)
    {
        SetCloseDownMode(mode);
        CheckError();
    }

    public void KillClientChecked(uint resource)
    {
        KillClient(resource);
        CheckError();
    }

    public void NoOperationChecked(params uint[] args)
    {
        NoOperation(args);
        CheckError();
    }

    public void PolyText8Checked(uint drawable, uint gc, ushort x, ushort y, Span<byte> data)
    {
        PolyText8(drawable, gc, x, y, data);
        CheckError();
    }

    public void PolyText16Checked(uint drawable, uint gc, ushort x, ushort y, Span<byte> data)
    {
        PolyText16(drawable, gc, x, y, data);
        CheckError();
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposedValue) return;
        if (disposing && socket.Connected)
            socket.Close();
        _disposedValue = true;
    }

    private void CheckError([CallerMemberName] string name = "")
    {
        var error = Received();
        if (error.HasValue) throw new XEventException(error.Value, name);
    }
}