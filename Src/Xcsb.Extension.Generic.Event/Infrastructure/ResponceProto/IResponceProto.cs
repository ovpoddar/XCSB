using Xcsb.Extension.Generic.Event.Response.Replies;
using Xcsb.Models;

namespace Xcsb.Extension.Generic.Event.Infrastructure.ResponceProto;

public interface IResponseProto
{
    AllocColorReply AllocColor(uint colorMap, ushort red, ushort green, ushort blue);
    QueryPointerReply QueryPointer(uint window);
    GrabPointerReply GrabPointer(bool ownerEvents, uint grabWindow, ushort mask, GrabMode pointerMode,
        GrabMode keyboardMode, uint confineTo, uint cursor, uint timeStamp);
    InternAtomReply InternAtom(bool onlyIfExist, string atomName);
    GetPropertyReply GetProperty(bool delete, uint window, ATOM property, ATOM type, uint offset, uint length);
    GetWindowAttributesReply GetWindowAttributes(uint window);
    GetGeometryReply GetGeometry(uint drawable);
    QueryTreeReply QueryTree(uint window);
    GetAtomNameReply GetAtomName(ATOM atom);
    ListPropertiesReply ListProperties(uint window);
    GetSelectionOwnerReply GetSelectionOwner(ATOM atom);
    GrabKeyboardReply GrabKeyboard(bool ownerEvents, uint grabWindow, uint timeStamp, GrabMode pointerMode,
        GrabMode keyboardMode);
    GetMotionEventsReply GetMotionEvents(uint window, uint startTime, uint endTime);
    TranslateCoordinatesReply TranslateCoordinates(uint srcWindow, uint destinationWindow, ushort srcX, ushort srcY);
    GetInputFocusReply GetInputFocus();
    QueryKeymapReply QueryKeymap();
    QueryFontReply QueryFont(uint fontId);
    QueryTextExtentsReply QueryTextExtents(uint font, ReadOnlySpan<char> stringForQuery);
    ListFontsReply ListFonts(ReadOnlySpan<byte> pattern, int maxNames);
    ListFontsWithInfoReply[] ListFontsWithInfo(ReadOnlySpan<byte> pattan, int maxNames);
    GetFontPathReply GetFontPath();
    GetImageReply GetImage(ImageFormat format, uint drawable, ushort x, ushort y, ushort width, ushort height,
        uint planeMask);
    ListInstalledColormapsReply ListInstalledColormaps(uint window);
    AllocNamedColorReply AllocNamedColor(uint colorMap, ReadOnlySpan<byte> name);
    AllocColorCellsReply AllocColorCells(bool contiguous, uint colorMap, ushort colors, ushort planes);

    AllocColorPlanesReply AllocColorPlanes(bool contiguous, uint colorMap, ushort colors, ushort reds, ushort greens,
        ushort blues);
    QueryColorsReply QueryColors(uint colorMap, Span<uint> pixels);
    LookupColorReply LookupColor(uint colorMap, ReadOnlySpan<byte> name);
    QueryBestSizeReply QueryBestSize(QueryShapeOf shape, uint drawable, ushort width, ushort height);
    QueryExtensionReply QueryExtension(ReadOnlySpan<byte> name);
    ListExtensionsReply ListExtensions();
    SetModifierMappingReply SetModifierMapping(Span<ulong> keycodes);
    GetModifierMappingReply GetModifierMapping();
    GetKeyboardMappingReply GetKeyboardMapping(byte firstKeycode, byte count);
    GetKeyboardControlReply GetKeyboardControl();
    SetPointerMappingReply SetPointerMapping(Span<byte> maps);
    GetPointerMappingReply GetPointerMapping();
    GetPointerControlReply GetPointerControl();
    GetScreenSaverReply GetScreenSaver();
    ListHostsReply ListHosts();
}