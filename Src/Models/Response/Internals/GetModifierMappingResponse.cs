using System.Runtime.InteropServices;
using Xcsb.Models.Response.Contract;

namespace Xcsb.Models.Response.Internals;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
internal readonly struct GetModifierMappingResponse : IXBaseResponse
{
    public readonly byte Reply;
    public readonly byte KeycodesPerModifier;
    public readonly ushort Sequence;
    public readonly uint Length;

    public bool Verify(in int sequence)
    {
        return this.Reply == 1 && this.Length == this.KeycodesPerModifier * 2 && this.Sequence == sequence;
    }
}