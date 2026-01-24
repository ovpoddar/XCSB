using Xcsb.Extension.Generic.Event.Masks;
using Xcsb.Extension.Generic.Event.Models.String;
using Xcsb.Models;





#if !NETSTANDARD
using System.Numerics;
#endif

namespace Xcsb.Extension.Generic.Event.Infrastructure.VoidProto;

public interface IVoidProtoChecked
{
    void CreateWindowChecked(byte depth, uint window, uint parent, short x, short y, ushort width, ushort height,
        ushort borderWidth, ClassType classType, uint rootVisualId, ValueMask mask, Span<uint> args);

    void ChangeWindowAttributesChecked(uint window, ValueMask mask, Span<uint> args);

    void DestroyWindowChecked(uint window);
    void DestroySubwindowsChecked(uint window);

    void ChangeSaveSetChecked(ChangeSaveSetMode changeSaveSetMode, uint window);

    void ReparentWindowChecked(uint window, uint parent, short x, short y);

    void MapWindowChecked(uint window);
    void MapSubwindowsChecked(uint window);
    void UnmapWindowChecked(uint window);
    void UnmapSubwindowsChecked(uint window);

    void ConfigureWindowChecked(uint window, ConfigureValueMask mask, Span<uint> args);

    void CirculateWindowChecked(Circulate circulate, uint window);

    void ChangePropertyChecked<T>(PropertyMode mode, uint window, ATOM property, ATOM type, Span<T> args)
        where T : struct
#if !NETSTANDARD
        , INumber<T>
#endif
    ;

    void DeletePropertyChecked(uint window, ATOM atom);

    void RotatePropertiesChecked(uint window, ushort delta, Span<ATOM> properties);

    void SetSelectionOwnerChecked(uint owner, ATOM atom, uint timestamp);

    void ConvertSelectionChecked(uint requestor, ATOM selection, ATOM target, ATOM property, uint timestamp);

    void SendEventChecked(bool propagate, uint destination, uint eventMask, XEvent evnt);

    void UngrabPointerChecked(uint time);

    void GrabButtonChecked(bool ownerEvents, uint grabWindow, ushort mask, GrabMode pointerMode, GrabMode keyboardMode,
        uint confineTo, uint cursor, Button button, ModifierMask modifiers);

    void UngrabButtonChecked(Button button, uint grabWindow, ModifierMask mask);

    void ChangeActivePointerGrabChecked(uint cursor, uint time, ushort mask);

    void UngrabKeyboardChecked(uint time);

    void GrabKeyChecked(bool exposures, uint grabWindow, ModifierMask mask, byte keycode, GrabMode pointerMode,
        GrabMode keyboardMode);

    void UngrabKeyChecked(byte key, uint grabWindow, ModifierMask modifier);

    void AllowEventsChecked(EventsMode mode, uint time);

    void GrabServerChecked();
    void UngrabServerChecked();

    void WarpPointerChecked(uint srcWindow, uint destinationWindow, short srcX, short srcY, ushort srcWidth,
        ushort srcHeight, short destinationX, short destinationY);

    void SetInputFocusChecked(InputFocusMode mode, uint focus, uint time);

    void OpenFontChecked(string fontName, uint fontId);

    void CloseFontChecked(uint fontId);
    void SetFontPathChecked(string[] strPaths);

    void CreatePixmapChecked(byte depth, uint pixmapId, uint drawable, ushort width, ushort height);

    void FreePixmapChecked(uint pixmapId);

    void CreateGCChecked(uint gc, uint drawable, GCMask mask, Span<uint> args);

    void ChangeGCChecked(uint gc, GCMask mask, Span<uint> args);

    void CopyGCChecked(uint srcGc, uint dstGc, GCMask mask);

    void SetDashesChecked(uint gc, ushort dashOffset, Span<byte> dashes);

    void SetClipRectanglesChecked(ClipOrdering ordering, uint gc, ushort clipX, ushort clipY, Span<Rectangle> rectangles);

    void FreeGCChecked(uint gc);

    void ClearAreaChecked(bool exposures, uint window, short x, short y, ushort width, ushort height);

