using System.Runtime.InteropServices;
using Xcsb.Masks;

namespace Xcsb.Models.Requests;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 12)]
internal readonly struct HandShakeRequestType(ushort nameLength, ushort dataLength, ushort majorVersion = 11, ushort minorVersion = 0)
{
    public readonly byte ByteOrder = (byte)(BitConverter.IsLittleEndian ? 'l' : 'B');
    private readonly byte _pad0;
    public readonly ushort MajorVersion = majorVersion;
    public readonly ushort MinorVersion = minorVersion;
    public readonly ushort NameLength = nameLength;
    public readonly ushort DataLength = dataLength;
    private readonly ushort _pad1;
}

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
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