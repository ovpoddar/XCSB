using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Xcsb.Helpers;
using Xcsb.Masks;
using Xcsb.Models;
using Xcsb.Models.Event;
using Xcsb.Models.Requests;

namespace Xcsb;
internal class XBufferProto : IXBufferProto
{
    private readonly Socket _socket;
    private List<byte> _buffer = new List<byte>();

    public XBufferProto(Socket socket)
    {
        _socket = socket ?? throw new ArgumentNullException(nameof(socket));
        // todo: pass a configuration object and based on that set up the XBufferProto
    }

    public void AllowEvents(
        EventsMode mode,
        uint time)
    {
        var request = new AllowEventsType(mode, time);
        _buffer.Add(ref request);
    }

    public void Bell(sbyte percent)
    {
        if (percent is not <= 100 or not >= (-100))
            throw new ArgumentOutOfRangeException(nameof(percent), "value must be between -100 to 100");

        var request = new BellType(percent);
        _buffer.Add(ref request);
    }

    public void ChangeActivePointerGrab(uint cursor, uint time, ushort mask)
    {
        var request = new ChangeActivePointerGrabType(cursor, time, mask);
        _buffer.Add(ref request);
    }

    public void ChangeGC(uint gc, GCMask mask, params uint[] args)
    {
        throw new NotImplementedException();
    }

    public void ChangeHosts(HostMode mode, Family family, byte[] address)
    {
        throw new NotImplementedException();
    }

    public void ChangeKeyboardControl(KeyboardControlMask mask, params uint[] args)
    {
        throw new NotImplementedException();
    }

    public void ChangeKeyboardMapping(byte keycodeCount, byte firstKeycode, byte keysymsPerKeycode, uint[] Keysym)
    {
        throw new NotImplementedException();
    }

    public void ChangePointerControl(Acceleration acceleration, ushort? threshold)
    {
        throw new NotImplementedException();
    }

    public void ChangeProperty<T>(PropertyMode mode, uint window, uint property, uint type, params T[] args) where T : struct, INumber<T>
    {
        throw new NotImplementedException();
    }

    public void ChangeSaveSet(ChangeSaveSetMode changeSaveSetMode, uint window)
    {
        throw new NotImplementedException();
    }

    public void ChangeWindowAttributes(uint window, ValueMask mask, params uint[] args)
    {
        throw new NotImplementedException();
    }

    public void CirculateWindow(Direction direction, uint window)
    {
        throw new NotImplementedException();
    }

    public void ClearArea(bool exposures, uint window, short x, short y, ushort width, ushort height)
    {
        throw new NotImplementedException();
    }

    public void CloseFont(uint fontId)
    {
        throw new NotImplementedException();
    }

    public void ConfigureWindow(uint window, ConfigureValueMask mask, params uint[] args)
    {
        throw new NotImplementedException();
    }

    public void ConvertSelection(uint requestor, uint selection, uint target, uint property, uint timestamp)
    {
        throw new NotImplementedException();
    }

    public void CopyArea(uint srcDrawable, uint destDrawable, uint gc, ushort srcX, ushort srcY, ushort destX, ushort destY, ushort width, ushort height)
    {
        throw new NotImplementedException();
    }

    public void CopyColormapAndFree(uint colormapId, uint srcColormapId)
    {
        throw new NotImplementedException();
    }

    public void CopyGC(uint srcGc, uint dstGc, GCMask mask)
    {
        throw new NotImplementedException();
    }

    public void CopyPlane(uint srcDrawable, uint destDrawable, uint gc, ushort srcX, ushort srcY, ushort destX, ushort destY, ushort width, ushort height, uint bitPlane)
    {
        throw new NotImplementedException();
    }

    public void CreateColormap(ColormapAlloc alloc, uint colormapId, uint window, uint visual)
    {
        throw new NotImplementedException();
    }

