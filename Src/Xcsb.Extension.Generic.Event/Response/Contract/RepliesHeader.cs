using System.Runtime.InteropServices;
using Xcsb.Extension.Generic.Event.Models;

namespace Xcsb.Extension.Generic.Event.Response.Contract;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 8)]
internal readonly struct RepliesHeader
{
    public readonly XEventType Type;
    public readonly byte Undecided;
    public readonly ushort Sequence;
    public readonly uint Length;
}
