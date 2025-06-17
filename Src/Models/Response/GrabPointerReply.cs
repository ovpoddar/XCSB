using System.Runtime.InteropServices;

namespace Xcsb.Models.Response;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
public readonly struct GrabPointerReply
{
    public readonly byte ResponseType;
    public readonly GrabStatus Status;
    public readonly ushort Sequence;
    public readonly uint Length;
}
