using Xcsb.Models;
using Xcsb.Response.Replies;

namespace Xcsb.Infrastructure.ResponceProto;

public interface IResponseProtoAsync
{
    Task<AllocColorReply> AllocColorAsync(uint colorMap, ushort red, ushort green, ushort blue);
    Task<QueryPointerReply> QueryPointerAsync(uint window);
    Task<GrabPointerReply> GrabPointerAsync(bool ownerEvents, uint grabWindow, ushort mask, GrabMode pointerMode,
        GrabMode keyboardMode, uint confineTo, uint cursor, uint timeStamp);
    Task<InternAtomReply> InternAtomAsync(bool onlyIfExist, string atomName);
    Task<GetPropertyReply> GetPropertyAsync(bool delete, uint window, ATOM property, ATOM type, uint offset, uint length);
    Task<GetWindowAttributesReply> GetWindowAttributesAsync(uint window);
    Task<GetGeometryReply> GetGeometryAsync(uint drawable);
    Task<QueryTreeReply> QueryTreeAsync(uint window);
    Task<GetAtomNameReply> GetAtomNameAsync(ATOM atom);
    Task<ListPropertiesReply> ListPropertiesAsync(uint window);
    Task<GetSelectionOwnerReply> GetSelectionOwnerAsync(ATOM atom);
    Task<GrabKeyboardReply> GrabKeyboardAsync(bool ownerEvents, uint grabWindow, uint timeStamp, GrabMode pointerMode,
        GrabMode keyboardMode);
    Task<GetMotionEventsReply> GetMotionEventsAsync(uint window, uint startTime, uint endTime);
    Task<TranslateCoordinatesReply> TranslateCoordinatesAsync(uint srcWindow, uint destinationWindow, ushort srcX, ushort srcY);
    Task<GetInputFocusReply> GetInputFocusAsync();
    Task<QueryKeymapReply> QueryKeymapAsync();
    Task<QueryFontReply> QueryFontAsync(uint fontId);
    Task<QueryTextExtentsReply> QueryTextExtentsAsync(uint font, ReadOnlySpan<char> stringForQuery);
    Task<ListFontsReply> ListFontsAsync(ReadOnlySpan<byte> pattern, int maxNames);
    Task<ListFontsWithInfoReply[]> ListFontsWithInfoAsync(ReadOnlySpan<byte> pattan, int maxNames);
    Task<GetFontPathReply> GetFontPathAsync();
    Task<GetImageReply> GetImageAsync(ImageFormat format, uint drawable, ushort x, ushort y, ushort width, ushort height,
        uint planeMask);
    Task<ListInstalledColormapsReply> ListInstalledColormapsAsync(uint window);
    Task<AllocNamedColorReply> AllocNamedColorAsync(uint colorMap, ReadOnlySpan<byte> name);
    Task<AllocColorCellsReply> AllocColorCellsAsync(bool contiguous, uint colorMap, ushort colors, ushort planes);

    Task<AllocColorPlanesReply> AllocColorPlanesAsync(bool contiguous, uint colorMap, ushort colors, ushort reds, ushort greens,
        ushort blues);
    Task<QueryColorsReply> QueryColorsAsync(uint colorMap, ReadOnlySpan<uint> pixels);
    Task<LookupColorReply> LookupColorAsync(uint colorMap, ReadOnlySpan<byte> name);
    Task<QueryBestSizeReply> QueryBestSizeAsync(QueryShapeOf shape, uint drawable, ushort width, ushort height);
    Task<SetModifierMappingReply> SetModifierMappingAsync(Span<ulong> keycodes);
    Task<GetModifierMappingReply> GetModifierMappingAsync();
    Task<GetKeyboardMappingReply> GetKeyboardMappingAsync(byte firstKeycode, byte count);
    Task<GetKeyboardControlReply> GetKeyboardControlAsync();
    Task<SetPointerMappingReply> SetPointerMappingAsync(Span<byte> maps);
    Task<GetPointerMappingReply> GetPointerMappingAsync();
    Task<GetPointerControlReply> GetPointerControlAsync();
    Task<GetScreenSaverReply> GetScreenSaverAsync();
    Task<ListHostsReply> ListHostsAsync();
}