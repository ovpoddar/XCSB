using Xcsb.Connection.Models;
using Xcsb.Models;
using Xcsb.Response.Replies;

namespace Xcsb.Infrastructure.ResponceProto;

public interface IResponseProto
{
    ReplyLease<AllocColorReply> AllocColor(uint colorMap, ushort red, ushort green, ushort blue);
    ReplyLease<QueryPointerReply> QueryPointer(uint window);
    ReplyLease<GrabPointerReply> GrabPointer(bool ownerEvents, uint grabWindow, ushort mask, GrabMode pointerMode,
        GrabMode keyboardMode, uint confineTo, uint cursor, uint timeStamp);
    ReplyLease<InternAtomReply> InternAtom(bool onlyIfExist, string atomName);

    ReplyLease<GetPropertyReply> GetProperty(bool delete, uint window, ATOM property, ATOM type, uint offset,
        uint length);

    ReplyLease<GetWindowAttributesReply> GetWindowAttributes(uint window);
    ReplyLease<GetGeometryReply> GetGeometry(uint drawable);
    ReplyLease<QueryTreeReply> QueryTree(uint window);
    ReplyLease<GetAtomNameReply> GetAtomName(ATOM atom);
    ReplyLease<ListPropertiesReply> ListProperties(uint window);
    ReplyLease<GetSelectionOwnerReply> GetSelectionOwner(ATOM atom);
    ReplyLease<GrabKeyboardReply> GrabKeyboard(bool ownerEvents, uint grabWindow, uint timeStamp, GrabMode pointerMode,
        GrabMode keyboardMode);

    ReplyLease<GetMotionEventsReply> GetMotionEvents(uint window, uint startTime, uint endTime);

    ReplyLease<TranslateCoordinatesReply> TranslateCoordinates(uint srcWindow, uint destinationWindow, ushort srcX,
        ushort srcY);

    ReplyLease<GetInputFocusReply> GetInputFocus();
    ReplyLease<QueryKeymapReply> QueryKeymap();
    ReplyLease<QueryFontReply> QueryFont(uint fontId);
    ReplyLease<QueryTextExtentsReply> QueryTextExtents(uint font, ReadOnlySpan<char> stringForQuery);
    ReplyLease<ListFontsReply> ListFonts(ReadOnlySpan<byte> pattern, int maxNames);
    ListFontsWithInfoReply[] ListFontsWithInfo(ReadOnlySpan<byte> pattan, int maxNames);
    ReplyLease<GetFontPathReply> GetFontPath();
    ReplyLease<GetImageReply> GetImage(ImageFormat format, uint drawable, ushort x, ushort y, ushort width,
        ushort height, uint planeMask);
    ReplyLease<ListInstalledColormapsReply> ListInstalledColormaps(uint window);
    ReplyLease<AllocNamedColorReply> AllocNamedColor(uint colorMap, ReadOnlySpan<byte> name);
    ReplyLease<AllocColorCellsReply> AllocColorCells(bool contiguous, uint colorMap, ushort colors, ushort planes);
    ReplyLease<AllocColorPlanesReply> AllocColorPlanes(bool contiguous, uint colorMap, ushort colors, ushort reds,
        ushort greens, ushort blues);

    ReplyLease<QueryColorsReply> QueryColors(uint colorMap, ReadOnlySpan<uint> pixels);
    ReplyLease<LookupColorReply> LookupColor(uint colorMap, ReadOnlySpan<byte> name);
    ReplyLease<QueryBestSizeReply> QueryBestSize(QueryShapeOf shape, uint drawable, ushort width, ushort height);
    ReplyLease<SetModifierMappingReply> SetModifierMapping(ReadOnlySpan<ulong> keycodes);
    ReplyLease<GetModifierMappingReply> GetModifierMapping();
    ReplyLease<GetKeyboardMappingReply> GetKeyboardMapping(byte firstKeycode, byte count);
    ReplyLease<GetKeyboardControlReply> GetKeyboardControl();
    ReplyLease<SetPointerMappingReply> SetPointerMapping(ReadOnlySpan<byte> maps);
    ReplyLease<GetPointerMappingReply> GetPointerMapping();
    ReplyLease<GetPointerControlReply> GetPointerControl();
    ReplyLease<GetScreenSaverReply> GetScreenSaver();
    ReplyLease<ListHostsReply> ListHosts();
}