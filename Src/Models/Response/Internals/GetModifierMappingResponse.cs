using System.Runtime.InteropServices;

namespace Xcsb.Models.Response.Internals;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
internal readonly struct GetModifierMappingResponse : IXBaseResponse
{
    public readonly byte Reply;
    public readonly byte KeycodesPerModifier;
    public readonly ushort Sequence;
    public readonly uint Length;

    public bool Verify()
    {
        return this.Reply == 1 && this.Length == this.KeycodesPerModifier * 2;
    }
}