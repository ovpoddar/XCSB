using System.Runtime.InteropServices;

namespace Xcsb.Models.Response.Internals;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
internal readonly struct ListPropertiesResponse : IXBaseResponse
{
    public readonly byte Reply;
    private readonly byte _pad0;
    public readonly ushort Sequence;
    public readonly uint Length;
    public readonly ushort NumberOfProperties;

    public bool Verify()
    {
        return this.Reply == 1 && this.Length == this.NumberOfProperties;
    }
}