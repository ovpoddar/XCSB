using System.Runtime.InteropServices;
using Xcsb.Models.Response.Contract;

namespace Xcsb.Models.Response;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
public readonly struct GrabPointerReply : IXBaseResponse
{
    public readonly byte ResponseType; // 1
    public readonly GrabStatus Status;
    public readonly ushort Sequence;
    public readonly uint Length;

    public bool Verify(in int sequence)
    {
        return this.ResponseType == 1 && this.Length == 0 && this.Sequence == sequence;
    }
}