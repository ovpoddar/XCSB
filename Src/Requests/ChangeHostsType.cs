using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Helpers;
using Xcsb.Models;

namespace Xcsb.Requests;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 8)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct ChangeHostsType(HostMode mode, Family family, int addressLength)
{
    public readonly Opcode OpCode = Opcode.ChangeHosts;
    public readonly HostMode Mode = mode;
    public readonly ushort Length = (ushort)(2 + addressLength.AddPadding() / 4);
    public readonly Family Family = family;
    private readonly byte _pad0;
    public readonly ushort AddressLength = (ushort)addressLength;
}