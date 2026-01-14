using System;
using System.Collections.Generic;
using System.Text;
using Xcsb.Masks;
using Xcsb.Models;
using Xcsb.Models.String;

#if !NETSTANDARD
using System.Numerics;
#endif

namespace Xcsb.Infrastructure.VoidProto;

public interface IVoidProtoUnchecked
{
    void CreateWindowUnchecked(byte depth, uint window, uint parent, short x, short y, ushort width, ushort height,
        ushort borderWidth, ClassType classType, uint rootVisualId, ValueMask mask, Span<uint> args);

    void ChangeWindowAttributesUnchecked(uint window, ValueMask mask, Span<uint> args);

    void DestroyWindowUnchecked(uint window);
    void DestroySubwindowsUnchecked(uint window);

    void ChangeSaveSetUnchecked(ChangeSaveSetMode changeSaveSetMode, uint window);

    void ReparentWindowUnchecked(uint window, uint parent, short x, short y);

    void MapWindowUnchecked(uint window);
    void MapSubwindowsUnchecked(uint window);
    void UnmapWindowUnchecked(uint window);
    void UnmapSubwindowsUnchecked(uint window);

    void ConfigureWindowUnchecked(uint window, ConfigureValueMask mask, Span<uint> args);

    void CirculateWindowUnchecked(Circulate circulate, uint window);

    void ChangePropertyUnchecked<T>(PropertyMode mode, uint window, ATOM property, ATOM type, Span<T> args)
        where T : struct
#if !NETSTANDARD
        , INumber<T>
#endif
    ;

    void DeletePropertyUnchecked(uint window, ATOM atom);

    void RotatePropertiesUnchecked(uint window, ushort delta, Span<ATOM> properties);

    void SetSelectionOwnerUnchecked(uint owner, ATOM atom, uint timestamp);

    void ConvertSelectionUnchecked(uint requestor, ATOM selection, ATOM target, ATOM property, uint timestamp);

    void SendEventUnchecked(bool propagate, uint destination, uint eventMask, XEvent evnt);

    void UngrabPointerUnchecked(uint time);

    void GrabButtonUnchecked(bool ownerEvents, uint grabWindow, ushort mask, GrabMode pointerMode, GrabMode keyboardMode,
        uint confineTo, uint cursor, Button button, ModifierMask modifiers);

    void UngrabButtonUnchecked(Button button, uint grabWindow, ModifierMask mask);

    void ChangeActivePointerGrabUnchecked(uint cursor, uint time, ushort mask);

    void UngrabKeyboardUnchecked(uint time);

    void GrabKeyUnchecked(bool exposures, uint grabWindow, ModifierMask mask, byte keycode, GrabMode pointerMode,
        GrabMode keyboardMode);

    void UngrabKeyUnchecked(byte key, uint grabWindow, ModifierMask modifier);

    void AllowEventsUnchecked(EventsMode mode, uint time);

    void GrabServerUnchecked();
    void UngrabServerUnchecked();

    void WarpPointerUnchecked(uint srcWindow, uint destinationWindow, short srcX, short srcY, ushort srcWidth, ushort srcHeight,
        short destinationX, short destinationY);

    void SetInputFocusUnchecked(InputFocusMode mode, uint focus, uint time);

    void OpenFontUnchecked(string fontName, uint fontId);

    void CloseFontUnchecked(uint fontId);
    void SetFontPathUnchecked(string[] strPaths);

    void CreatePixmapUnchecked(byte depth, uint pixmapId, uint drawable, ushort width, ushort height);

    void FreePixmapUnchecked(uint pixmapId);

    void CreateGCUnchecked(uint gc, uint drawable, GCMask mask, Span<uint> args);

    void ChangeGCUnchecked(uint gc, GCMask mask, Span<uint> args);

    void CopyGCUnchecked(uint srcGc, uint dstGc, GCMask mask);

    void SetDashesUnchecked(uint gc, ushort dashOffset, Span<byte> dashes);

    void SetClipRectanglesUnchecked(ClipOrdering ordering, uint gc, ushort clipX, ushort clipY, Span<Rectangle> rectangles);

    void FreeGCUnchecked(uint gc);

    void ClearAreaUnchecked(bool exposures, uint window, short x, short y, ushort width, ushort height);

