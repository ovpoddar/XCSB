using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Models;

namespace Xcsb.Requests;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 24)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct ConvertSelectionType(
    uint requestor,
    ATOM selection,
    ATOM target,
    ATOM property,
    uint timestamp)
{
    public readonly Opcode OpCode = Opcode.ConvertSelection;
    private readonly byte _pad0 = 0;
    public readonly ushort Length = 6;
    public readonly uint Requestor = requestor;
    public readonly ATOM Selection = selection;
    public readonly ATOM Target = target;
    public readonly ATOM Property = property;
    public readonly uint TimeStamp = timestamp;
}