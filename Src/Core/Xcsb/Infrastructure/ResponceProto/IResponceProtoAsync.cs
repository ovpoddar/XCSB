using Xcsb.Connection.Models;
using Xcsb.Models;
using Xcsb.Response.Replies;

namespace Xcsb.Infrastructure.ResponceProto;

public interface IResponseProtoAsync
{
    ValueTask<ReplyLease<AllocColorReply>> AllocColorAsync(uint colorMap, ushort red, ushort green, ushort blue, CancellationToken token = default);
    ValueTask<ReplyLease<QueryPointerReply>> QueryPointerAsync(uint window, CancellationToken token = default);
    ValueTask<ReplyLease<GrabPointerReply>> GrabPointerAsync(bool ownerEvents, uint grabWindow, ushort mask, GrabMode pointerMode,
        GrabMode keyboardMode, uint confineTo, uint cursor, uint timeStamp, CancellationToken token = default);
    ValueTask<ReplyLease<InternAtomReply>> InternAtomAsync(bool onlyIfExist, string atomName, CancellationToken token = default);
    ValueTask<ReplyLease<GetPropertyReply>> GetPropertyAsync(bool delete, uint window, ATOM property, ATOM type, uint offset, uint length, CancellationToken token = default);
    ValueTask<ReplyLease<GetWindowAttributesReply>> GetWindowAttributesAsync(uint window, CancellationToken token = default);
    ValueTask<ReplyLease<GetGeometryReply>> GetGeometryAsync(uint drawable, CancellationToken token = default);
    ValueTask<ReplyLease<QueryTreeReply>> QueryTreeAsync(uint window, CancellationToken token = default);
    ValueTask<ReplyLease<GetAtomNameReply>> GetAtomNameAsync(ATOM atom, CancellationToken token = default);
    ValueTask<ReplyLease<ListPropertiesReply>> ListPropertiesAsync(uint window, CancellationToken token = default);
    ValueTask<ReplyLease<GetSelectionOwnerReply>> GetSelectionOwnerAsync(ATOM atom, CancellationToken token = default);
    ValueTask<ReplyLease<GrabKeyboardReply>> GrabKeyboardAsync(bool ownerEvents, uint grabWindow, uint timeStamp, GrabMode pointerMode,
        GrabMode keyboardMode, CancellationToken token = default);
    ValueTask<ReplyLease<GetMotionEventsReply>> GetMotionEventsAsync(uint window, uint startTime, uint endTime, CancellationToken token = default);
    ValueTask<ReplyLease<TranslateCoordinatesReply>> TranslateCoordinatesAsync(uint srcWindow, uint destinationWindow, ushort srcX, ushort srcY, CancellationToken token = default);
    ValueTask<ReplyLease<GetInputFocusReply>> GetInputFocusAsync(CancellationToken token = default);
    ValueTask<ReplyLease<QueryKeymapReply>> QueryKeymapAsync(CancellationToken token = default);
    ValueTask<ReplyLease<QueryFontReply>> QueryFontAsync(uint fontId, CancellationToken token = default);
    ValueTask<ReplyLease<QueryTextExtentsReply>> QueryTextExtentsAsync(uint font, string stringForQuery, CancellationToken token = default);
    ValueTask<ReplyLease<ListFontsReply>> ListFontsAsync(ReadOnlyMemory<byte> pattern, int maxNames, CancellationToken token = default);
    ValueTask<ReplyLease<ListFontsWithInfoReply>[]> ListFontsWithInfoAsync(ReadOnlyMemory<byte> pattan, int maxNames, CancellationToken token = default);
    ValueTask<ReplyLease<GetFontPathReply>> GetFontPathAsync(CancellationToken token = default);
    ValueTask<ReplyLease<GetImageReply>> GetImageAsync(ImageFormat format, uint drawable, ushort x, ushort y, ushort width, ushort height,
        uint planeMask, CancellationToken token = default);
    ValueTask<ReplyLease<ListInstalledColormapsReply>> ListInstalledColormapsAsync(uint window, CancellationToken token = default);
    ValueTask<ReplyLease<AllocNamedColorReply>> AllocNamedColorAsync(uint colorMap, ReadOnlyMemory<byte> name, CancellationToken token = default);
    ValueTask<ReplyLease<AllocColorCellsReply>> AllocColorCellsAsync(bool contiguous, uint colorMap, ushort colors, ushort planes, CancellationToken token = default);
    
    ValueTask<ReplyLease<AllocColorPlanesReply>> AllocColorPlanesAsync(bool contiguous, uint colorMap, ushort colors, ushort reds, ushort greens,
        ushort blues, CancellationToken token = default);
    ValueTask<ReplyLease<QueryColorsReply>> QueryColorsAsync(uint colorMap, ReadOnlyMemory<uint> pixels, CancellationToken token = default);
    ValueTask<ReplyLease<LookupColorReply>> LookupColorAsync(uint colorMap, ReadOnlyMemory<byte> name, CancellationToken token = default);
    ValueTask<ReplyLease<QueryBestSizeReply>> QueryBestSizeAsync(QueryShapeOf shape, uint drawable, ushort width, ushort height, CancellationToken token = default);
    ValueTask<ReplyLease<SetModifierMappingReply>> SetModifierMappingAsync(ReadOnlyMemory<ulong> keycodes, CancellationToken token = default);
    ValueTask<ReplyLease<GetModifierMappingReply>> GetModifierMappingAsync(CancellationToken token = default);
    ValueTask<ReplyLease<GetKeyboardMappingReply>> GetKeyboardMappingAsync(byte firstKeycode, byte count, CancellationToken token = default);
    ValueTask<ReplyLease<GetKeyboardControlReply>> GetKeyboardControlAsync(CancellationToken token = default);
    ValueTask<ReplyLease<SetPointerMappingReply>> SetPointerMappingAsync(ReadOnlyMemory<byte> maps, CancellationToken token = default);
    ValueTask<ReplyLease<GetPointerMappingReply>> GetPointerMappingAsync(CancellationToken token = default);
    ValueTask<ReplyLease<GetPointerControlReply>> GetPointerControlAsync(CancellationToken token = default);
    ValueTask<ReplyLease<GetScreenSaverReply>> GetScreenSaverAsync(CancellationToken token = default);
    ValueTask<ReplyLease<ListHostsReply>> ListHostsAsync(CancellationToken token = default);
}