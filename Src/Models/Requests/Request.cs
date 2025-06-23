﻿using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Helpers;
using Xcsb.Masks;
using Xcsb.Models.Event;

namespace Xcsb.Models.Requests;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 4)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct BellType(sbyte percent)
{
    public readonly Opcode Opcode = Opcode.Bell;
    public readonly sbyte Percent = percent;
    public readonly ushort Length = 1;
}

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 4)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct ForceScreenSaverType(ForceScreenSaverMode mode)
{
    public readonly Opcode Opcode = Opcode.ForceScreenSaver;
    public readonly ForceScreenSaverMode Mode = mode;
    public readonly ushort Length = 1;
}

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 4)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct SetAccessControlType(AccessControlMode mode)
{
    public readonly Opcode Opcode = Opcode.SetAccessControl;
    public readonly AccessControlMode Mode = mode;
    public readonly ushort Length = 1;
}

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 4)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct SetCloseDownModeType(CloseDownMode mode)
{
    public readonly Opcode Opcode = Opcode.SetCloseDownMode;
    public readonly CloseDownMode Mode = mode;
    public readonly ushort Length = 1;
}

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 4)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct GrabServerType()
{
    public readonly Opcode Opcode = Opcode.GrabServer;
    private readonly byte _pad0;
    public readonly ushort Length = 1;
}

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 4)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct UnGrabServerType()
{
    public readonly Opcode Opcode = Opcode.UngrabServer;
    private readonly byte _pad0;
    public readonly ushort Length = 1;
}

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 8)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct QueryPointerType(uint window)
{
    public readonly Opcode Opcode = Opcode.QueryPointer;
    private readonly byte _pad0;
    public readonly ushort Length = 2;
    public readonly uint Window = window;
}

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 8)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct UngrabKeyboardType(uint time)
{
    public readonly Opcode Opcode = Opcode.UngrabKeyboard;
    private readonly byte _pad0;
    public readonly ushort Length = 2;
    public readonly uint Time = time;
}

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 8)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct CloseFontType(uint fontId)
{
    public readonly Opcode Opcode = Opcode.CloseFont;
    private readonly byte _pad0;
    public readonly ushort Length = 2;
    public readonly uint FontId = fontId;
}

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 8)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct UninstallColormapType(uint colormapId)
{
    public readonly Opcode Opcode = Opcode.UninstallColormap;
    private readonly byte _pad0;
    public readonly ushort Length = 2;
    public readonly uint ColorMapId = colormapId;
}

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 8)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct InstallColormapType(uint colormapId)
{
    public readonly Opcode Opcode = Opcode.InstallColormap;
    private readonly byte _pad0;
    public readonly ushort Length = 2;
    public readonly uint ColorMapId = colormapId;
}

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 8)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct FreePixmapType(uint pixmapId)
{
    public readonly Opcode Opcode = Opcode.FreePixmap;
    private readonly byte _pad0;
    public readonly ushort Length = 2;
    public readonly uint PixmapId = pixmapId;
}

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 8)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct KillClientType(uint resource)
{
    public readonly Opcode Opcode = Opcode.KillClient;
    private readonly byte _pad0;
    public readonly ushort Length = 2;
    public readonly uint Resource = resource;
}

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 8)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct FreeColormapType(uint colormapId)
{
    public readonly Opcode Opcode = Opcode.FreeColormap;
    private readonly byte _pad0;
    public readonly ushort Length = 2;
    public readonly uint ColorMapId = colormapId;
}

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 8)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct DestroyWindowType(uint window)
{
    public readonly Opcode Opcode = Opcode.DestroyWindow;
    private readonly byte _pad0;
    public readonly ushort Length = 2;
    public readonly uint Window = window;
}

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 8)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct MapWindowType(uint window)
{
    public readonly Opcode OpCode = Opcode.MapWindow;
    private readonly byte _pad0;
    public readonly ushort Length = 2;
    public readonly uint Window = window;
}

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 8)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct UnmapWindowType(uint window)
{
    public readonly Opcode OpCode = Opcode.UnmapWindow;
    private readonly byte _pad0;
    public readonly ushort Length = 2;
    public readonly uint Window = window;
}

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 8)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct DestroySubWindowsType(uint window)
{
    public readonly Opcode OpCode = Opcode.DestroySubwindows;
    private readonly byte _pad0;
    public readonly ushort Length = 2;
    public readonly uint Window = window;
}

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 8)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct MapSubWindowsType(uint window)
{
    public readonly Opcode OpCode = Opcode.MapSubwindows;
    private readonly byte _pad0;
    public readonly ushort Length = 2;
    public readonly uint Window = window;
}

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 8)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct UnMapSubwindowsType(uint window)
{
    public readonly Opcode OpCode = Opcode.UnmapSubwindows;
    private readonly byte _pad0;
    public readonly ushort Length = 2;
    public readonly uint Window = window;
}

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 8)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct StoreColorsType(uint colormapId, int itemLength)
{
    public readonly Opcode OpCode = Opcode.StoreColors;
    private readonly byte _pad0;
    public readonly ushort Length = (ushort)(2 + (itemLength / 3));
    public readonly uint ColorMapId = colormapId;
}

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 8)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct SetFontPathType(ushort itemsLength, int requestLength)
{
    public readonly Opcode OpCode = Opcode.SetFontPath;
    private readonly byte _pad0;
    public readonly ushort Length = (ushort)(2 + (requestLength / 4));
    public readonly ushort ItemsLength = itemsLength;
}

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 8)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct CirculateWindowType(Direction direction, uint window)
{
    public readonly Opcode OpCode = Opcode.CirculateWindow;
    public readonly Direction Direction = direction;
    public readonly ushort Length = 2;
    public readonly uint Window = window;
}

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 8)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct ChangeKeyboardControlType(KeyboardControlMask mask, int argsLength)
{
    public readonly Opcode OpCode = Opcode.ChangeKeyboardControl;
    private readonly byte _pad0;
    public readonly ushort Length = (ushort)(2 + argsLength);
    public readonly KeyboardControlMask Mask = mask;
}

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 8)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct FreeGCType(uint gc)
{
    public readonly Opcode OpCode = Opcode.FreeGC;
    private readonly byte _pad0;
    public readonly ushort Length = 2;
    public readonly uint GC = gc;
}

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 8)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct FreeCursorType(uint cursorId)
{
    public readonly Opcode OpCode = Opcode.FreeCursor;
    private readonly byte _pad0;
    public readonly ushort Length = 2;
    public readonly uint CursorId = cursorId;
}

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 8)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct AllowEventsType(EventsMode mode, uint time)
{
    public readonly Opcode OpCode = Opcode.AllowEvents;
    public readonly EventsMode Mode = mode;
    public readonly ushort Length = 2;
    public readonly uint Time = time;
}

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 8)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct UngrabPointerType(uint time)
{
    public readonly Opcode OpCode = Opcode.UngrabPointer;
    private readonly byte _pad0;
    public readonly ushort Length = 2;
    public readonly uint Time = time;
}

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 8)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct ChangeSaveSetType(ChangeSaveSetMode changeSaveSetMode, uint window)
{
    public readonly Opcode OpCode = Opcode.ChangeSaveSet;
    public readonly ChangeSaveSetMode Mode = changeSaveSetMode;
    public readonly ushort Length = 2;
    public readonly uint Window = window;
}

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 8)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct ChangeHostsType(HostMode mode, Family family, int addressLength)
{
    public readonly Opcode OpCode = Opcode.ChangeHosts;
    public readonly HostMode Mode = mode;
    public readonly ushort Length = (ushort)(2 + (addressLength.AddPadding() / 4));
    public readonly Family Family = family;
    private readonly byte _pad0;
    public readonly ushort AddressLength = (ushort)addressLength;
}

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 8)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct ChangeKeyboardMappingType(byte keycodeCount, byte firstKeycode, byte keysymsPerKeycode)
{
    public readonly Opcode OpCode = Opcode.ChangeKeyboardMapping;
    public readonly byte KeycodeCount = keycodeCount;
    public readonly ushort Length = (ushort)(2 + (keycodeCount * keysymsPerKeycode));
    public readonly byte FirstKeycode = firstKeycode;
    public readonly byte KeysymsPerKeycode = keysymsPerKeycode;
}

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 8)]
internal readonly struct InternAtomType(bool onlyIfExist, int atomNameLength)
{
    public readonly Opcode Opcode = Opcode.InternAtom;
    public readonly byte OnlyIfExists = (byte)(onlyIfExist ? 1 : 0);
    public readonly ushort Length = (ushort)(2 + (atomNameLength.AddPadding() / 4));
    public readonly ushort NameLength = (ushort)atomNameLength;
    private readonly ushort _pad0;
}

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 12)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct HandShakeRequestType(ushort nameLength, ushort dataLength,
    ushort majorVersion = 11, ushort minorVersion = 0)
{
    public readonly byte ByteOrder = (byte)(BitConverter.IsLittleEndian ? 'l' : 'B');
    private readonly byte _pad0;
    public readonly ushort MajorVersion = majorVersion;
    public readonly ushort MinorVersion = minorVersion;
    public readonly ushort NameLength = nameLength;
    public readonly ushort DataLength = dataLength;
    private readonly ushort _pad1;
}

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 12)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct OpenFontType(uint fontId, ushort fontLength)
{
    public readonly Opcode opcode = Opcode.OpenFont;
    private readonly byte _pad0;
    public readonly ushort Length = (ushort)(3 + (fontLength.AddPadding() / 4));
    public readonly uint FontId = fontId;
    public readonly ushort FontLength = fontLength;
    private readonly ushort _pad1;
}

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 12)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct DeletePropertyType(uint window, uint atom)
{
    public readonly Opcode opcode = Opcode.DeleteProperty;
    private readonly byte _pad0;
    public readonly ushort Length = 3;
    public readonly uint Window = window;
    public readonly uint Atom = atom;
}

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 12)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct CopyColormapAndFreeType(uint colormapId, uint srcColormapId)
{
    public readonly Opcode opcode = Opcode.CopyColormapAndFree;
    private readonly byte _pad0;
    public readonly ushort Length = 3;
    public readonly uint ColorMapId = colormapId;
    public readonly uint SourceColorMapId = srcColormapId;
}

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 12)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct SetDashesType(uint gc, ushort dashOffset, int dashLength)
{
    public readonly Opcode opcode = Opcode.SetDashes;
    private readonly byte _pad0;
    public readonly ushort Length = (ushort)(3 + (dashLength.AddPadding() / 4));
    public readonly uint GContext = gc;
    public readonly ushort DashOffset = dashOffset;
    public readonly ushort DashLength = (ushort)dashLength;
}

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 12)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct ConfigureWindowType(uint window, ConfigureValueMask mask, int argsLength)
{
    public readonly Opcode opcode = Opcode.ConfigureWindow;
    private readonly byte _pad0;
    public readonly ushort Length = (ushort)(3 + argsLength);
    public readonly uint Window = window;
    public readonly ConfigureValueMask Mask = mask;
    private readonly ushort _pad1;
}

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 12)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct SetInputFocusType(InputFocusMode mode, uint focus, uint time)
{
    public readonly Opcode opcode = Opcode.SetInputFocus;
    public readonly InputFocusMode Mode = mode;
    public readonly ushort Length = 3;
    public readonly uint Focus = focus;
    public readonly uint Time = time;
}

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 12)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct SetClipRectanglesType(ClipOrdering ordering, uint gc, ushort clipX, ushort clipY, int rectanglesLength)
{
    public readonly Opcode opcode = Opcode.SetClipRectangles;
    public readonly ClipOrdering ordering = ordering;
    public readonly ushort Length = (ushort)(3 + (2 * rectanglesLength));
    public readonly uint Gc = gc;
    public readonly ushort ClipX = clipX;
    public readonly ushort ClipY = clipY;
}

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 12)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct PolyPointType(CoordinateMode coordinate, uint drawable, uint gc, int pointsLength)
{
    public readonly Opcode opcode = Opcode.PolyPoint;
    public readonly CoordinateMode Coordinate = coordinate;
    public readonly ushort Length = (ushort)(3 + pointsLength);
    public readonly uint Drawable = drawable;
    public readonly uint Gc = gc;
}

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 12)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct PolyLineType(CoordinateMode coordinate, uint drawable, uint gc, int pointsLength)
{
    public readonly Opcode opcode = Opcode.PolyLine;
    public readonly CoordinateMode Coordinate = coordinate;
    public readonly ushort Length = (ushort)(3 + pointsLength);
    public readonly uint Drawable = drawable;
    public readonly uint Gc = gc;
}

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 12)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct ChangeGCType(uint gc, GCMask mask, int argsLength)
{
    public readonly Opcode opcode = Opcode.ChangeGC;
    private readonly byte _pad0;
    public readonly ushort Length = (ushort)(3 + argsLength);
    public readonly uint Gc = gc;
    public readonly GCMask Mask = mask;
}

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 12)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct PolyRectangleType(uint drawable, uint gc, int rectanglesLength)
{
    public readonly Opcode opcode = Opcode.PolyRectangle;
    private readonly byte _pad0;
    public readonly ushort Length = (ushort)(3 + (rectanglesLength * 2));
    public readonly uint Drawable = drawable;
    public readonly uint Gc = gc;
}

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 12)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct ChangeWindowAttributesType(uint window, ValueMask mask, int argsLength)
{
    public readonly Opcode opcode = Opcode.ChangeWindowAttributes;
    private readonly byte _pad0;
    public readonly ushort Length = (ushort)(3 + argsLength);
    public readonly uint Window = window;
    public readonly ValueMask Mask = mask;
}

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 12)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct PolyFillRectangleType(uint drawable, uint gc, int rectanglesLength)
{
    public readonly Opcode opcode = Opcode.PolyFillRectangle;
    private readonly byte _pad0;
    public readonly ushort Length = (ushort)(3 + (2 * rectanglesLength));
    public readonly uint Drawable = drawable;
    public readonly uint GC = gc;
}

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 12)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct PolySegmentType(uint drawable, uint gc, int segmentsLength)
{
    public readonly Opcode opcode = Opcode.PolySegment;
    private readonly byte _pad0;
    public readonly ushort Length = (ushort)(3 + (2 * segmentsLength));
    public readonly uint Drawable = drawable;
    public readonly uint GC = gc;
}

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 12)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct PolyArcType(uint drawable, uint gc, int arcLength)
{
    public readonly Opcode opcode = Opcode.PolyArc;
    private readonly byte _pad0;
    public readonly ushort Length = (ushort)(3 + (3 * arcLength));
    public readonly uint Drawable = drawable;
    public readonly uint GC = gc;
}

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 12)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct PolyFillArcType(uint drawable, uint gc, int arcLength)
{
    public readonly Opcode opcode = Opcode.PolyFillArc;
    private readonly byte _pad0;
    public readonly ushort Length = (ushort)(3 + (3 * arcLength));
    public readonly uint Drawable = drawable;
    public readonly uint GC = gc;
}

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 12)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct FreeColorsType(uint colormapId, uint planeMask, int pixelsLength)
{
    public readonly Opcode opcode = Opcode.FreeColors;
    private readonly byte _pad0;
    public readonly ushort Length = (ushort)(3 + pixelsLength);
    public readonly uint ColorMapId = colormapId;
    public readonly uint PlaneMask = planeMask;
}

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 12)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct UngrabButtonType(Button button, uint grabWindow, ModifierMask modifier)
{
    public readonly Opcode opcode = Opcode.UngrabButton;
    public readonly Button Button = button;
    public readonly ushort Length = 3;
    public readonly uint GrabWindow = grabWindow;
    public readonly ModifierMask Modifier = modifier;
}

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 12)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct UngrabKeyType(byte key, uint grabWindow, ModifierMask modifier)
{
    public readonly Opcode opcode = Opcode.UngrabKey;
    public readonly byte Key = key;
    public readonly ushort Length = 3;
    public readonly uint GrabWindow = grabWindow;
    public readonly ModifierMask Modifier = modifier;
}

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 12)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct RotatePropertiesType(uint window, int propertiesLength, ushort delta)
{
    public readonly Opcode opcode = Opcode.RotateProperties;
    private readonly byte _pad0;
    public readonly ushort Length = (ushort)(3 + propertiesLength);
    public readonly uint Window = window;
    public readonly ushort PropertiesLength = (ushort)propertiesLength;
    public readonly ushort Delta = delta;
}

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 12)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct ChangePointerControlType(ushort accelerationNumerator,
    ushort accelerationDenominator,
    ushort threshold,
    byte doAcceleration,
    byte doThreshold
    )
{
    public readonly Opcode opcode = Opcode.ChangePointerControl;
    private readonly byte _pad0;
    public readonly ushort Length = 3;
    public readonly ushort AccelerationNumerator = accelerationNumerator;
    public readonly ushort AccelerationDenominator = accelerationDenominator;
    public readonly ushort Threshold = threshold;
    public readonly byte DoAcceleration = doAcceleration;
    public readonly byte DoThreshold = doThreshold;
}

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 12)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct SetScreenSaverType(short timeout, short interval, TriState preferBlanking, TriState allowExposures)
{
    public readonly Opcode OpCode = Opcode.SetScreenSaver;
    private readonly byte _pad0;
    public readonly ushort Length = 3;
    public readonly short TimeOut = timeout;
    public readonly short Interval = interval;
    public readonly TriState PreferBlanking = preferBlanking;
    public readonly TriState AllowExposures = allowExposures;
}

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 16)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct CreateGCType(uint gc, uint drawable, GCMask mask, int argsLength)
{
    public readonly Opcode OpCode = Opcode.CreateGC;
    private readonly byte _pad0;
    public readonly ushort Length = (ushort)(4 + argsLength);
    public readonly uint Gc = gc;
    public readonly uint Drawable = drawable;
    public readonly GCMask Mask = mask;
}

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 16)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct ClearAreaType(bool exposures, uint window, short x, short y, ushort width, ushort height)
{
    public readonly Opcode OpCode = Opcode.ClearArea;
    public readonly byte Exposures = (byte)(exposures ? 1 : 0);
    public readonly ushort Length = 4;
    public readonly uint Window = window;
    public readonly short X = x;
    public readonly short Y = y;
    public readonly ushort Width = width;
    public readonly ushort Height = height;
}

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 16)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct FillPolyType(uint drawable, uint gc, PolyShape shape, CoordinateMode coordinate, int pointsLength)
{
    public readonly Opcode opcode = Opcode.FillPoly;
    private readonly byte _pad0;
    public readonly ushort Length = (ushort)(4 + pointsLength);
    public readonly uint Drawable = drawable;
    public readonly uint Gc = gc;
    public readonly PolyShape Shape = shape;
    public readonly CoordinateMode Coordinate = coordinate;
}

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 16)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct GrabKeyType(bool exposures, uint grabWindow, ModifierMask mask, byte keycode, GrabMode pointerMode, GrabMode keyboardMode)
{
    public readonly Opcode OpCode = Opcode.GrabKey;
    public readonly byte Exposures = (byte)(exposures ? 1 : 0);
    public readonly ushort Length = 4;
    public readonly uint GrabWindow = grabWindow;
    public readonly ModifierMask ModifierMask = mask;
    public readonly byte KeyCode = keycode;
    public readonly GrabMode PointerMode = pointerMode;
    public readonly GrabMode KeyboardMode = keyboardMode;
}

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 16)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct AllocColorType(uint colorMap, ushort red, ushort green, ushort blue)
{
    public readonly Opcode OpCode = Opcode.AllocColor;
    private readonly byte _pad0;
    public readonly ushort Length = 4;
    public readonly uint ColorMap = colorMap;
    public readonly ushort Red = red;
    public readonly ushort Green = green;
    public readonly ushort Blue = blue;
}

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 16)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct CreateColormapType(ColormapAlloc alloc, uint colormapId, uint window, uint visual)
{
    public readonly Opcode OpCode = Opcode.CreateColormap;
    public readonly ColormapAlloc Alloc = alloc;
    public readonly ushort Length = 4;
    public readonly uint ColorMapId = colormapId;
    public readonly uint Window = window;
    public readonly uint Visual = visual;
}

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 16)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct StoreNamedColorType(ColorFlag mode, uint colormapId, uint pixels, int nameLength)
{
    public readonly Opcode OpCode = Opcode.StoreNamedColor;
    public readonly ColorFlag Mode = mode;
    public readonly ushort Length = (ushort)(4 + (nameLength.AddPadding() / 4));
    public readonly uint ColorMapId = colormapId;
    public readonly uint Pixels = pixels;
    public readonly ushort NameLength = (ushort)nameLength;
}

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 16)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct CopyGCType(uint srcGc, uint dstGc, GCMask mask)
{
    public readonly Opcode OpCode = Opcode.CopyGC;
    private readonly byte _pad0;
    public readonly ushort Length = 4;
    public readonly uint SourceGC = srcGc;
    public readonly uint DestinationGC = dstGc;
    public readonly GCMask GCMake = mask;
}

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 16)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct SetSelectionOwnerType(uint owner, uint atom, uint timestamp)
{
    public readonly Opcode OpCode = Opcode.SetSelectionOwner;
    private readonly byte _pad0;
    public readonly ushort Length = 4;
    public readonly uint Owner = owner;
    public readonly uint Atom = atom;
    public readonly uint Timestamp = timestamp;
}

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 16)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct ChangeActivePointerGrabType(uint cursor, uint time, ushort mask)
{
    public readonly Opcode OpCode = Opcode.ChangeActivePointerGrab;
    private readonly byte _pad0;
    public readonly ushort Length = 4;
    public readonly uint Cursor = cursor;
    public readonly uint Time = time;
    public readonly ushort Mask = mask;
}

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 16)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct ImageText16Type(uint drawable, uint gc, short x, short y, int textLength)
{
    public readonly Opcode OpCode = Opcode.ImageText16;
    public readonly byte TextLength = (byte)textLength;
    public readonly ushort Length = (ushort)(4 + ((2 * textLength).AddPadding() / 4));
    public readonly uint Drawable = drawable;
    public readonly uint GC = gc;
    public readonly short X = x;
    public readonly short Y = y;
}

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 16)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct CreatePixmapType(byte depth, uint pixmapId, uint drawable, ushort width, ushort height)
{
    public readonly Opcode OpCode = Opcode.CreatePixmap;
    public readonly byte Depth = depth;
    public readonly ushort Length = 4;
    public readonly uint PixmapId = pixmapId;
    public readonly uint Drawable = drawable;
    public readonly ushort Width = width;
    public readonly ushort Height = height;
}

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 16)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct ReparentWindowType(uint window, uint parent, short x, short y)
{
    public readonly Opcode OpCode = Opcode.ReparentWindow;
    private readonly byte _pad0;
    public readonly ushort Length = 4;
    public readonly uint Window = window;
    public readonly uint Parent = parent;
    public readonly short X = x;
    public readonly short Y = y;
}

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 16)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct ImageText8Type(uint drawable, uint gc, short x, short y, int textLength)
{
    public readonly Opcode OpCode = Opcode.ImageText8;
    public readonly byte TextLength = (byte)textLength;
    public readonly ushort Length = (ushort)(4 + (textLength.AddPadding() / 4));
    public readonly uint Drawable = drawable;
    public readonly uint GC = gc;
    public readonly short X = x;
    public readonly short Y = y;
}

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 20)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct RecolorCursorType(uint cursorId, ushort foreRed, ushort foreGreen, ushort foreBlue, ushort backRed, ushort backGreen, ushort backBlue)
{
    public readonly Opcode OpCode = Opcode.RecolorCursor;
    private readonly byte _pad0;
    public readonly ushort Length = 5;
    public readonly uint CursorId = cursorId;
    public readonly ushort ForegroundRed = foreRed;
    public readonly ushort ForegroundGreen = foreGreen;
    public readonly ushort ForegroundBlue = foreBlue;
    public readonly ushort BackgroundRed = backRed;
    public readonly ushort BackgroundGreen = backGreen;
    public readonly ushort BackgroundBlue = backBlue;
}

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 24)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct PutImageType(ImageFormat format, uint drawable, uint gc, ushort width,
        ushort height, short x, short y, byte leftPad, byte depth, int dataLength)
{
    public readonly Opcode OpCode = Opcode.PutImage;
    public readonly ImageFormat Format = format;
    public readonly ushort Length = (ushort)(6 + (dataLength.AddPadding() / 4));
    public readonly uint Drawable = drawable;
    public readonly uint Gc = gc;
    public readonly ushort Width = width;
    public readonly ushort Height = height;
    public readonly short X = x;
    public readonly short Y = y;
    public readonly byte LeftPad = leftPad;
    public readonly byte Depth = depth;
}

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 24)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct ChangePropertyType(PropertyMode mode, uint window, uint property, uint type, int argsLength, byte size)
{
    public readonly Opcode OpCode = Opcode.ChangeProperty;
    public readonly PropertyMode Mode = mode;
    public readonly ushort Length = (ushort)(6 + (argsLength.AddPadding() / 4));
    public readonly uint Window = window;
    public readonly uint Property = property;
    public readonly uint Type = type;
    public readonly byte Size = size;
    private readonly byte _pad0;
    private readonly byte _pad1;
    private readonly byte _pad2;
    public readonly int ArgsLength = argsLength;
}

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 24)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct ConvertSelectionType(uint requestor, uint selection, uint target, uint property, uint timestamp)
{
    public readonly Opcode OpCode = Opcode.ConvertSelection;
    private readonly byte _pad0;
    public readonly ushort Length = 6;
    public readonly uint Requestor = requestor;
    public readonly uint Selection = selection;
    public readonly uint Target = target;
    public readonly uint Property = property;
    public readonly uint TimeStamp = timestamp;
}

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 24)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct WarpPointerType(uint srcWindow, uint destWindow, short srcX, short srcY, ushort srcWidth, ushort srcHeight, short destX, short destY)
{
    public readonly Opcode OpCode = Opcode.WarpPointer;
    private readonly byte _pad0;
    public readonly ushort Length = 6;
    public readonly uint SrcWindow = srcWindow;
    public readonly uint DestinationWindow = destWindow;
    public readonly short SrcX = srcX;
    public readonly short SrcY = srcY;
    public readonly ushort SrcWidth = srcWidth;
    public readonly ushort SrcHeight = srcHeight;
    public readonly short DestinationX = destX;
    public readonly short DestinationY = destY;
}

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 24)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct GrabPointerType(bool ownerEvents, uint grabWindow, ushort mask, GrabMode pointerMode, GrabMode keyboardMode, uint confineTo, uint cursor, uint timeStamp)
{
    public readonly Opcode OpCode = Opcode.GrabPointer;
    public readonly byte OwnerEvents = (byte)(ownerEvents ? 1 : 0);
    public readonly ushort Length = 6;
    public readonly uint GrabWindow = grabWindow;
    public readonly ushort Mask = mask;
    public readonly GrabMode PointerMode = pointerMode;
    public readonly GrabMode KeyboardMode = keyboardMode;
    public readonly uint ConfineTo = confineTo;
    public readonly uint Cursor = cursor;
    public readonly uint TimeStamp = timeStamp;
}

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 24)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct GrabButtonType(bool ownerEvents, uint grabWindow, ushort mask, GrabMode pointerMode, GrabMode keyboardMode, uint confineTo, uint cursor, Button button, ModifierMask modifiers)
{
    public readonly Opcode OpCode = Opcode.GrabButton;
    public readonly byte OwnerEvents = (byte)(ownerEvents ? 1 : 0);
    public readonly ushort Length = 6;
    public readonly uint GrabWindow = grabWindow;
    public readonly ushort Mask = mask;
    public readonly GrabMode PointerMode = pointerMode;
    public readonly GrabMode KeyboardMode = keyboardMode;
    public readonly uint ConfineTo = confineTo;
    public readonly uint Cursor = cursor;
    public readonly Button Button = button;
    private readonly byte _pad0;
    public readonly ModifierMask Modifiers = modifiers;
}

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 24)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct GetPropertyType(bool delete, uint window, uint property, uint type, uint offset, uint length)
{
    public readonly Opcode OpCode = Opcode.GetProperty;
    public readonly byte Delete = (byte)(delete ? 1 : 0);
    public readonly ushort Length = 6;
    public readonly uint Window = window;
    public readonly uint Property = property;
    public readonly uint Type = type;
    public readonly uint Offset = offset;
    public readonly uint Length1 = length;
}

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 28)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct CopyAreaType(
    uint srcDrawable,
    uint destDrawable,
    uint gc,
    ushort srcX,
    ushort srcY,
    ushort destX,
    ushort destY,
    ushort width,
    ushort height)
{
    public readonly Opcode OpCode = Opcode.CopyArea;
    private readonly byte _pad0;
    public readonly ushort Length = 7;
    public readonly uint SourceDrawable = srcDrawable;
    public readonly uint DestinationDrawable = destDrawable;
    public readonly uint Gc = gc;
    public readonly ushort SourceX = srcX;
    public readonly ushort SourceY = srcY;
    public readonly ushort DestinationX = destX;
    public readonly ushort DestinationY = destY;
    public readonly ushort Width = width;
    public readonly ushort Height = height;
}

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct CreateWindowType(byte depth, uint window, uint parent, short x, short y, ushort width, ushort height, ushort borderWidth,
    ClassType classType, uint rootVisualId, ValueMask mask, int argsLength)
{
    public readonly Opcode OpCode = Opcode.CreateWindow;
    public readonly byte Depth = depth;
    public readonly ushort Length = (ushort)(8 + argsLength);
    public readonly uint Window = window;
    public readonly uint Parent = parent;
    public readonly short X = x;
    public readonly short Y = y;
    public readonly ushort Width = width;
    public readonly ushort Height = height;
    public readonly ushort BorderWidth = borderWidth;
    public readonly ClassType ClassType = classType;
    public readonly uint RootVisualId = rootVisualId;
    public readonly ValueMask Mask = mask;
}

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct CopyPlaneType(uint srcDrawable, uint destDrawable, uint gc, ushort srcX, ushort srcY, ushort destX, ushort destY, ushort width, ushort height, uint bitPlane)
{
    public readonly Opcode OpCode = Opcode.CopyPlane;
    private readonly byte _pad0;
    public readonly ushort Length = 8;
    public readonly uint SourceDrawable = srcDrawable;
    public readonly uint DestinationDrawable = destDrawable;
    public readonly uint Gc = gc;
    public readonly ushort SourceX = srcX;
    public readonly ushort SourceY = srcY;
    public readonly ushort DestinationX = destX;
    public readonly ushort DestinationY = destY;
    public readonly ushort Width = width;
    public readonly ushort Height = height;
    public readonly uint BitPlane = bitPlane;
}