    void CopyAreaChecked(uint srcDrawable, uint destinationDrawable, uint gc, ushort srcX, ushort srcY,
        ushort destinationX, ushort destinationY, ushort width, ushort height);

    void CopyPlaneChecked(uint srcDrawable, uint destinationDrawable, uint gc, ushort srcX, ushort srcY,
        ushort destinationX, ushort destinationY, ushort width, ushort height, uint bitPlane);

    void PolyPointChecked(CoordinateMode coordinate, uint drawable, uint gc, Span<Point> points);

    void PolyLineChecked(CoordinateMode coordinate, uint drawable, uint gc, Span<Point> points);

    void PolySegmentChecked(uint drawable, uint gc, Span<Segment> segments);

    void PolyRectangleChecked(uint drawable, uint gc, Span<Rectangle> rectangles);

    void PolyArcChecked(uint drawable, uint gc, Span<Arc> arcs);

    void FillPolyChecked(uint drawable, uint gc, PolyShape shape, CoordinateMode coordinate, Span<Point> points);

    void PolyFillRectangleChecked(uint drawable, uint gc, Span<Rectangle> rectangles);

    void PolyFillArcChecked(uint drawable, uint gc, Span<Arc> arcs);

    void PutImageChecked(ImageFormatBitmap format, uint drawable, uint gc, ushort width, ushort height, short x, short y,
        byte leftPad, byte depth, Span<byte> data);

    void ImageText8Checked(uint drawable, uint gc, short x, short y, ReadOnlySpan<byte> text);

    void ImageText16Checked(uint drawable, uint gc, short x, short y, ReadOnlySpan<char> text);

    void CreateColormapChecked(ColormapAlloc alloc, uint colormapId, uint window, uint visual);

    void FreeColormapChecked(uint colormapId);

    void CopyColormapAndFreeChecked(uint colormapId, uint srcColormapId);

    void InstallColormapChecked(uint colormapId);
    void UninstallColormapChecked(uint colormapId);

    void FreeColorsChecked(uint colormapId, uint planeMask, Span<uint> pixels);

    void StoreColorsChecked(uint colormapId, Span<ColorItem> item);

    void StoreNamedColorChecked(ColorFlag mode, uint colormapId, uint pixels, ReadOnlySpan<byte> name);

    void CreateCursorChecked(uint cursorId, uint source, uint mask, ushort foreRed, ushort foreGreen, ushort foreBlue,
        ushort backRed, ushort backGreen, ushort backBlue, ushort x, ushort y);

    void CreateGlyphCursorChecked(uint cursorId, uint sourceFont, uint fontMask, char sourceChar, ushort charMask,
        ushort foreRed, ushort foreGreen, ushort foreBlue, ushort backRed, ushort backGreen, ushort backBlue);

    void FreeCursorChecked(uint cursorId);

    void RecolorCursorChecked(uint cursorId, ushort foreRed, ushort foreGreen, ushort foreBlue, ushort backRed,
        ushort backGreen, ushort backBlue);

    // suppose need changes
    void ChangeKeyboardMappingChecked(byte keycodeCount, byte firstKeycode, byte keysymsPerKeycode, Span<uint> Keysym);

    void BellChecked(sbyte percent);

    void ChangeKeyboardControlChecked(KeyboardControlMask mask, Span<uint> args);

    void ChangePointerControlChecked(Acceleration? acceleration, ushort? threshold);

    void SetScreenSaverChecked(short timeout, short interval, TriState preferBlanking, TriState allowExposures);

    void ForceScreenSaverChecked(ForceScreenSaverMode mode);

    void ChangeHostsChecked(HostMode mode, Family family, Span<byte> address);

    void SetAccessControlChecked(AccessControlMode mode);
    void SetCloseDownModeChecked(CloseDownMode mode);
    void KillClientChecked(uint resource);

    void NoOperationChecked(Span<uint> args);

    void PolyText8Checked(uint drawable, uint gc, ushort x, ushort y, TextItem8[] data);
    void PolyText16Checked(uint drawable, uint gc, ushort x, ushort y, TextItem16[] data);
}