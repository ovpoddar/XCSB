using System.Numerics;
using System.Runtime.CompilerServices;
using Xcsb.Masks;
using Xcsb.Models;
using Xcsb.Models.Event;
using Xcsb.Models.Handshake;
using Xcsb.Models.Response;

namespace Xcsb;
public interface IXProto : IDisposable
{
    HandshakeSuccessResponseBody HandshakeSuccessResponseBody { get; }
    ref XEvent GetEvent(Span<byte> scratchBuffer);
    uint NewId();
    void CreateWindow(byte depth,
        uint window,
        uint parent,
        short x,
        short y,
        ushort width,
        ushort height,
        ushort borderWidth,
        ClassType classType,
        uint rootVisualId,
        ValueMask mask,
        params uint[] args);
    void ChangeWindowAttributes(uint window,
        ValueMask mask,
        params uint[] args);
    void GetWindowAttributes();
    void DestroyWindow(uint window);
    void DestroySubwindows(uint window);
    void ChangeSaveSet(ChangeSaveSetMode changeSaveSetMode, uint window);
    void ReparentWindow(uint window, uint parent, short x, short y);
    void MapWindow(uint window);
    void MapSubwindows(uint window);
    void UnmapWindow(uint window);
    void UnmapSubwindows(uint window);
    void ConfigureWindow(uint window, ConfigureValueMask mask, params uint[] args);
    void CirculateWindow(Direction direction, uint window);
    void GetGeometry();
    void QueryTree();
    InternAtomReply InternAtom(bool onlyIfExist, string atomName);
    void GetAtomName();
    void ChangeProperty<T>(PropertyMode mode, uint window, uint property, uint type, params T[] args) where T : struct, INumber<T>;
    void DeleteProperty(uint window, uint atom);
    GetPropertyReply GetProperty(bool delete, uint window, uint property, uint type, uint offset, uint length);
    void RotateProperties(uint window, ushort delta, params uint[] properties);
    void ListProperties();
    void SetSelectionOwner(uint owner, uint atom, uint timestamp);
    void GetSelectionOwner();
    void ConvertSelection(uint requestor, uint selection, uint target, uint property, uint timestamp);
    void SendEvent(bool propagate, uint destination, uint eventMask, XEvent evnt);
    GrabPointerReply GrabPointer(bool ownerEvents, uint grabWindow, ushort mask, GrabMode pointerMode, GrabMode keyboardMode, uint confineTo, uint cursor, uint timeStamp);
    void UngrabPointer(uint time);
    void GrabButton(bool ownerEvents, uint grabWindow, ushort mask, GrabMode pointerMode, GrabMode keyboardMode, uint confineTo, uint cursor, Button button, ModifierMask modifiers);
    void UngrabButton(Button button, uint grabWindow, ModifierMask mask);
    void ChangeActivePointerGrab(uint cursor, uint time, ushort mask);
    void GrabKeyboard();
    void UngrabKeyboard(uint time);
    void GrabKey(bool exposures, uint grabWindow, ModifierMask mask, byte keycode, GrabMode pointerMode, GrabMode keyboardMode);
    void UngrabKey(byte key, uint grabWindow, ModifierMask modifier);
    void AllowEvents(EventsMode mode, uint time);
    void GrabServer();
    void UngrabServer();
    QueryPointerReply QueryPointer(uint window);
    void GetMotionEvents();
    void TranslateCoordinates();
    void WarpPointer(uint srcWindow, uint destWindow, short srcX, short srcY, ushort srcWidth, ushort srcHeight, short destX, short destY);
    void SetInputFocus(InputFocusMode mode, uint focus, uint time);
    void GetInputFocus();
    void QueryKeymap();
    void OpenFont(string fontName, uint fontId);
    void CloseFont(uint fontId);
    void QueryFont();
    void QueryTextExtents();
    void ListFonts();
    void ListFontsWithInfo();
    void SetFontPath(string[] strPaths);
    void GetFontPath();
    void CreatePixmap(byte depth, uint pixmapId, uint drawable, ushort width, ushort height);
    void FreePixmap(uint pixmapId);
    void CreateGC(uint gc, uint drawable, GCMask mask, params uint[] args);
    void ChangeGC(uint gc, GCMask mask, params uint[] args);
    void CopyGC(uint srcGc, uint dstGc, GCMask mask);
    void SetDashes(uint gc, ushort dashOffset, byte[] dashes);
    void SetClipRectangles(ClipOrdering ordering, uint gc, ushort clipX, ushort clipY, Rectangle[] rectangles);
    void FreeGC(uint gc);
    void ClearArea(bool exposures, uint window, short x, short y, ushort width, ushort height);
    void CopyArea(
        uint srcDrawable,
        uint destDrawable,
        uint gc,
        ushort srcX,
        ushort srcY,
        ushort destX,
        ushort destY,
        ushort width,
        ushort height);
    void CopyPlane(
        uint srcDrawable,
        uint destDrawable,
        uint gc,
        ushort srcX,
        ushort srcY,
        ushort destX,
        ushort destY,
        ushort width,
        ushort height,
        uint bitPlane);
    void PolyPoint(CoordinateMode coordinate, uint drawable, uint gc, Point[] points);
    void PolyLine(CoordinateMode coordinate, uint drawable, uint gc, Point[] points);
    void PolySegment(uint drawable, uint gc, Segment[] segments);
    void PolyRectangle(uint drawable, uint gc, Rectangle[] rectangles);
    void PolyArc(uint drawable, uint gc, Arc[] arcs);
    void FillPoly(uint drawable, uint gc, PolyShape shape, CoordinateMode coordinate, Point[] points);
    void PolyFillRectangle(uint drawable, uint gc, Rectangle[] rectangles);
    void PolyFillArc(uint drawable, uint gc, Arc[] arcs);
    void PutImage(ImageFormat format,
        uint drawable,
        uint gc,
        ushort width,
        ushort height,
        short x,
        short y,
        byte leftPad,
        byte depth,
        Span<byte> data);
    void GetImage();
    void PolyText8();  //todo:imp
    void PolyText16();  //todo:imp
    void ImageText8(uint drawable, uint gc, short x, short y, ReadOnlySpan<byte> text);
    void ImageText16(uint drawable, uint gc, short x, short y, ReadOnlySpan<char> text);
    void CreateColormap(ColormapAlloc alloc, uint colormapId, uint window, uint visual);
    void FreeColormap(uint colormapId);
    void CopyColormapAndFree(uint colormapId, uint srcColormapId);
    void InstallColormap(uint colormapId);
    void UninstallColormap(uint colormapId);
    void ListInstalledColormaps();
    AllocColorReply AllocColor(uint colorMap, ushort red, ushort green, ushort blue);
    void AllocNamedColor();
    void AllocColorCells();
    void AllocColorPlanes();
    void FreeColors(uint colormapId, uint planeMask, params uint[] pixels);
    void StoreColors(uint colormapId, params ColorItem[] item);
    void StoreNamedColor(ColorFlag mode, uint colormapId, uint pixels, ReadOnlySpan<byte> name);
    void QueryColors();
    void LookupColor();
    void CreateCursor();
    void CreateGlyphCursor(uint cursorId, uint sourceFont, uint fontMask, char sourceChar, ushort charMask, ushort foreRed, ushort foreGreen, ushort foreBlue, ushort backRed, ushort backGreen, ushort backBlue);
    void FreeCursor(uint cursorId);
    void RecolorCursor(uint cursorId, ushort foreRed, ushort foreGreen, ushort foreBlue, ushort backRed, ushort backGreen, ushort backBlue);
    void QueryBestSize();
    void QueryExtension();
    void ListExtensions();
    void SetModifierMapping();
    void GetModifierMapping();
    // suppose need changes
    void ChangeKeyboardMapping(byte keycodeCount, byte firstKeycode, byte keysymsPerKeycode, uint[] Keysym);
    void GetKeyboardMapping();
    void ChangeKeyboardControl(KeyboardControlMask mask, params uint[] args);
    void GetKeyboardControl();
    void Bell(sbyte percent);
    void SetPointerMapping();
    void GetPointerMapping();
    void ChangePointerControl(Acceleration acceleration, ushort? threshold);
    void GetPointerControl();
    void SetScreenSaver(short timeout, short interval, TriState preferBlanking, TriState allowExposures);
    void GetScreenSaver();
    void ForceScreenSaver(ForceScreenSaverMode mode);
    void ChangeHosts(HostMode mode, Family family, byte[] address);
    void ListHosts();
    void SetAccessControl(AccessControlMode mode);
    void SetCloseDownMode(CloseDownMode mode);
    void KillClient(uint resource);
    void NoOperation(params uint[] args);
}