[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct CreateCursorType(uint cursorId, uint source, uint mask, ushort foreRed, ushort foreGreen, ushort foreBlue, ushort backRed, ushort backGreen, ushort backBlue, ushort x, ushort y)
{
    public readonly Opcode OpCode = Opcode.CreateCursor;
    private readonly byte _pad0;
    public readonly ushort Length = 8;
    public readonly uint CursorId = cursorId;
    public readonly uint Source = source;
    public readonly uint Mask = mask;
    public readonly ushort ForeRed = foreRed;
    public readonly ushort ForeGreen = foreGreen;
    public readonly ushort ForeBlue = foreBlue;
    public readonly ushort BackRed = backRed;
    public readonly ushort BackGreen = backGreen;
    public readonly ushort BackBlue = backBlue;
    public readonly ushort X = x;
    public readonly ushort Y = y;
}

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct CreateGlyphCursorType(uint cursorId, uint sourceFont, uint fontMask, char sourceChar, ushort charMask, ushort foreRed, ushort foreGreen, ushort foreBlue, ushort backRed, ushort backGreen, ushort backBlue)
{
    public readonly Opcode OpCode = Opcode.CreateGlyphCursor;
    private readonly byte _pad0;
    public readonly ushort Length = 8;
    public readonly uint CursorId = cursorId;
    public readonly uint SourceFont = sourceFont;
    public readonly uint FontMask = fontMask;
    public readonly char SourceChar = sourceChar;
    public readonly ushort CharMask = charMask;
    public readonly ushort ForeRed = foreRed;
    public readonly ushort ForeGreen = foreGreen;
    public readonly ushort ForeBlue = foreBlue;
    public readonly ushort BackRed = backRed;
    public readonly ushort BackGreen = backGreen;
    public readonly ushort BackBlue = backBlue;
}

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 44)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct SendEventType(bool propagate, uint destination, uint eventMask, XEvent evnt)
{
    public readonly Opcode OpCode = Opcode.SendEvent;
    public readonly byte Propagate = (byte)(propagate ? 1 : 0);
    public readonly ushort Length = 11;
    public readonly uint Destination = destination;
    public readonly uint EventMask = eventMask;
    public readonly XEvent XEvent = evnt;
}
