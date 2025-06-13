using System.Runtime.InteropServices;

namespace Xcsb.Models;
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct InternAtomReply
{
    public readonly byte Reply;
    private readonly byte _pad0;
    public readonly ushort SequenceNumber;
    public readonly uint ReplyLength;
    public readonly uint Atom;
    private fixed byte _pad1[20];
}
