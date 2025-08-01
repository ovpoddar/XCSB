using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Models;

namespace Xcsb.Requests;

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