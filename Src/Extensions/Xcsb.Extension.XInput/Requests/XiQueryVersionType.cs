using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Extension.XInput.Models;

namespace Xcsb.Extension.XInput.Requests;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 8)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct XiQueryVersionType(byte majorOpCode, ushort majorVersion, ushort minorVersion)
{
    public readonly byte MajorOpcode = majorOpCode;
    public readonly OpCode Opcode = OpCode.XiQueryVersion;
    public readonly ushort Length = 2;
    public readonly ushort MajorVersion = majorVersion;
    public readonly ushort MinorVersion = minorVersion;
}