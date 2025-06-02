using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Helpers;
using Xcsb.Masks;

namespace Xcsb.Models.Requests;

[StructLayout(LayoutKind.Sequential, Pack =1, Size =4)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct BellType(sbyte percent)
{
    public readonly Opcode Opcode = Opcode.Bell;
    public readonly sbyte Percent = percent;
    public readonly ushort Length = 1;
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
internal readonly struct CirculateWindowType(Direction direction, uint window)
{
    public readonly Opcode OpCode = Opcode.CirculateWindow;
    public readonly Direction Direction = direction;
    public readonly ushort Length = 2;
    public readonly uint Window = window;
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
internal readonly struct ChangeWindowAttributesType(uint window, ValueMask mask, int argsLength)
{
    public readonly Opcode opcode = Opcode.ChangeWindowAttributes;
    private readonly byte _pad0;
    public readonly ushort Length = (ushort)(3 + argsLength);
    public readonly uint Window = window;
    public readonly ValueMask Mask = mask;
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