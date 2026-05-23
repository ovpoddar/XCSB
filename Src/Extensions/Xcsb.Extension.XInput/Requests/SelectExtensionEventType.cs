using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Extension.XInput.Models;

namespace Xcsb.Extension.XInput.Requests;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 12)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct SelectExtensionEventType(byte majorOpCode, uint window, ushort numClasses)
{
    public readonly byte MajorOpcode = majorOpCode;
    public readonly OpCode Opcode = OpCode.SelectExtensionEvent;
    public readonly ushort Length = 3;
    public readonly uint Window = window;
    public readonly ushort NumClasses = numClasses;
}