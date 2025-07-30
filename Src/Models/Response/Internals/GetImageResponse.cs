using System.Runtime.InteropServices;

namespace Xcsb.Models.Response.Internals;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
internal struct GetImageResponse: IXBaseResponse
{
    public readonly byte Reply;
    public readonly byte Depth;
    public readonly ushort Sequence;
    public readonly uint Length;
    public readonly uint VisualId;
    public bool Verify()
    {
        return this.Reply == 1;
    }
}