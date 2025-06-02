using System.Numerics;
using Xcsb.Masks;
using Xcsb.Models;
using Xcsb.Models.Event;
using Xcsb.Models.Handshake;

namespace Xcsb;
public interface IXProto : IDisposable
{
    HandshakeSuccessResponseBody HandshakeSuccessResponseBody { get; }
    ref XEvent GetEvent(Span<byte> scratchBuffer);

    uint NewId();
    void CreateWindow(uint window,
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
    void GetAtomName();
    void ChangeProperty<T>(PropertyMode mode, uint window, uint property, uint type, params T[] args) where T : struct, INumber<T>;
    void DeleteProperty(uint window, uint atom);
    void GetProperty();
    void RotateProperties(uint window, ushort delta, params uint[] properties);
    void ListProperties();
    void SetSelectionOwner(uint owner, uint atom, uint timestamp);
    void GetSelectionOwner();
    void ConvertSelection(uint requestor, uint selection, uint target, uint property, uint timestamp);
    void SendEvent(bool propagate, uint destination, uint eventMask, XEvent evnt);
    void GrabPointer();
    void UngrabPointer(uint time);
    void GrabButton();
    void UngrabButton();
    void ChangeActivePointerGrab(uint cursor, uint time, ushort mask);
    void GrabKeyboard();
    void UngrabKeyboard();
    void GrabKey();
    void UngrabKey();
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
    void SetFontPath();
    void GetFontPath();
    void CreatePixmap();
    void FreePixmap();
    void CreateGC(uint gc, uint drawable, GCMask mask, params uint[] args);
    void ChangeGC(uint gc, GCMask mask, params uint[] args);
    void CopyGC(uint srcGc, uint dstGc, GCMask mask);
    void SetDashes();
    void SetClipRectangles();
    void FreeGC(uint gc);
    void ClearArea();
    void CopyArea();
    void CopyPlane();
    void PolyPoint();
    void PolyLine();
    void PolySegment();
    void PolyRectangle();
    void PolyArc();
    void FillPoly();
    void PolyFillRectangle();
    void PolyFillArc();
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
    void PolyText8();
    void PolyText16();
    void ImageText8(uint drawable, uint gc, short x, short y, ReadOnlySpan<byte> text);
    void ImageText16(uint drawable, uint gc, short x, short y, ReadOnlySpan<char> text);
    void CreateColormap(ColormapAlloc alloc, uint colormapId, uint window, uint visual);
    void FreeColormap(uint colormapId);
    void CopyColormapAndFree(uint colormapId, uint srcColormapId);
    void InstallColormap(uint colormapId);
    void UninstallColormap(uint colormapId);
    void ListInstalledColormaps();
    void AllocColor();
    void AllocNamedColor();
    void AllocColorCells();
    void AllocColorPlanes();
    void FreeColors(uint colormapId, uint planeMask, params uint[] pixels);
    void StoreColors(uint colormapId, params ColorItem[] item);
    void StoreNamedColor(ColorFlag mode, uint colormapId, uint pixels, ReadOnlySpan<byte> name);
    void QueryColors();
    void LookupColor();
    void CreateCursor();
    void CreateGlyphCursor();
    void FreeCursor(uint cursorId);
    void RecolorCursor();
    void QueryBestSize();
    void QueryExtension();
    void ListExtensions();
    void SetModifierMapping();
    void GetModifierMapping();
    void ChangeKeyboardMapping();
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
    void ChangeHosts();
    void ListHosts();
    void SetAccessControl(AccessControlMode mode);
    void SetCloseDownMode(CloseDownMode mode);
    void KillClient(uint resource);
    void NoOperation(params uint[] args);
}

