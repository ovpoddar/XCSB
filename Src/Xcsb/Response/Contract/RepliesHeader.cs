using System.Runtime.InteropServices;
using Xcsb.Models;

namespace Xcsb.Response.Contract;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 8)]
internal readonly struct RepliesHeader
{
    public readonly ResponseType Type;
    public readonly byte Undecided;
    public readonly ushort Sequence;
    public readonly uint Length;
}
