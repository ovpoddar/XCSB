using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Xcsb.Models.Requests;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 24)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct ConvertSelectionType(uint requestor, uint selection, uint target, uint property, uint timestamp)
{
    public readonly Opcode OpCode = Opcode.ConvertSelection;
    private readonly byte _pad0;
    public readonly ushort Length = 6;
    public readonly uint Requestor = requestor;
    public readonly uint Selection = selection;
    public readonly uint Target = target;
    public readonly uint Property = property;
    public readonly uint TimeStamp = timestamp;
}