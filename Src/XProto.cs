using Src.Helpers;
using Src.Models;
using Src.Models.Event;
using Src.Models.Handshake;
using System.Buffers;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Src;
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

    void IXProto.Bell()
    {
        throw new NotImplementedException();
    }

    void IXProto.ChangeActivePointerGrab()
    {
        throw new NotImplementedException();
    }

    void IXProto.ChangeGC()
    {
        throw new NotImplementedException();
    }

    void IXProto.ChangeHosts()
    {
        throw new NotImplementedException();
    }

    void IXProto.ChangeKeyboardControl()
    {
        throw new NotImplementedException();
    }

    void IXProto.ChangeKeyboardMapping()
    {
        throw new NotImplementedException();
    }

    void IXProto.ChangePointerControl()
    {
        throw new NotImplementedException();
    }

    void IXProto.ChangeProperty()
    {
        throw new NotImplementedException();
    }

    void IXProto.ChangeSaveSet()
    {
        throw new NotImplementedException();
    }

    void IXProto.ChangeWindowAttributes()
    {
        throw new NotImplementedException();
    }

    void IXProto.CirculateWindow()
    {
        throw new NotImplementedException();
    }

    void IXProto.ClearArea()
    {
        throw new NotImplementedException();
    }

    void IXProto.CloseFont()
    {
        throw new NotImplementedException();
    }

    void IXProto.ConfigureWindow()
    {
        throw new NotImplementedException();
    }

    void IXProto.ConvertSelection()
    {
        throw new NotImplementedException();
    }

    void IXProto.CopyArea()
    {
        throw new NotImplementedException();
    }

    void IXProto.CopyColormapAndFree()
    {
        throw new NotImplementedException();
    }

    void IXProto.CopyGC()
    {
        throw new NotImplementedException();
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

    void IXProto.CreateGC()
    {
        throw new NotImplementedException();
    }

    void IXProto.CreateGlyphCursor()
    {
        throw new NotImplementedException();
    }

    void IXProto.CreatePixmap()
    {
        throw new NotImplementedException();
    }

    [SkipLocalsInit]
    void IXProto.CreateWindow(int window,
        uint parent,
        short x,
        short y,
        ushort width,
        ushort height,
        ushort borderWidth,
        ClassType classType,
        uint rootVisualId,
        ValueMask mask,
        uint[] args,
        params uint[] args1
    )
    {
        var requiredBuffer = 32 + args.Length * 4 + args1.Length * 4;
        if (requiredBuffer < GlobalSetting.StackAllocThreshold)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
            scratchBuffer[0] = 1;
            scratchBuffer[1] = 0;
            MemoryMarshal.Write(scratchBuffer[2..4], (ushort)(requiredBuffer / 4));
            MemoryMarshal.Write(scratchBuffer[4..8], window);
            MemoryMarshal.Write(scratchBuffer[8..12], parent);
            MemoryMarshal.Write(scratchBuffer[12..14], x);
            MemoryMarshal.Write(scratchBuffer[14..16], y);
            MemoryMarshal.Write(scratchBuffer[16..18], width);
            MemoryMarshal.Write(scratchBuffer[18..20], height);
            MemoryMarshal.Write(scratchBuffer[20..22], borderWidth);
            MemoryMarshal.Write(scratchBuffer[22..24], (ushort)classType);
            MemoryMarshal.Write(scratchBuffer[24..28], rootVisualId);
            MemoryMarshal.Write(scratchBuffer[28..32], (uint)mask);

            var writtenIndex = 32;
            foreach (var item in args)
            {
                MemoryMarshal.Write(scratchBuffer[writtenIndex..], item);
                writtenIndex += 4;
            }

            foreach (var item in args1)
            {
                MemoryMarshal.Write(scratchBuffer[writtenIndex..], item);
                writtenIndex += 4;
            }
            _socket.SendExact(scratchBuffer);
        }

    }

    void IXProto.DeleteProperty()
    {
        throw new NotImplementedException();
    }

    void IXProto.DestroySubwindows()
    {
        throw new NotImplementedException();
    }

    void IXProto.DestroyWindow()
    {
        throw new NotImplementedException();
    }

    void IXProto.FillPoly()
    {
        throw new NotImplementedException();
    }

    void IXProto.ForceScreenSaver()
    {
        throw new NotImplementedException();
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

    void IXProto.FreeGC()
    {
        throw new NotImplementedException();
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

    void IXProto.InternAtom()
    {
        throw new NotImplementedException();
    }

    void IXProto.KillClient()
    {
        throw new NotImplementedException();
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

    void IXProto.MapSubwindows()
    {
        throw new NotImplementedException();
    }

    [SkipLocalsInit]
    void IXProto.MapWindow(uint window)
    {
        Span<byte> values = stackalloc byte[8];
        values[0] = (byte)Opcode.MapWindow;
        values[1] = 0;
        MemoryMarshal.Write<ushort>(values[2..], 2);
        MemoryMarshal.Write(values[4..], window);
        _socket.SendExact(values, SocketFlags.None);
    }

    void IXProto.NoOperation()
    {
        throw new NotImplementedException();
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

    void IXProto.PutImage()
    {
        throw new NotImplementedException();
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

    void IXProto.ReparentWindow()
    {
        throw new NotImplementedException();
    }

    void IXProto.RotateProperties()
    {
        throw new NotImplementedException();
    }

    void IXProto.SendEvent()
    {
        throw new NotImplementedException();
    }

    void IXProto.SetAccessControl()
    {
        throw new NotImplementedException();
    }

    void IXProto.SetClipRectangles()
    {
        throw new NotImplementedException();
    }

    void IXProto.SetCloseDownMode()
    {
        throw new NotImplementedException();
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

    void IXProto.SetScreenSaver()
    {
        throw new NotImplementedException();
    }

    void IXProto.SetSelectionOwner()
    {
        throw new NotImplementedException();
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

    void IXProto.UngrabPointer()
    {
        throw new NotImplementedException();
    }

    void IXProto.UngrabServer()
    {
        throw new NotImplementedException();
    }

    void IXProto.UninstallColormap()
    {
        throw new NotImplementedException();
    }

    void IXProto.UnmapSubwindows()
    {
        throw new NotImplementedException();
    }

    void IXProto.UnmapWindow()
    {
        throw new NotImplementedException();
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
        _globalId += 1;
        return result;
    }

    public XEvent GetEvent()
    {
        Span<byte> scratchBuffer = stackalloc byte[Marshal.SizeOf<XEvent>()];
        _socket.Receive(scratchBuffer);
        var result = scratchBuffer.ToStruct<XEvent>();
        return result;
    }
}
