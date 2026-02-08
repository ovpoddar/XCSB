using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Connection.Helpers;
using Xcsb.Models;

namespace Xcsb.Requests.BigExtensation;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 12)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct ChangeHostsBigType(HostMode mode, Family family, int addressLength)
{
    public readonly Opcode OpCode = Opcode.ChangeHosts;
    public readonly HostMode Mode = mode;
    private readonly ushort _pad = 0;
    public readonly uint Length = (uint)(3 + addressLength.AddPadding() / 4);
    public readonly Family Family = family;
    private readonly byte _pad0 = 0;
    public readonly ushort AddressLength = (ushort)addressLength;
}