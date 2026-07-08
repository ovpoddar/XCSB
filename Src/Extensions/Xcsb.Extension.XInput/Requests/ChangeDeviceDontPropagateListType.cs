using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Extension.XInput.Models;

namespace Xcsb.Extension.XInput.Requests;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 12)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct ChangeDeviceDontPropagateListType(byte majorOpCode, uint window, PropagateMode mode, ushort numClasses)
{
    public readonly byte MajorOpcode = majorOpCode;
    public readonly OpCode Opcode = OpCode.ChangeDeviceDontPropagateList;
    public readonly ushort Length = (ushort)(3 + numClasses);
    public readonly uint Window = window;
    public readonly ushort NumClasses = numClasses;
    public readonly PropagateMode Mode = mode;
}