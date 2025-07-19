using System.Runtime.InteropServices;

namespace Xcsb.Models.Response;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
public readonly struct InternAtomReply : IXBaseResponse
{
    public readonly byte Reply; // 1
    private readonly byte _pad0;
    public readonly ushort Sequence;
    public readonly uint ReplyLength;
    public readonly uint Atom;
    
    public bool Verify()
    {
        return Reply == 1 && ReplyLength == 0 && _pad0 == 0;
    }
}