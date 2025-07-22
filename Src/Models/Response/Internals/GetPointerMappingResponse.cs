using System.Runtime.InteropServices;

namespace Xcsb.Models.Response.Internals;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
public readonly struct GetPointerMappingResponse : IXBaseResponse
{
    public readonly byte Reply;
    public readonly byte MapLength;
    public readonly ushort Sequence;
    public readonly uint Length;
    
    public bool Verify()
    {
        return this.Reply == 1;
    }
}