    public void CreateCursor(uint cursorId, uint source, uint mask, ushort foreRed, ushort foreGreen, ushort foreBlue, ushort backRed, ushort backGreen, ushort backBlue, ushort x, ushort y)
    {
        throw new NotImplementedException();
    }

    public void CreateGC(uint gc, uint drawable, GCMask mask, params uint[] args)
    {
        throw new NotImplementedException();
    }

    public void CreateGlyphCursor(uint cursorId, uint sourceFont, uint fontMask, char sourceChar, ushort charMask, ushort foreRed, ushort foreGreen, ushort foreBlue, ushort backRed, ushort backGreen, ushort backBlue)
    {
        throw new NotImplementedException();
    }

    public void CreatePixmap(byte depth, uint pixmapId, uint drawable, ushort width, ushort height)
    {
        throw new NotImplementedException();
    }

    public void CreateWindow(byte depth, uint window, uint parent, short x, short y, ushort width, ushort height, ushort borderWidth, ClassType classType, uint rootVisualId, ValueMask mask, params uint[] args)
    {
        throw new NotImplementedException();
    }

    public void DeleteProperty(uint window, uint atom)
    {
        throw new NotImplementedException();
    }

    public void DestroySubwindows(uint window)
    {
        throw new NotImplementedException();
    }

    public void DestroyWindow(uint window)
    {
        throw new NotImplementedException();
    }

    public void FillPoly(uint drawable, uint gc, PolyShape shape, CoordinateMode coordinate, Point[] points)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<ErrorEvent> Flush()
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<ErrorEvent>> FlushAsync()
    {
        throw new NotImplementedException();
    }

    public void ForceScreenSaver(ForceScreenSaverMode mode)
    {
        throw new NotImplementedException();
    }

    public void FreeColormap(uint colormapId)
    {
        throw new NotImplementedException();
    }

    public void FreeColors(uint colormapId, uint planeMask, params uint[] pixels)
    {
        throw new NotImplementedException();
    }

    public void FreeCursor(uint cursorId)
    {
        throw new NotImplementedException();
    }

    public void FreeGC(uint gc)
    {
        throw new NotImplementedException();
    }

    public void FreePixmap(uint pixmapId)
    {
        throw new NotImplementedException();
    }

    public void GrabButton(bool ownerEvents, uint grabWindow, ushort mask, GrabMode pointerMode, GrabMode keyboardMode, uint confineTo, uint cursor, Button button, ModifierMask modifiers)
    {
        throw new NotImplementedException();
    }

    public void GrabKey(bool exposures, uint grabWindow, ModifierMask mask, byte keycode, GrabMode pointerMode, GrabMode keyboardMode)
    {
        throw new NotImplementedException();
    }

    public void GrabServer()
    {
        throw new NotImplementedException();
    }

    public void ImageText16(uint drawable, uint gc, short x, short y, ReadOnlySpan<char> text)
    {
        throw new NotImplementedException();
    }

    public void ImageText8(uint drawable, uint gc, short x, short y, ReadOnlySpan<byte> text)
    {
        throw new NotImplementedException();
    }

    public void InstallColormap(uint colormapId)
    {
        throw new NotImplementedException();
    }

    public void KillClient(uint resource)
    {
        throw new NotImplementedException();
    }

    public void MapSubwindows(uint window)
    {
        throw new NotImplementedException();
    }

    public void MapWindow(uint window)
    {
        throw new NotImplementedException();
    }

    public void NoOperation(params uint[] args)
    {
        throw new NotImplementedException();
    }

    public void OpenFont(string fontName, uint fontId)
    {
        throw new NotImplementedException();
    }

    public void PolyArc(uint drawable, uint gc, Arc[] arcs)
    {
        throw new NotImplementedException();
    }

    public void PolyFillArc(uint drawable, uint gc, Arc[] arcs)
    {
        throw new NotImplementedException();
    }

