using Xcsb.Masks;
using Xcsb.Models;
using Xcsb.Models.Event;
#if !NETSTANDARD
using System.Numerics;
#endif

namespace Xcsb;

public interface IVoidProto
{
    void CreateWindow(byte depth, uint window, uint parent, short x, short y, ushort width, ushort height,
        ushort borderWidth, ClassType classType, uint rootVisualId, ValueMask mask, params uint[] args);

    void ChangeWindowAttributes(uint window, ValueMask mask, params uint[] args);

    void DestroyWindow(uint window);
    void DestroySubwindows(uint window);

    void ChangeSaveSet(ChangeSaveSetMode changeSaveSetMode, uint window);

    void ReparentWindow(uint window, uint parent, short x, short y);

    void MapWindow(uint window);
    void MapSubwindows(uint window);
    void UnmapWindow(uint window);
    void UnmapSubwindows(uint window);

    void ConfigureWindow(uint window, ConfigureValueMask mask, params uint[] args);

    void CirculateWindow(Circulate circulate, uint window);

    void ChangeProperty<T>(PropertyMode mode, uint window, uint property, uint type, params T[] args)
        where T : struct
#if !NETSTANDARD
        , INumber<T>
#endif
    ;

    void DeleteProperty(uint window, uint atom);

    void RotateProperties(uint window, ushort delta, params uint[] properties);

    void SetSelectionOwner(uint owner, uint atom, uint timestamp);

    void ConvertSelection(uint requestor, uint selection, uint target, uint property, uint timestamp);

    void SendEvent(bool propagate, uint destination, uint eventMask, XEvent evnt);

    void UngrabPointer(uint time);

    void GrabButton(bool ownerEvents, uint grabWindow, ushort mask, GrabMode pointerMode, GrabMode keyboardMode,
        uint confineTo, uint cursor, Button button, ModifierMask modifiers);

    void UngrabButton(Button button, uint grabWindow, ModifierMask mask);

    void ChangeActivePointerGrab(uint cursor, uint time, ushort mask);

    void UngrabKeyboard(uint time);

    void GrabKey(bool exposures, uint grabWindow, ModifierMask mask, byte keycode, GrabMode pointerMode,
        GrabMode keyboardMode);

    void UngrabKey(byte key, uint grabWindow, ModifierMask modifier);

    void AllowEvents(EventsMode mode, uint time);

    void GrabServer();
    void UngrabServer();

    void WarpPointer(uint srcWindow, uint destinationWindow, short srcX, short srcY, ushort srcWidth, ushort srcHeight,
        short destinationX, short destinationY);

    void SetInputFocus(InputFocusMode mode, uint focus, uint time);

    void OpenFont(string fontName, uint fontId);

    void CloseFont(uint fontId);
    void SetFontPath(string[] strPaths);

    void CreatePixmap(byte depth, uint pixmapId, uint drawable, ushort width, ushort height);

    void FreePixmap(uint pixmapId);

    void CreateGC(uint gc, uint drawable, GCMask mask, params uint[] args);

    void ChangeGC(uint gc, GCMask mask, params uint[] args);

    void CopyGC(uint srcGc, uint dstGc, GCMask mask);

    void SetDashes(uint gc, ushort dashOffset, byte[] dashes);

    void SetClipRectangles(ClipOrdering ordering, uint gc, ushort clipX, ushort clipY, Rectangle[] rectangles);

    void FreeGC(uint gc);

    void ClearArea(bool exposures, uint window, short x, short y, ushort width, ushort height);

    void CopyArea(uint srcDrawable, uint destinationDrawable, uint gc, ushort srcX, ushort srcY, ushort destinationX,
        ushort destinationY,
        ushort width, ushort height);

    void CopyPlane(uint srcDrawable, uint destinationDrawable, uint gc, ushort srcX, ushort srcY, ushort destinationX,
        ushort destinationY,
        ushort width, ushort height, uint bitPlane);

    void PolyPoint(CoordinateMode coordinate, uint drawable, uint gc, Point[] points);

    void PolyLine(CoordinateMode coordinate, uint drawable, uint gc, Point[] points);

    void PolySegment(uint drawable, uint gc, Segment[] segments);

    void PolyRectangle(uint drawable, uint gc, Rectangle[] rectangles);

    void PolyArc(uint drawable, uint gc, Arc[] arcs);

    void FillPoly(uint drawable, uint gc, PolyShape shape, CoordinateMode coordinate, Point[] points);

    void PolyFillRectangle(uint drawable, uint gc, Rectangle[] rectangles);

    void PolyFillArc(uint drawable, uint gc, Arc[] arcs);

    void PutImage(ImageFormat format, uint drawable, uint gc, ushort width, ushort height, short x, short y,
        byte leftPad, byte depth, Span<byte> data);

    void ImageText8(uint drawable, uint gc, short x, short y, ReadOnlySpan<byte> text);

    void ImageText16(uint drawable, uint gc, short x, short y, ReadOnlySpan<char> text);

    void CreateColormap(ColormapAlloc alloc, uint colormapId, uint window, uint visual);

    void FreeColormap(uint colormapId);

    void CopyColormapAndFree(uint colormapId, uint srcColormapId);

    void InstallColormap(uint colormapId);
    void UninstallColormap(uint colormapId);

    void FreeColors(uint colormapId, uint planeMask, params uint[] pixels);

    void StoreColors(uint colormapId, params ColorItem[] item);

    void StoreNamedColor(ColorFlag mode, uint colormapId, uint pixels, ReadOnlySpan<byte> name);

    void CreateCursor(uint cursorId, uint source, uint mask, ushort foreRed, ushort foreGreen, ushort foreBlue,
        ushort backRed, ushort backGreen, ushort backBlue, ushort x, ushort y);

    void CreateGlyphCursor(uint cursorId, uint sourceFont, uint fontMask, char sourceChar, ushort charMask,
        ushort foreRed, ushort foreGreen, ushort foreBlue, ushort backRed, ushort backGreen, ushort backBlue);

    void FreeCursor(uint cursorId);

    void RecolorCursor(uint cursorId, ushort foreRed, ushort foreGreen, ushort foreBlue, ushort backRed,
        ushort backGreen, ushort backBlue);

    // suppose need changes
    void ChangeKeyboardMapping(byte keycodeCount, byte firstKeycode, byte keysymsPerKeycode, uint[] Keysym);

    void Bell(sbyte percent);

    void ChangeKeyboardControl(KeyboardControlMask mask, params uint[] args);

    void ChangePointerControl(Acceleration acceleration, ushort? threshold);

    void SetScreenSaver(short timeout, short interval, TriState preferBlanking, TriState allowExposures);

    void ForceScreenSaver(ForceScreenSaverMode mode);

    void ChangeHosts(HostMode mode, Family family, byte[] address);

    void SetAccessControl(AccessControlMode mode);
    void SetCloseDownMode(CloseDownMode mode);
    void KillClient(uint resource);

    void NoOperation(params uint[] args);

    // todo: need a writer for the TEXTITEM16, TEXTITEM8
    void PolyText8(uint drawable, uint gc, ushort x, ushort y, Span<byte> data);
    void PolyText16(uint drawable, uint gc, ushort x, ushort y, Span<byte> data);
}