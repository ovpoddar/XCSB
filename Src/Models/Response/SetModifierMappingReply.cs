using System.Runtime.InteropServices;
using Xcsb.Models.Response.Contract;

namespace Xcsb.Models.Response;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
public readonly struct SetModifierMappingReply : IXBaseResponse
{
    public readonly byte Reply;
    public readonly MappingStatus Status;
    public readonly ushort Sequence;
    public readonly uint Length;
    public bool Verify()
    {
        return this.Reply == 1 && this.Length == 0;
    }
}