    public void PolyFillRectangle(uint drawable, uint gc, Rectangle[] rectangles)
    {
        throw new NotImplementedException();
    }

    public void PolyLine(CoordinateMode coordinate, uint drawable, uint gc, Point[] points)
    {
        throw new NotImplementedException();
    }

    public void PolyPoint(CoordinateMode coordinate, uint drawable, uint gc, Point[] points)
    {
        throw new NotImplementedException();
    }

    public void PolyRectangle(uint drawable, uint gc, Rectangle[] rectangles)
    {
        throw new NotImplementedException();
    }

    public void PolySegment(uint drawable, uint gc, Segment[] segments)
    {
        throw new NotImplementedException();
    }

    public void PolyText16()
    {
        throw new NotImplementedException();
    }

    public void PolyText8()
    {
        throw new NotImplementedException();
    }

    public void PutImage(ImageFormat format, uint drawable, uint gc, ushort width, ushort height, short x, short y, byte leftPad, byte depth, Span<byte> data)
    {
        throw new NotImplementedException();
    }

    public void RecolorCursor(uint cursorId, ushort foreRed, ushort foreGreen, ushort foreBlue, ushort backRed, ushort backGreen, ushort backBlue)
    {
        throw new NotImplementedException();
    }

    public void ReparentWindow(uint window, uint parent, short x, short y)
    {
        throw new NotImplementedException();
    }

    public void RotateProperties(uint window, ushort delta, params uint[] properties)
    {
        throw new NotImplementedException();
    }

    public void SendEvent(bool propagate, uint destination, uint eventMask, XEvent evnt)
    {
        throw new NotImplementedException();
    }

    public void SetAccessControl(AccessControlMode mode)
    {
        throw new NotImplementedException();
    }

    public void SetClipRectangles(ClipOrdering ordering, uint gc, ushort clipX, ushort clipY, Rectangle[] rectangles)
    {
        throw new NotImplementedException();
    }

    public void SetCloseDownMode(CloseDownMode mode)
    {
        throw new NotImplementedException();
    }

    public void SetDashes(uint gc, ushort dashOffset, byte[] dashes)
    {
        throw new NotImplementedException();
    }

    public void SetFontPath(string[] strPaths)
    {
        throw new NotImplementedException();
    }

    public void SetInputFocus(InputFocusMode mode, uint focus, uint time)
    {
        throw new NotImplementedException();
    }

    public void SetScreenSaver(short timeout, short interval, TriState preferBlanking, TriState allowExposures)
    {
        throw new NotImplementedException();
    }

    public void SetSelectionOwner(uint owner, uint atom, uint timestamp)
    {
        throw new NotImplementedException();
    }

    public void StoreColors(uint colormapId, params ColorItem[] item)
    {
        throw new NotImplementedException();
    }

    public void StoreNamedColor(ColorFlag mode, uint colormapId, uint pixels, ReadOnlySpan<byte> name)
    {
        throw new NotImplementedException();
    }

    public void UngrabButton(Button button, uint grabWindow, ModifierMask mask)
    {
        throw new NotImplementedException();
    }

    public void UngrabKey(byte key, uint grabWindow, ModifierMask modifier)
    {
        throw new NotImplementedException();
    }

    public void UngrabKeyboard(uint time)
    {
        throw new NotImplementedException();
    }

    public void UngrabPointer(uint time)
    {
        throw new NotImplementedException();
    }

    public void UngrabServer()
    {
        throw new NotImplementedException();
    }

    public void UninstallColormap(uint colormapId)
    {
        throw new NotImplementedException();
    }

    public void UnmapSubwindows(uint window)
    {
        throw new NotImplementedException();
    }

    public void UnmapWindow(uint window)
    {
        throw new NotImplementedException();
    }

    public void WarpPointer(uint srcWindow, uint destWindow, short srcX, short srcY, ushort srcWidth, ushort srcHeight, short destX, short destY)
    {
        throw new NotImplementedException();
    }
}
