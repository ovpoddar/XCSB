using Xcsb.Models;
using Xcsb.Models.Response;

namespace Xcsb;

public interface IResponseProto
{
    AllocColorReply AllocColor(uint colorMap, ushort red, ushort green, ushort blue);
    QueryPointerReply QueryPointer(uint window);

    GrabPointerReply GrabPointer(bool ownerEvents, uint grabWindow, ushort mask, GrabMode pointerMode,
        GrabMode keyboardMode, uint confineTo, uint cursor, uint timeStamp);

    InternAtomReply InternAtom(bool onlyIfExist, string atomName);
    GetPropertyReply GetProperty(bool delete, uint window, uint property, uint type, uint offset, uint length);
    GetWindowAttributesReply GetWindowAttributes(uint window);
    GetGeometryReply GetGeometry(uint drawable);
    QueryTreeReply QueryTree(uint window);
    void GetAtomName();
    void ListProperties();
    void GetSelectionOwner();
    void GrabKeyboard();
    void GetMotionEvents();
    void TranslateCoordinates();
    void GetInputFocus();
    void QueryKeymap();
    void QueryFont();
    void QueryTextExtents();
    void ListFonts();
    void ListFontsWithInfo();
    void GetFontPath();
    void GetImage();
    void ListInstalledColormaps();
    void AllocNamedColor();
    void AllocColorCells();
    void AllocColorPlanes();
    void QueryColors();
    void LookupColor();
    void QueryBestSize();
    void QueryExtension();
    void ListExtensions();
    void SetModifierMapping();
    void GetModifierMapping();
    void GetKeyboardMapping();
    void GetKeyboardControl();
    void SetPointerMapping();
    void GetPointerMapping();
    void GetPointerControl();
    void GetScreenSaver();
    void ListHosts();
}