using System;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Helpers;
using Xcsb.Masks;

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
internal readonly struct GrabServerType()
{
    public readonly Opcode Opcode = Opcode.GrabServer;
    private readonly byte _pad0 = 0;
    public readonly ushort Length = 1;
}

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 4)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct UnGrabServerType()
{
    public readonly Opcode Opcode = Opcode.UngrabServer;
    private readonly byte _pad0 = 0;
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
internal readonly struct CirculateWindowType(Direction direction, uint window)
{
    public readonly Opcode OpCode = Opcode.CirculateWindow;
    public readonly Direction Direction = direction;
    public readonly ushort Length = 2;
    public readonly uint Window = window;
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
internal readonly struct AllowEventsType(EventsMode mode, uint time)
{
    public readonly Opcode OpCode = Opcode.AllowEvents;
    public readonly EventsMode Mode = mode;
    public readonly ushort Length = 2;
    public readonly uint Time = time;
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
    public readonly uint Mask = gc;
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
internal readonly struct CopyGCType(uint srcGc, uint dstGc, GCMask mask)
{
    public readonly Opcode OpCode = Opcode.CopyGC;
    private readonly byte _pad0 = 0;
    public readonly ushort Length = 4;
    public readonly uint SourceGC = srcGc;
    public readonly uint DestinationGC = dstGc;
    public readonly GCMask GCMake = mask;
}

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 16)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct ImageText16Type(uint drawable, uint gc, short x, short y, int textLength)
{
    public readonly Opcode OpCode = Opcode.ImageText16;
    private readonly byte _pad0 = (byte)textLength;
    public readonly ushort Length = (ushort)(4 + ((2 * textLength).AddPadding() / 4));
    public readonly uint Drawable = drawable;
    public readonly uint GC = gc;
    public readonly short X = x;
    public readonly short Y = y;
}

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 16)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct ImageText8Type(uint drawable, uint gc, short x, short y, int textLength)
{
    public readonly Opcode OpCode = Opcode.ImageText8;
    private readonly byte _pad0 = (byte)textLength;
    public readonly ushort Length = (ushort)(4 + (textLength.AddPadding() / 4));
    public readonly uint Drawable = drawable;
    public readonly uint GC = gc;
    public readonly short X = x;
    public readonly short Y = y;
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

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct CreateWindowType(uint window, uint parent, short x, short y, ushort width, ushort height, ushort borderWidth,
    ClassType classType, uint rootVisualId, ValueMask mask, int argsLength)
{
    public readonly Opcode OpCode = Opcode.CreateWindow;
    private readonly byte _pad0;
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