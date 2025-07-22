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
    GetAtomNameReply GetAtomName(uint atom);
    ListPropertiesReply ListProperties(uint window);
    GetSelectionOwnerReply GetSelectionOwner(uint atom);
    GrabKeyboardReply GrabKeyboard(bool ownerEvents, uint grabWindow, uint timeStamp, GrabMode pointerMode,
        GrabMode keyboardMode);
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
    SetPointerMappingReply SetPointerMapping(Span<byte> maps);
    GetPointerMappingReply GetPointerMapping();
    GetPointerControlReply GetPointerControl();
    GetScreenSaverReply GetScreenSaver();
    ListHostsReply ListHosts();
}