    void CopyAreaUnchecked(uint srcDrawable, uint destinationDrawable, uint gc, ushort srcX, ushort srcY, ushort destinationX,
        ushort destinationY, ushort width, ushort height);

    void CopyPlaneUnchecked(uint srcDrawable, uint destinationDrawable, uint gc, ushort srcX, ushort srcY, ushort destinationX,
        ushort destinationY, ushort width, ushort height, uint bitPlane);

    void PolyPointUnchecked(CoordinateMode coordinate, uint drawable, uint gc, Span<Point> points);

    void PolyLineUnchecked(CoordinateMode coordinate, uint drawable, uint gc, Span<Point> points);

    void PolySegmentUnchecked(uint drawable, uint gc, Span<Segment> segments);

    void PolyRectangleUnchecked(uint drawable, uint gc, Span<Rectangle> rectangles);

    void PolyArcUnchecked(uint drawable, uint gc, Span<Arc> arcs);

    void FillPolyUnchecked(uint drawable, uint gc, PolyShape shape, CoordinateMode coordinate, Span<Point> points);

    void PolyFillRectangleUnchecked(uint drawable, uint gc, Span<Rectangle> rectangles);

    void PolyFillArcUnchecked(uint drawable, uint gc, Span<Arc> arcs);

    void PutImageUnchecked(ImageFormatBitmap format, uint drawable, uint gc, ushort width, ushort height, short x, short y,
        byte leftPad, byte depth, Span<byte> data);

    void ImageText8Unchecked(uint drawable, uint gc, short x, short y, ReadOnlySpan<byte> text);

    void ImageText16Unchecked(uint drawable, uint gc, short x, short y, ReadOnlySpan<char> text);

    void CreateColormapUnchecked(ColormapAlloc alloc, uint colormapId, uint window, uint visual);

    void FreeColormapUnchecked(uint colormapId);

    void CopyColormapAndFreeUnchecked(uint colormapId, uint srcColormapId);

    void InstallColormapUnchecked(uint colormapId);
    void UninstallColormapUnchecked(uint colormapId);

    void FreeColorsUnchecked(uint colormapId, uint planeMask, Span<uint> pixels);

    void StoreColorsUnchecked(uint colormapId, Span<ColorItem> item);

    void StoreNamedColorUnchecked(ColorFlag mode, uint colormapId, uint pixels, ReadOnlySpan<byte> name);

    void CreateCursorUnchecked(uint cursorId, uint source, uint mask, ushort foreRed, ushort foreGreen, ushort foreBlue,
        ushort backRed, ushort backGreen, ushort backBlue, ushort x, ushort y);

    void CreateGlyphCursorUnchecked(uint cursorId, uint sourceFont, uint fontMask, char sourceChar, ushort charMask,
        ushort foreRed, ushort foreGreen, ushort foreBlue, ushort backRed, ushort backGreen, ushort backBlue);

    void FreeCursorUnchecked(uint cursorId);

    void RecolorCursorUnchecked(uint cursorId, ushort foreRed, ushort foreGreen, ushort foreBlue, ushort backRed,
        ushort backGreen, ushort backBlue);

    // suppose need changes
    void ChangeKeyboardMappingUnchecked(byte keycodeCount, byte firstKeycode, byte keysymsPerKeycode, Span<uint> Keysym);

    void BellUnchecked(sbyte percent);

    void ChangeKeyboardControlUnchecked(KeyboardControlMask mask, Span<uint> args);

    void ChangePointerControlUnchecked(Acceleration? acceleration, ushort? threshold);

    void SetScreenSaverUnchecked(short timeout, short interval, TriState preferBlanking, TriState allowExposures);

    void ForceScreenSaverUnchecked(ForceScreenSaverMode mode);

    void ChangeHostsUnchecked(HostMode mode, Family family, Span<byte> address);

    void SetAccessControlUnchecked(AccessControlMode mode);
    void SetCloseDownModeUnchecked(CloseDownMode mode);
    void KillClientUnchecked(uint resource);

    void NoOperationUnchecked(Span<uint> args);

    void PolyText8Unchecked(uint drawable, uint gc, ushort x, ushort y, TextItem8[] data);
    void PolyText16Unchecked(uint drawable, uint gc, ushort x, ushort y, TextItem16[] data);
}
