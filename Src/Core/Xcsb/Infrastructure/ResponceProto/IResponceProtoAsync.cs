using Xcsb.Models;
using Xcsb.Response.Replies;

namespace Xcsb.Infrastructure.ResponceProto;

public interface IResponseProtoAsync
{
    Task<AllocColorReply> AllocColorAsync(uint colorMap, ushort red, ushort green, ushort blue, CancellationToken token = default);
    Task<QueryPointerReply> QueryPointerAsync(uint window, CancellationToken token = default);
    Task<GrabPointerReply> GrabPointerAsync(bool ownerEvents, uint grabWindow, ushort mask, GrabMode pointerMode,
        GrabMode keyboardMode, uint confineTo, uint cursor, uint timeStamp, CancellationToken token = default);
    Task<InternAtomReply> InternAtomAsync(bool onlyIfExist, string atomName, CancellationToken token = default);
    Task<GetPropertyReply> GetPropertyAsync(bool delete, uint window, ATOM property, ATOM type, uint offset, uint length, CancellationToken token = default);
    Task<GetWindowAttributesReply> GetWindowAttributesAsync(uint window, CancellationToken token = default);
    Task<GetGeometryReply> GetGeometryAsync(uint drawable, CancellationToken token = default);
    Task<QueryTreeReply> QueryTreeAsync(uint window, CancellationToken token = default);
    Task<GetAtomNameReply> GetAtomNameAsync(ATOM atom, CancellationToken token = default);
    Task<ListPropertiesReply> ListPropertiesAsync(uint window, CancellationToken token = default);
    Task<GetSelectionOwnerReply> GetSelectionOwnerAsync(ATOM atom, CancellationToken token = default);
    Task<GrabKeyboardReply> GrabKeyboardAsync(bool ownerEvents, uint grabWindow, uint timeStamp, GrabMode pointerMode,
        GrabMode keyboardMode, CancellationToken token = default);
    Task<GetMotionEventsReply> GetMotionEventsAsync(uint window, uint startTime, uint endTime, CancellationToken token = default);
    Task<TranslateCoordinatesReply> TranslateCoordinatesAsync(uint srcWindow, uint destinationWindow, ushort srcX, ushort srcY, CancellationToken token = default);
    Task<GetInputFocusReply> GetInputFocusAsync(CancellationToken token = default);
    Task<QueryKeymapReply> QueryKeymapAsync(CancellationToken token = default);
    Task<QueryFontReply> QueryFontAsync(uint fontId, CancellationToken token = default);
    Task<QueryTextExtentsReply> QueryTextExtentsAsync(uint font, string stringForQuery, CancellationToken token = default);
    Task<ListFontsReply> ListFontsAsync(ReadOnlyMemory<byte> pattern, int maxNames, CancellationToken token = default);
    Task<ListFontsWithInfoReply[]> ListFontsWithInfoAsync(ReadOnlyMemory<byte> pattan, int maxNames, CancellationToken token = default);
    Task<GetFontPathReply> GetFontPathAsync(CancellationToken token = default);
    Task<GetImageReply> GetImageAsync(ImageFormat format, uint drawable, ushort x, ushort y, ushort width, ushort height,
        uint planeMask, CancellationToken token = default);
    Task<ListInstalledColormapsReply> ListInstalledColormapsAsync(uint window, CancellationToken token = default);
    Task<AllocNamedColorReply> AllocNamedColorAsync(uint colorMap, ReadOnlyMemory<byte> name, CancellationToken token = default);
    Task<AllocColorCellsReply> AllocColorCellsAsync(bool contiguous, uint colorMap, ushort colors, ushort planes, CancellationToken token = default);

    Task<AllocColorPlanesReply> AllocColorPlanesAsync(bool contiguous, uint colorMap, ushort colors, ushort reds, ushort greens,
        ushort blues, CancellationToken token = default);
    Task<QueryColorsReply> QueryColorsAsync(uint colorMap, ReadOnlyMemory<uint> pixels, CancellationToken token = default);
    Task<LookupColorReply> LookupColorAsync(uint colorMap, ReadOnlyMemory<byte> name, CancellationToken token = default);
    Task<QueryBestSizeReply> QueryBestSizeAsync(QueryShapeOf shape, uint drawable, ushort width, ushort height, CancellationToken token = default);
    Task<SetModifierMappingReply> SetModifierMappingAsync(ReadOnlyMemory<ulong> keycodes, CancellationToken token = default);
    Task<GetModifierMappingReply> GetModifierMappingAsync(CancellationToken token = default);
    Task<GetKeyboardMappingReply> GetKeyboardMappingAsync(byte firstKeycode, byte count, CancellationToken token = default);
    Task<GetKeyboardControlReply> GetKeyboardControlAsync(CancellationToken token = default);
    Task<SetPointerMappingReply> SetPointerMappingAsync(ReadOnlyMemory<byte> maps, CancellationToken token = default);
    Task<GetPointerMappingReply> GetPointerMappingAsync(CancellationToken token = default);
    Task<GetPointerControlReply> GetPointerControlAsync(CancellationToken token = default);
    Task<GetScreenSaverReply> GetScreenSaverAsync(CancellationToken token = default);
    Task<ListHostsReply> ListHostsAsync(CancellationToken token = default);
}