using System.Runtime.InteropServices;

namespace Xcsb.Models.Response.Internals;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
internal unsafe struct QueryKeymapResponse : IXBaseResponse
{
    public readonly byte Reply;
    private readonly byte _pad0;
    public readonly ushort Sequence;
    public readonly uint Length;
    public fixed byte Keys[32];

    public bool Verify()
    {
        return this.Reply == 1 && this.Length == 2 && this._pad0 == 0;
    }
}