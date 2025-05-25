using Src.Helpers;
using Src.Models;
using Src.Models.Event;
using Src.Models.Handshake;
using System.Buffers;
using System.Diagnostics;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Src;

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

    void IXProto.AllowEvents()
    {
        throw new NotImplementedException();
    }

    void IXProto.Bell(byte percent)
    {
        Span<byte> scratchBuffer = stackalloc byte[4];
        scratchBuffer[0] = (byte)Opcode.Bell;
        scratchBuffer[1] = percent;
        MemoryMarshal.Write(scratchBuffer[2..4], 1);
        _socket.SendExact(scratchBuffer);
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
        var requiredBuffer = 12 + args.Length * 4;
        if (requiredBuffer < GlobalSetting.StackAllocThreshold)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
            scratchBuffer[0] = (byte)Opcode.CreateGC;
            scratchBuffer[1] = 0;
            MemoryMarshal.Write(scratchBuffer[2..4], (ushort)(requiredBuffer / 4));
            MemoryMarshal.Write(scratchBuffer[4..8], gc);
            MemoryMarshal.Write(scratchBuffer[8..12], mask);
            args.AsSpan().CopyTo(MemoryMarshal.Cast<byte, uint>(scratchBuffer[12..requiredBuffer]));
            _socket.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            scratchBuffer[0] = (byte)Opcode.CreateGC;
            scratchBuffer[1] = 0;
            MemoryMarshal.Write(scratchBuffer[2..4], (ushort)(requiredBuffer / 4));
            MemoryMarshal.Write(scratchBuffer[4..8], gc);
            MemoryMarshal.Write(scratchBuffer[8..12], mask);
            args.AsSpan().CopyTo(MemoryMarshal.Cast<byte, uint>(scratchBuffer[12..requiredBuffer]));
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

        var requiredBuffer = 24 + args.Length.AddPadding();
        if (requiredBuffer < GlobalSetting.StackAllocThreshold)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
            scratchBuffer[0] = (byte)Opcode.ChangeProperty;
            scratchBuffer[1] = (byte)mode;
            MemoryMarshal.Write<ushort>(scratchBuffer[2..4], (ushort)(requiredBuffer /4));
            MemoryMarshal.Write(scratchBuffer[4..8], window);
            MemoryMarshal.Write(scratchBuffer[8..12], property);
            MemoryMarshal.Write(scratchBuffer[12..16], type);
            scratchBuffer[17] = (byte)size;
            scratchBuffer[18] = 0;
            scratchBuffer[19] = 0;
            scratchBuffer[20] = 0;
            MemoryMarshal.Write(scratchBuffer[21..24], args.Length);
            args.AsSpan().CopyTo(MemoryMarshal.Cast<byte, T>(scratchBuffer[24..requiredBuffer]));
            _socket.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            scratchBuffer[0] = (byte)Opcode.ChangeProperty;
            scratchBuffer[1] = (byte)mode;
            MemoryMarshal.Write<ushort>(scratchBuffer[2..4], (ushort)(requiredBuffer /4));
            MemoryMarshal.Write(scratchBuffer[4..8], window);
            MemoryMarshal.Write(scratchBuffer[8..12], property);
            MemoryMarshal.Write(scratchBuffer[12..16], type);
            scratchBuffer[17] = (byte)size;
            scratchBuffer[18] = 0;
            scratchBuffer[19] = 0;
            scratchBuffer[20] = 0;
            MemoryMarshal.Write(scratchBuffer[21..24], args.Length);
            args.AsSpan().CopyTo(MemoryMarshal.Cast<byte, T>(scratchBuffer[24..requiredBuffer]));
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

    void IXProto.ChangeWindowAttributes(uint window,
        ValueMask mask,
        params uint[] args)
    {
        var requiredBuffer = 12 + args.Length * 4;
        if (requiredBuffer < GlobalSetting.StackAllocThreshold)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
            scratchBuffer[0] = (byte)Opcode.ChangeWindowAttributes;
            scratchBuffer[1] = 0;
            MemoryMarshal.Write(scratchBuffer[2..4], (ushort)(requiredBuffer / 4));
            MemoryMarshal.Write(scratchBuffer[4..8], window);
            MemoryMarshal.Write(scratchBuffer[8..12], mask);
            args.AsSpan().CopyTo(MemoryMarshal.Cast<byte, uint>(scratchBuffer[12..requiredBuffer]));
            _socket.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            scratchBuffer[0] = (byte)Opcode.ChangeWindowAttributes;
            scratchBuffer[1] = 0;
            MemoryMarshal.Write(scratchBuffer[2..4], (ushort)(requiredBuffer / 4));
            MemoryMarshal.Write(scratchBuffer[4..8], window);
            MemoryMarshal.Write(scratchBuffer[8..12], mask);
            args.AsSpan().CopyTo(MemoryMarshal.Cast<byte, uint>(scratchBuffer[12..requiredBuffer]));
            _socket.SendExact(scratchBuffer[..requiredBuffer]);
        }
    }

    void IXProto.CirculateWindow(Direction direction, uint window)
    {
        Span<byte> scratchBuffer = stackalloc byte[8];
        scratchBuffer[0] = (byte)Opcode.CirculateWindow;
        scratchBuffer[1] = (byte)direction;
        MemoryMarshal.Write<ushort>(scratchBuffer[2..4], 2);
        MemoryMarshal.Write(scratchBuffer[4..8], window);
        _socket.SendExact(scratchBuffer);
    }

    void IXProto.ClearArea()
    {
        throw new NotImplementedException();
    }

    void IXProto.CloseFont()
    {
        throw new NotImplementedException();
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

    void IXProto.CopyColormapAndFree()
    {
        throw new NotImplementedException();
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

    void IXProto.CreateColormap()
    {
        throw new NotImplementedException();
    }

    void IXProto.CreateCursor()
    {
        throw new NotImplementedException();
    }

    void IXProto.CreateGC(uint gc, uint drawable, GCMask mask, params uint[] args)
    {
        var requiredBuffer = 16 + args.Length * 4;
        if (requiredBuffer < GlobalSetting.StackAllocThreshold)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
            scratchBuffer[0] = (byte)Opcode.CreateGC;
            scratchBuffer[1] = 0;
            MemoryMarshal.Write(scratchBuffer[2..4], (ushort)(requiredBuffer / 4));
            MemoryMarshal.Write(scratchBuffer[4..8], gc);
            MemoryMarshal.Write(scratchBuffer[8..12], drawable);
            MemoryMarshal.Write(scratchBuffer[12..16], mask);
            args.AsSpan().CopyTo(MemoryMarshal.Cast<byte, uint>(scratchBuffer[16..requiredBuffer]));
            _socket.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            scratchBuffer[0] = (byte)Opcode.CreateGC;
            scratchBuffer[1] = 0;
            MemoryMarshal.Write(scratchBuffer[2..4], (ushort)(requiredBuffer / 4));
            MemoryMarshal.Write(scratchBuffer[4..8], gc);
            MemoryMarshal.Write(scratchBuffer[8..12], drawable);
            MemoryMarshal.Write(scratchBuffer[12..16], mask);
            args.AsSpan().CopyTo(MemoryMarshal.Cast<byte, uint>(scratchBuffer[16..requiredBuffer]));
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
        var requiredBuffer = 32 + args.Length * 4;
        if (requiredBuffer < GlobalSetting.StackAllocThreshold)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
            scratchBuffer[0] = (byte)Opcode.CreateWindow;
            scratchBuffer[1] = 0;
            MemoryMarshal.Write(scratchBuffer[2..4], (ushort)(requiredBuffer / 4));
            MemoryMarshal.Write(scratchBuffer[4..8], window);
            MemoryMarshal.Write(scratchBuffer[8..12], parent);
            MemoryMarshal.Write(scratchBuffer[12..14], x);
            MemoryMarshal.Write(scratchBuffer[14..16], y);
            MemoryMarshal.Write(scratchBuffer[16..18], width);
            MemoryMarshal.Write(scratchBuffer[18..20], height);
            MemoryMarshal.Write(scratchBuffer[20..22], borderWidth);
            MemoryMarshal.Write(scratchBuffer[22..24], classType);
            MemoryMarshal.Write(scratchBuffer[24..28], rootVisualId);
            MemoryMarshal.Write(scratchBuffer[28..32], mask);
            args.AsSpan().CopyTo(MemoryMarshal.Cast<byte, uint>(scratchBuffer[32..requiredBuffer]));
            _socket.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            scratchBuffer[0] = (byte)Opcode.CreateWindow;
            scratchBuffer[1] = 0;
            MemoryMarshal.Write(scratchBuffer[2..4], (ushort)(requiredBuffer / 4));
            MemoryMarshal.Write(scratchBuffer[4..8], window);
            MemoryMarshal.Write(scratchBuffer[8..12], parent);
            MemoryMarshal.Write(scratchBuffer[12..14], x);
            MemoryMarshal.Write(scratchBuffer[14..16], y);
            MemoryMarshal.Write(scratchBuffer[16..18], width);
            MemoryMarshal.Write(scratchBuffer[18..20], height);
            MemoryMarshal.Write(scratchBuffer[20..22], borderWidth);
            MemoryMarshal.Write(scratchBuffer[22..24], classType);
            MemoryMarshal.Write(scratchBuffer[24..28], rootVisualId);
            MemoryMarshal.Write(scratchBuffer[28..32], mask);
            args.AsSpan().CopyTo(MemoryMarshal.Cast<byte, uint>(scratchBuffer[32..requiredBuffer]));
            _socket.SendExact(scratchBuffer[..requiredBuffer]);
        }
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
        Span<byte> scratchBuffer = stackalloc byte[8];
        scratchBuffer[0] = (byte)Opcode.DestroyWindow;
        scratchBuffer[1] = 0;
        MemoryMarshal.Write(scratchBuffer[2..4], (ushort)2);
        MemoryMarshal.Write(scratchBuffer[4..8], window);
        _socket.SendExact(scratchBuffer);
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

    void IXProto.FreeColormap()
    {
        throw new NotImplementedException();
    }

    void IXProto.FreeColors()
    {
        throw new NotImplementedException();
    }

    void IXProto.FreeCursor()
    {
        throw new NotImplementedException();
    }

    void IXProto.FreeGC(uint gc)
    {
        Span<byte> scratchBuffer = stackalloc byte[8];
        scratchBuffer[0] = (byte)Opcode.FreeGC;
        scratchBuffer[1] = 0;
        MemoryMarshal.Write<ushort>(scratchBuffer[2..4], 2);
        MemoryMarshal.Write(scratchBuffer[4..8], gc);
        _socket.SendExact(scratchBuffer);
    }

    void IXProto.FreePixmap()
    {
        throw new NotImplementedException();
    }

    void IXProto.GetAtomName()
    {
        throw new NotImplementedException();
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

    void IXProto.GetProperty()
    {
        throw new NotImplementedException();
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
        throw new NotImplementedException();
    }

    void IXProto.ImageText16()
    {
        throw new NotImplementedException();
    }

    void IXProto.ImageText8()
    {
        throw new NotImplementedException();
    }

    void IXProto.InstallColormap()
    {
        throw new NotImplementedException();
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
        Span<byte> scratchBuffer = stackalloc byte[8];
        scratchBuffer[0] = (byte)Opcode.MapWindow;
        scratchBuffer[1] = 0;
        MemoryMarshal.Write<ushort>(scratchBuffer[2..4], 2);
        MemoryMarshal.Write(scratchBuffer[4..8], window);
        _socket.SendExact(scratchBuffer);
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

    void IXProto.OpenFont()
    {
        throw new NotImplementedException();
    }

    void IXProto.PolyArc()
    {
        throw new NotImplementedException();
    }

    void IXProto.PolyFillArc()
    {
        throw new NotImplementedException();
    }

    void IXProto.PolyFillRectangle()
    {
        throw new NotImplementedException();
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
        Span<byte> scratchBuffer = stackalloc byte[24];
        scratchBuffer[0] = (byte)Opcode.PutImage;
        scratchBuffer[1] = (byte)format;
        MemoryMarshal.Write(scratchBuffer[2..4], (ushort)((6 + (data.Length.AddPadding() / 4))));
        MemoryMarshal.Write(scratchBuffer[4..8], drawable);
        MemoryMarshal.Write(scratchBuffer[8..12], gc);
        MemoryMarshal.Write(scratchBuffer[12..14], width);
        MemoryMarshal.Write(scratchBuffer[14..16], height);
        MemoryMarshal.Write(scratchBuffer[16..18], x);
        MemoryMarshal.Write(scratchBuffer[18..20], y);
        scratchBuffer[21] = leftPad;
        scratchBuffer[22] = depth;
        MemoryMarshal.Write(scratchBuffer[22..24], 0);
        _socket.SendExact(scratchBuffer);
        _socket.SendExact(data);
        scratchBuffer = scratchBuffer[..data.Length.Padding()];
        scratchBuffer.Clear();
        _socket.SendExact(scratchBuffer);
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

    void IXProto.QueryPointer()
    {
        throw new NotImplementedException();
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
        var requiredBuffer = 12 + properties.Length * 4;
        if (requiredBuffer < GlobalSetting.StackAllocThreshold)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
            scratchBuffer[0] = (byte)Opcode.RotateProperties;
            scratchBuffer[1] = 0;
            MemoryMarshal.Write(scratchBuffer[2..4], (ushort)(requiredBuffer / 4));
            MemoryMarshal.Write(scratchBuffer[4..8], window);
            MemoryMarshal.Write(scratchBuffer[8..10], (ushort)properties.Length);
            MemoryMarshal.Write(scratchBuffer[10..12], delta);
            properties.AsSpan().CopyTo(MemoryMarshal.Cast<byte, uint>(scratchBuffer[12..requiredBuffer]));
            _socket.SendExact(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            scratchBuffer[0] = (byte)Opcode.RotateProperties;
            scratchBuffer[1] = 0;
            MemoryMarshal.Write(scratchBuffer[2..4], (ushort)(requiredBuffer / 4));
            MemoryMarshal.Write(scratchBuffer[4..8], window);
            MemoryMarshal.Write(scratchBuffer[8..10], (ushort)properties.Length);
            MemoryMarshal.Write(scratchBuffer[10..12], delta);
            properties.AsSpan().CopyTo(MemoryMarshal.Cast<byte, uint>(scratchBuffer[12..requiredBuffer]));
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

    void IXProto.SetInputFocus()
    {
        throw new NotImplementedException();
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
        scratchBuffer[9] = (byte)preferBlanking;
        scratchBuffer[10] = (byte)allowExposures;
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

    void IXProto.StoreColors()
    {
        throw new NotImplementedException();
    }

    void IXProto.StoreNamedColor()
    {
        throw new NotImplementedException();
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
        throw new NotImplementedException();
    }

    void IXProto.UninstallColormap()
    {
        throw new NotImplementedException();
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
        Span<byte> scratchBuffer = stackalloc byte[8];
        scratchBuffer[0] = (byte)Opcode.UnmapWindow;
        scratchBuffer[1] = 0;
        MemoryMarshal.Write<ushort>(scratchBuffer[2..4], 2);
        MemoryMarshal.Write(scratchBuffer[4..8], window);
        _socket.SendExact(scratchBuffer);
    }

    void IXProto.WarpPointer()
    {
        throw new NotImplementedException();
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                if (_socket.Connected) _socket.Close();
            }

            _disposedValue = true;
        }
    }

    void IDisposable.Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    public uint NewId()
    {
        var result = (uint)((_connectionResult.ResourceIDMask & _globalId) | _connectionResult.ResourceIDBase);
        _globalId++;
        return result;
    }

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