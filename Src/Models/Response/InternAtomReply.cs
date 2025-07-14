using System.Runtime.InteropServices;

namespace Xcsb.Models.Response;
[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
public readonly struct InternAtomReply
{
    public readonly byte Reply; // 1
    private readonly byte _pad0;
    public readonly ushort Sequence;
    public readonly uint ReplyLength;
    public readonly uint Atom;
}
