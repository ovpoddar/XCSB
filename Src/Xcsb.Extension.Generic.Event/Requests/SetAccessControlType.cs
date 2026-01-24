using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Models;

namespace Xcsb.Extension.Generic.Event.Requests;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 4)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct SetAccessControlType(AccessControlMode mode)
{
    public readonly Opcode Opcode = Opcode.SetAccessControl;
    public readonly AccessControlMode Mode = mode;
    public readonly ushort Length = 1;
}