using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Masks;

namespace Xcsb.Models.Requests;

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