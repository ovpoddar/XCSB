﻿using Xcsb.Models;
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

    GetMotionEventsReply GetMotionEvents(uint window, uint startTime, uint endTime);
    TranslateCoordinatesReply TranslateCoordinates(uint srcWindow, uint destinationWindow, ushort srcX, ushort srcY);
    GetInputFocusReply GetInputFocus();
    QueryKeymapReply QueryKeymap();
    void QueryFont(uint fontId);
    QueryTextExtentsReply QueryTextExtents();
    void ListFonts();
    void ListFontsWithInfo();
    void GetFontPath();
    void GetImage();
    void ListInstalledColormaps(uint window);
    AllocNamedColorReply AllocNamedColor();
    AllocColorCellsReply AllocColorCells(bool contiguous, uint colorMap, ushort colors, ushort planes);
    void AllocColorPlanes(bool contiguous, uint colorMap, ushort colors, ushort reds, ushort greens, ushort blues);
    void QueryColors();
    LookupColorReply LookupColor();
    QueryBestSizeReply QueryBestSize(QueryShapeOf shape, uint drawable, ushort width, ushort height);
    QueryExtensionReply QueryExtension();
    void ListExtensions();
    SetModifierMappingReply SetModifierMapping();
    void GetModifierMapping();
    void GetKeyboardMapping();
    GetKeyboardControlReply GetKeyboardControl();
    SetPointerMappingReply SetPointerMapping(Span<byte> maps);
    GetPointerMappingReply GetPointerMapping();
    GetPointerControlReply GetPointerControl();
    GetScreenSaverReply GetScreenSaver();
    ListHostsReply ListHosts();
}