using System;
using System.Buffers;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Xcsb.Helpers;
using Xcsb.Masks;
using Xcsb.Models;
using Xcsb.Models.Event;
using Xcsb.Models.Handshake;
using Xcsb.Models.Requests;

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

    void IXProto.AllocColor()
    {
        throw new NotImplementedException();
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
        Span<byte> scratchBuffer = stackalloc byte[16];
        scratchBuffer[0] = (byte)Opcode.ChangeActivePointerGrab;
        scratchBuffer[1] = 0;
        MemoryMarshal.Write<short>(scratchBuffer[2..4], 4);
        MemoryMarshal.Write(scratchBuffer[4..8], cursor);
        MemoryMarshal.Write(scratchBuffer[8..12], time);
        MemoryMarshal.Write(scratchBuffer[12..14], mask);
        MemoryMarshal.Write(scratchBuffer[14..16], 0);
        _socket.SendExact(scratchBuffer);
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
        Span<byte> scratchBuffer = stackalloc byte[12];
        scratchBuffer[0] = (byte)Opcode.ChangePointerControl;
        scratchBuffer[1] = 0;
        MemoryMarshal.Write<ushort>(scratchBuffer[2..4], 3);
        MemoryMarshal.Write(scratchBuffer[4..6], acceleration?.Numerator ?? 0);
        MemoryMarshal.Write(scratchBuffer[6..8], acceleration?.Denominator ?? 0);
        MemoryMarshal.Write(scratchBuffer[8..10], threshold ?? 0);
        scratchBuffer[11] = (byte)(acceleration is not null ? 1 : 0);
        scratchBuffer[12] = (byte)(threshold.HasValue ? 1 : 0);
        _socket.SendExact(scratchBuffer);
    }

    void IXProto.ChangeProperty<T>(PropertyMode mode, uint window, uint property, uint type, params T[] args)
    {
        var size = Marshal.SizeOf<T>();
        if (size is not 1 or 2 or 4)
            throw new ArgumentException("type must be byte, sbyte, short, ushort, int, uint");
        var request = new ChangePropertyType(mode, window, property, type, args.Length, (byte)(size * 8));
        var requiredBuffer = 24 + args.Length.AddPadding();

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
        Span<byte> scratchBuffer = stackalloc byte[8];
        scratchBuffer[0] = (byte)Opcode.ChangeSaveSet;
        scratchBuffer[1] = (byte)changeSaveSetMode;
        MemoryMarshal.Write<ushort>(scratchBuffer[2..4], 2);
        MemoryMarshal.Write(scratchBuffer[4..8], window);
        _socket.SendExact(scratchBuffer);
    }

    void IXProto.ChangeWindowAttributes(uint window, ValueMask mask, params uint[] args)
    {
        // todo: optamice
        var request = new ChangeWindowAttributesType(window, mask, args.Length);
        _socket.Send(ref request);
        _socket.SendExact(MemoryMarshal.Cast<uint, byte>(args));
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
        if (requiredBuffer < GlobalSetting.StackAllocThreshold)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
            scratchBuffer[0] = (byte)Opcode.ConfigureWindow;
            scratchBuffer[1] = 0;
            MemoryMarshal.Write(scratchBuffer[2..4], (ushort)(requiredBuffer / 4));
            MemoryMarshal.Write(scratchBuffer[4..8], window);
            MemoryMarshal.Write(scratchBuffer[8..10], mask);
            MemoryMarshal.Write(scratchBuffer[10..12], 0);
            args.AsSpan().CopyTo(MemoryMarshal.Cast<byte, uint>(scratchBuffer[12..requiredBuffer]));
            _socket.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            scratchBuffer[0] = (byte)Opcode.ConfigureWindow;
            scratchBuffer[1] = 0;
            MemoryMarshal.Write(scratchBuffer[2..4], (ushort)(requiredBuffer / 4));
            MemoryMarshal.Write(scratchBuffer[4..8], window);
            MemoryMarshal.Write(scratchBuffer[8..10], mask);
            MemoryMarshal.Write(scratchBuffer[10..12], 0);
            args.AsSpan().CopyTo(MemoryMarshal.Cast<byte, uint>(scratchBuffer[12..requiredBuffer]));
            _socket.SendExact(scratchBuffer[..requiredBuffer]);
        }
    }

    void IXProto.ConvertSelection(uint requestor, uint selection, uint target, uint property, uint timestamp)
    {
        Span<byte> scratchBuffer = stackalloc byte[24];
        scratchBuffer[0] = (byte)Opcode.ConvertSelection;
        scratchBuffer[1] = 0;
        MemoryMarshal.Write<ushort>(scratchBuffer[2..4], 6);
        MemoryMarshal.Write(scratchBuffer[4..8], requestor);
        MemoryMarshal.Write(scratchBuffer[8..12], selection);
        MemoryMarshal.Write(scratchBuffer[12..16], target);
        MemoryMarshal.Write(scratchBuffer[16..20], property);
        MemoryMarshal.Write(scratchBuffer[20..24], timestamp);
        _socket.SendExact(scratchBuffer);
    }

    void IXProto.CopyArea()
    {
        throw new NotImplementedException();
    }

    void IXProto.CopyColormapAndFree(uint colormapId, uint srcColormapId)
    {
        Span<byte> scratchBuffer = stackalloc byte[12];
        scratchBuffer[0] = (byte)Opcode.CopyColormapAndFree;
        scratchBuffer[1] = 0;
        MemoryMarshal.Write<short>(scratchBuffer[2..4], 3);
        MemoryMarshal.Write(scratchBuffer[4..8], colormapId);
        MemoryMarshal.Write(scratchBuffer[8..12], srcColormapId);
        _socket.SendExact(scratchBuffer);
    }

    void IXProto.CopyGC(uint srcGc, uint dstGc, GCMask mask)
    {
        Span<byte> scratchBuffer = stackalloc byte[16];
        scratchBuffer[0] = (byte)Opcode.CopyGC;
        scratchBuffer[1] = 0;
        MemoryMarshal.Write<short>(scratchBuffer[2..4], 4);
        MemoryMarshal.Write(scratchBuffer[4..8], srcGc);
        MemoryMarshal.Write(scratchBuffer[8..12], dstGc);
        MemoryMarshal.Write(scratchBuffer[12..16], mask);
        _socket.SendExact(scratchBuffer);
    }

    void IXProto.CopyPlane()
    {
        throw new NotImplementedException();
    }

    void IXProto.CreateColormap(ColormapAlloc alloc, uint colormapId, uint window, uint visual)
    {
        Span<byte> scratchBuffer = stackalloc byte[16];
        scratchBuffer[0] = (byte)Opcode.CreateColormap;
        scratchBuffer[1] = (byte)alloc;
        MemoryMarshal.Write<short>(scratchBuffer[2..4], 4);
        MemoryMarshal.Write(scratchBuffer[4..8], colormapId);
        MemoryMarshal.Write(scratchBuffer[8..12], window);
        MemoryMarshal.Write(scratchBuffer[12..16], visual);
        _socket.SendExact(scratchBuffer);
    }

    void IXProto.CreateCursor()
    {
        throw new NotImplementedException();
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

    void IXProto.CreateGlyphCursor()
    {
        throw new NotImplementedException();
    }

    void IXProto.CreatePixmap()
    {
        throw new NotImplementedException();
    }

    void IXProto.CreateWindow(uint window,
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
        // todo: optamice
        var request = new CreateWindowType(window,
            parent,
            x, y, width, height,
            borderWidth,
            classType,
            rootVisualId,
            mask,
            args.Length);
        _socket.Send(ref request);
        _socket.SendExact(MemoryMarshal.Cast<uint, byte>(args));
    }

    void IXProto.DeleteProperty(uint window, uint atom)
    {
        Span<byte> scratchBuffer = stackalloc byte[12];
        scratchBuffer[0] = (byte)Opcode.DeleteProperty;
        scratchBuffer[1] = 0;
        MemoryMarshal.Write<ushort>(scratchBuffer[2..4], 3);
        MemoryMarshal.Write(scratchBuffer[4..8], window);
        MemoryMarshal.Write(scratchBuffer[8..12], atom);
        _socket.SendExact(scratchBuffer);
    }

    void IXProto.DestroySubwindows(uint window)
    {
        Span<byte> scratchBuffer = stackalloc byte[8];
        scratchBuffer[0] = (byte)Opcode.DestroySubwindows;
        scratchBuffer[1] = 0;
        MemoryMarshal.Write(scratchBuffer[2..4], (ushort)2);
        MemoryMarshal.Write(scratchBuffer[4..8], window);
        _socket.SendExact(scratchBuffer);
    }

    void IXProto.DestroyWindow(uint window)
    {
        var request = new DestroyWindowType(window);
        _socket.Send(ref request);
    }

    void IXProto.FillPoly()
    {
        throw new NotImplementedException();
    }

    void IXProto.ForceScreenSaver(ForceScreenSaverMode mode)
    {
        Span<byte> scratchBuffer = stackalloc byte[4];
        scratchBuffer[0] = (byte)Opcode.ForceScreenSaver;
        scratchBuffer[1] = (byte)mode;
        MemoryMarshal.Write(scratchBuffer[2..4], 1);
        _socket.SendExact(scratchBuffer);
    }

    void IXProto.FreeColormap(uint colormapId)
    {
        Span<byte> scratchBuffer = stackalloc byte[8];
        scratchBuffer[0] = (byte)Opcode.FreeColormap;
        scratchBuffer[1] = 0;
        MemoryMarshal.Write(scratchBuffer[2..4], 2);
        MemoryMarshal.Write(scratchBuffer[4..8], colormapId);
        _socket.SendExact(scratchBuffer);
    }

    void IXProto.FreeColors(uint colormapId, uint planeMask, params uint[] pixels)
    {
        var requiredBuffer = 12 + pixels.Length * 4;
        if (requiredBuffer < GlobalSetting.StackAllocThreshold)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
            scratchBuffer[0] = (byte)Opcode.FreeColors;
            scratchBuffer[1] = 0;
            MemoryMarshal.Write(scratchBuffer[2..4], (ushort)(requiredBuffer / 4));
            MemoryMarshal.Write(scratchBuffer[4..8], colormapId);
            MemoryMarshal.Write(scratchBuffer[8..12], planeMask);
            pixels.AsSpan().CopyTo(MemoryMarshal.Cast<byte, uint>(scratchBuffer[12..requiredBuffer]));
            _socket.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            scratchBuffer[0] = (byte)Opcode.FreeColors;
            scratchBuffer[1] = 0;
            MemoryMarshal.Write(scratchBuffer[2..4], (ushort)(requiredBuffer / 4));
            MemoryMarshal.Write(scratchBuffer[4..8], colormapId);
            MemoryMarshal.Write(scratchBuffer[8..12], planeMask);
            pixels.AsSpan().CopyTo(MemoryMarshal.Cast<byte, uint>(scratchBuffer[12..requiredBuffer]));
            _socket.SendExact(scratchBuffer[..requiredBuffer]);
        }
    }

    void IXProto.FreeCursor(uint cursorId)
    {
        Span<byte> scratchBuffer = stackalloc byte[8];
        scratchBuffer[0] = (byte)Opcode.FreeCursor;
        scratchBuffer[1] = 0;
        MemoryMarshal.Write<ushort>(scratchBuffer[2..4], 2);
        MemoryMarshal.Write(scratchBuffer[4..8], cursorId);
        _socket.SendExact(scratchBuffer);
    }

    void IXProto.FreeGC(uint gc)
    {
        var request = new FreeGCType(gc);
        _socket.Send(ref request);
    }

    void IXProto.FreePixmap()
    {
        throw new NotImplementedException();
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
            requiredBuffer -= requestSize;

            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            Encoding.ASCII.GetBytes(atomName, scratchBuffer.Slice(atomName.Length));
            scratchBuffer[(atomName.Length)..requiredBuffer].Clear();

            _socket.Send(ref request);
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

    void IXProto.GrabPointer()
    {
        throw new NotImplementedException();
    }

    void IXProto.GrabServer()
    {
        Span<byte> scratchBuffer = stackalloc byte[4];
        scratchBuffer[0] = (byte)Opcode.GrabServer;
        scratchBuffer[1] = 0;
        MemoryMarshal.Write(scratchBuffer[2..4], 1);
        _socket.SendExact(scratchBuffer);
    }

    void IXProto.ImageText16(uint drawable, uint gc, short x, short y, ReadOnlySpan<char> text)
    {
        var requiredBuffer = 16 + (text.Length * 2).AddPadding();
        if (requiredBuffer < GlobalSetting.StackAllocThreshold)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
            scratchBuffer[0] = (byte)Opcode.ImageText16;
            scratchBuffer[1] = (byte)text.Length;
            MemoryMarshal.Write(scratchBuffer[2..4], (ushort)(requiredBuffer / 4));
            MemoryMarshal.Write(scratchBuffer[4..8], drawable);
            MemoryMarshal.Write(scratchBuffer[8..12], gc);
            MemoryMarshal.Write(scratchBuffer[12..14], x);
            MemoryMarshal.Write(scratchBuffer[14..16], y);
            Encoding.BigEndianUnicode.GetBytes(text, scratchBuffer[16..(text.Length * 2 + 16)]);
            scratchBuffer[(16 + text.Length * 2)..requiredBuffer].Clear();
            _socket.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            scratchBuffer[0] = (byte)Opcode.ImageText16;
            scratchBuffer[1] = (byte)text.Length;
            MemoryMarshal.Write(scratchBuffer[2..4], (ushort)(requiredBuffer / 4));
            MemoryMarshal.Write(scratchBuffer[4..8], drawable);
            MemoryMarshal.Write(scratchBuffer[8..12], gc);
            MemoryMarshal.Write(scratchBuffer[12..14], x);
            MemoryMarshal.Write(scratchBuffer[14..16], y);
            Encoding.BigEndianUnicode.GetBytes(text, scratchBuffer[16..(text.Length * 2 + 16)]);
            scratchBuffer[(16 + text.Length * 2)..requiredBuffer].Clear();
            _socket.SendExact(scratchBuffer[..requiredBuffer]);
        }
    }

    void IXProto.ImageText8(uint drawable, uint gc, short x, short y, ReadOnlySpan<byte> text)
    {
        var requiredBuffer = 16 + text.Length.AddPadding();
        if (requiredBuffer < GlobalSetting.StackAllocThreshold)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
            scratchBuffer[0] = (byte)Opcode.ImageText8;
            scratchBuffer[1] = (byte)text.Length;
            MemoryMarshal.Write(scratchBuffer[2..4], (ushort)(requiredBuffer / 4));
            MemoryMarshal.Write(scratchBuffer[4..8], drawable);
            MemoryMarshal.Write(scratchBuffer[8..12], gc);
            MemoryMarshal.Write(scratchBuffer[12..14], x);
            MemoryMarshal.Write(scratchBuffer[14..16], y);
            text.CopyTo(scratchBuffer[16..(text.Length + 16)]);
            scratchBuffer[(16 + text.Length)..requiredBuffer].Clear();
            _socket.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            scratchBuffer[0] = (byte)Opcode.ImageText8;
            scratchBuffer[1] = (byte)text.Length;
            MemoryMarshal.Write(scratchBuffer[2..4], (ushort)(requiredBuffer / 4));
            MemoryMarshal.Write(scratchBuffer[4..8], drawable);
            MemoryMarshal.Write(scratchBuffer[8..12], gc);
            MemoryMarshal.Write(scratchBuffer[12..14], x);
            MemoryMarshal.Write(scratchBuffer[14..16], y);
            text.CopyTo(scratchBuffer[16..(text.Length + 16)]);
            scratchBuffer[(16 + text.Length)..requiredBuffer].Clear();
            _socket.SendExact(scratchBuffer[..requiredBuffer]);
        }
    }

    void IXProto.InstallColormap(uint colormapId)
    {
        Span<byte> scratchBuffer = stackalloc byte[8];
        scratchBuffer[0] = (byte)Opcode.InstallColormap;
        scratchBuffer[1] = 0;
        MemoryMarshal.Write<ushort>(scratchBuffer[2..4], 2);
        MemoryMarshal.Write(scratchBuffer[4..8], colormapId);
        _socket.SendExact(scratchBuffer);
    }

    void IXProto.KillClient(uint resource)
    {
        Span<byte> scratchBuffer = stackalloc byte[8];
        scratchBuffer[0] = (byte)Opcode.KillClient;
        scratchBuffer[1] = 0;
        MemoryMarshal.Write<ushort>(scratchBuffer[2..4], 2);
        MemoryMarshal.Write(scratchBuffer[4..8], resource);
        _socket.SendExact(scratchBuffer);
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
        Span<byte> scratchBuffer = stackalloc byte[8];
        scratchBuffer[0] = (byte)Opcode.MapSubwindows;
        scratchBuffer[1] = 0;
        MemoryMarshal.Write<ushort>(scratchBuffer[2..4], 2);
        MemoryMarshal.Write(scratchBuffer[4..8], window);
        _socket.SendExact(scratchBuffer);
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

    void IXProto.PolyArc()
    {
        throw new NotImplementedException();
    }

    void IXProto.PolyFillArc()
    {
        throw new NotImplementedException();
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

    void IXProto.PolyLine()
    {
        throw new NotImplementedException();
    }

    void IXProto.PolyPoint()
    {
        throw new NotImplementedException();
    }

    void IXProto.PolyRectangle()
    {
        throw new NotImplementedException();
    }

    void IXProto.PolySegment()
    {
        throw new NotImplementedException();
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

    void IXProto.RecolorCursor()
    {
        throw new NotImplementedException();
    }

    void IXProto.ReparentWindow(uint window, uint parent, short x, short y)
    {
        Span<byte> scratchBuffer = stackalloc byte[16];
        scratchBuffer[0] = (byte)Opcode.ReparentWindow;
        scratchBuffer[1] = 0;
        MemoryMarshal.Write<short>(scratchBuffer[2..4], 4);
        MemoryMarshal.Write(scratchBuffer[4..8], window);
        MemoryMarshal.Write(scratchBuffer[8..12], parent);
        MemoryMarshal.Write(scratchBuffer[12..14], x);
        MemoryMarshal.Write(scratchBuffer[14..16], y);
        _socket.SendExact(scratchBuffer);
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
        Span<byte> scratchBuffer = stackalloc byte[44];
        scratchBuffer[0] = (byte)Opcode.SendEvent;
        scratchBuffer[1] = (byte)(propagate ? 1 : 0);
        MemoryMarshal.Write<ushort>(scratchBuffer[2..4], 11);
        MemoryMarshal.Write(scratchBuffer[4..8], destination);
        MemoryMarshal.Write(scratchBuffer[8..12], eventMask);
        MemoryMarshal.Write(scratchBuffer[12..44], evnt);
        _socket.SendExact(scratchBuffer);
    }

    void IXProto.SetAccessControl(AccessControlMode mode)
    {
        Span<byte> scratchBuffer = stackalloc byte[4];
        scratchBuffer[0] = (byte)Opcode.SetAccessControl;
        scratchBuffer[1] = (byte)mode;
        MemoryMarshal.Write(scratchBuffer[2..4], 1);
        _socket.SendExact(scratchBuffer);
    }

    void IXProto.SetClipRectangles()
    {
        throw new NotImplementedException();
    }

    void IXProto.SetCloseDownMode(CloseDownMode mode)
    {
        Span<byte> scratchBuffer = stackalloc byte[4];
        scratchBuffer[0] = (byte)Opcode.SetCloseDownMode;
        scratchBuffer[1] = (byte)mode;
        MemoryMarshal.Write(scratchBuffer[2..4], 1);
        _socket.SendExact(scratchBuffer);
    }

    void IXProto.SetDashes()
    {
        throw new NotImplementedException();
    }

    void IXProto.SetFontPath()
    {
        throw new NotImplementedException();
    }

    void IXProto.SetInputFocus(InputFocusMode mode, uint focus, uint time)
    {
        Span<byte> scratchBuffer = stackalloc byte[12];
        scratchBuffer[0] = (byte)Opcode.SetInputFocus;
        scratchBuffer[1] = (byte)mode;
        MemoryMarshal.Write<ushort>(scratchBuffer[2..4], 3);
        MemoryMarshal.Write(scratchBuffer[4..8], focus);
        MemoryMarshal.Write(scratchBuffer[8..12], time);
        _socket.SendExact(scratchBuffer);
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
        Span<byte> scratchBuffer = stackalloc byte[12];
        scratchBuffer[0] = (byte)Opcode.SetScreenSaver;
        scratchBuffer[1] = 0;
        MemoryMarshal.Write<ushort>(scratchBuffer[2..4], 3);
        MemoryMarshal.Write(scratchBuffer[4..6], timeout);
        MemoryMarshal.Write(scratchBuffer[6..8], interval);
        scratchBuffer[8] = (byte)preferBlanking;
        scratchBuffer[9] = (byte)allowExposures;
        MemoryMarshal.Write<ushort>(scratchBuffer[10..12], 2);
        _socket.SendExact(scratchBuffer);
    }

    void IXProto.SetSelectionOwner(uint owner, uint atom, uint timestamp)
    {
        Span<byte> scratchBuffer = stackalloc byte[16];
        scratchBuffer[0] = (byte)Opcode.SetSelectionOwner;
        scratchBuffer[1] = 0;
        MemoryMarshal.Write<ushort>(scratchBuffer[2..4], 4);
        MemoryMarshal.Write(scratchBuffer[4..8], owner);
        MemoryMarshal.Write(scratchBuffer[8..12], atom);
        MemoryMarshal.Write(scratchBuffer[12..16], timestamp);
        _socket.SendExact(scratchBuffer);
    }

    void IXProto.StoreColors(uint colormapId, params ColorItem[] item)
    {
        var requiredBuffer = 8 + 12 * item.Length;
        if (requiredBuffer < GlobalSetting.StackAllocThreshold)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
            scratchBuffer[0] = (byte)Opcode.StoreColors;
            scratchBuffer[1] = 0;
            MemoryMarshal.Write(scratchBuffer[2..4], (ushort)(requiredBuffer / 4));
            MemoryMarshal.Write(scratchBuffer[4..8], colormapId);
            item.CopyTo(MemoryMarshal.Cast<byte, ColorItem>(scratchBuffer[8..requiredBuffer]));
            scratchBuffer[^1] = 0;
            _socket.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            scratchBuffer[0] = (byte)Opcode.StoreColors;
            scratchBuffer[1] = 0;
            MemoryMarshal.Write(scratchBuffer[2..4], (ushort)(requiredBuffer / 4));
            MemoryMarshal.Write(scratchBuffer[4..8], colormapId);
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
        Span<byte> scratchBuffer = stackalloc byte[8];
        scratchBuffer[0] = (byte)Opcode.UngrabPointer;
        scratchBuffer[1] = 0;
        MemoryMarshal.Write<ushort>(scratchBuffer[2..4], 2);
        MemoryMarshal.Write(scratchBuffer[4..8], time);
        _socket.SendExact(scratchBuffer);
    }

    void IXProto.UngrabServer()
    {
        Span<byte> scratchBuffer = stackalloc byte[4];
        scratchBuffer[0] = (byte)Opcode.UngrabServer;
        scratchBuffer[1] = 0;
        MemoryMarshal.Write(scratchBuffer[2..4], 1);
        _socket.SendExact(scratchBuffer);
    }

    void IXProto.UninstallColormap(uint colormapId)
    {
        Span<byte> scratchBuffer = stackalloc byte[8];
        scratchBuffer[0] = (byte)Opcode.UninstallColormap;
        scratchBuffer[1] = 0;
        MemoryMarshal.Write<ushort>(scratchBuffer[2..4], 2);
        MemoryMarshal.Write(scratchBuffer[4..8], colormapId);
        _socket.SendExact(scratchBuffer);
    }

    void IXProto.UnmapSubwindows(uint window)
    {
        Span<byte> scratchBuffer = stackalloc byte[8];
        scratchBuffer[0] = (byte)Opcode.UnmapSubwindows;
        scratchBuffer[1] = 0;
        MemoryMarshal.Write<ushort>(scratchBuffer[2..4], 2);
        MemoryMarshal.Write(scratchBuffer[4..8], window);
        _socket.SendExact(scratchBuffer);
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