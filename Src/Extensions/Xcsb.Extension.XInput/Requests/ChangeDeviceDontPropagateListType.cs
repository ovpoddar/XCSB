using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Extension.XInput.Models;

namespace Xcsb.Extension.XInput.Requests;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 12)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct ChangeDeviceDontPropagateListType(byte majorOpCode, uint window, ushort numClasses, byte mode)
{
    public readonly byte MajorOpcode = majorOpCode;
    public readonly OpCode Opcode = OpCode.ChangeDeviceDontPropagateList;
    public readonly ushort Length = 3;
    public readonly uint Window = window;
    public readonly ushort NumClasses = numClasses;
    public readonly byte Mode = mode;
}