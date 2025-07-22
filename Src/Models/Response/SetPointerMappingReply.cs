using System.Runtime.InteropServices;

namespace Xcsb.Models.Response;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
public readonly struct SetPointerMappingReply : IXBaseResponse
{
    public readonly byte Reply;
    public readonly Status Status;
    public readonly ushort Sequence;
    public readonly uint Length;
    public bool Verify()
    {
        return this.Reply == 1 && this.Length == 0;
    }
}