using Src.Models;
using Src.Models.Event;
using Src.Models.Handshake;

namespace Src;
public interface IXProto : IDisposable
{
    HandshakeSuccessResponseBody HandshakeSuccessResponseBody { get; }

    XEvent GetEvent();

    int NewId();
    void CreateWindow(int window,
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
    void DestroyWindow();
    void DestroySubwindows();
    void ChangeSaveSet();
    void ReparentWindow();
    void MapWindow(uint window);
    void MapSubwindows();
    void UnmapWindow();
    void UnmapSubwindows();
    void ConfigureWindow();
    void CirculateWindow();
    void GetGeometry();
    void QueryTree();
    void InternAtom();
    void GetAtomName();
    void ChangeProperty();
    void DeleteProperty();
    void GetProperty();
    void RotateProperties();
    void ListProperties();
    void SetSelectionOwner();
    void GetSelectionOwner();
    void ConvertSelection();
    void SendEvent();
    void GrabPointer();
    void UngrabPointer();
    void GrabButton();
    void UngrabButton();
    void ChangeActivePointerGrab();
    void GrabKeyboard();
    void UngrabKeyboard();
    void GrabKey();
    void UngrabKey();
    void AllowEvents();
    void GrabServer();
    void UngrabServer();
    void QueryPointer();
    void GetMotionEvents();
    void TranslateCoordinates();
    void WarpPointer();
    void SetInputFocus();
    void GetInputFocus();
    void QueryKeymap();
    void OpenFont();
    void CloseFont();
    void QueryFont();
    void QueryTextExtents();
    void ListFonts();
    void ListFontsWithInfo();
    void SetFontPath();
    void GetFontPath();
    void CreatePixmap();
    void FreePixmap();
    void CreateGC();
    void ChangeGC();
    void CopyGC();
    void SetDashes();
    void SetClipRectangles();
    void FreeGC();
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
    void PutImage();
    void GetImage();
    void PolyText8();
    void PolyText16();
    void ImageText8();
    void ImageText16();
    void CreateColormap();
    void FreeColormap();
    void CopyColormapAndFree();
    void InstallColormap();
    void UninstallColormap();
    void ListInstalledColormaps();
    void AllocColor();
    void AllocNamedColor();
    void AllocColorCells();
    void AllocColorPlanes();
    void FreeColors();
    void StoreColors();
    void StoreNamedColor();
    void QueryColors();
    void LookupColor();
    void CreateCursor();
    void CreateGlyphCursor();
    void FreeCursor();
    void RecolorCursor();
    void QueryBestSize();
    void QueryExtension();
    void ListExtensions();
    void SetModifierMapping();
    void GetModifierMapping();
    void ChangeKeyboardMapping();
    void GetKeyboardMapping();
    void ChangeKeyboardControl();
    void GetKeyboardControl();
    void Bell();
    void SetPointerMapping();
    void GetPointerMapping();
    void ChangePointerControl();
    void GetPointerControl();
    void SetScreenSaver();
    void GetScreenSaver();
    void ForceScreenSaver();
    void ChangeHosts();
    void ListHosts();
    void SetAccessControl();
    void SetCloseDownMode();
    void KillClient();
    void NoOperation();
}

