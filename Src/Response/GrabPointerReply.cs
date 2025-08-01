using System.Runtime.InteropServices;
using Xcsb.Models;
using Xcsb.Response.Contract;

namespace Xcsb.Response;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
public readonly struct GrabPointerReply : IXBaseResponse
{
    public readonly byte ResponseType; // 1
    public readonly GrabStatus Status;
    public readonly ushort Sequence;
    public readonly uint Length;

    public bool Verify(in int sequence)
    {
        return ResponseType == 1 && Length == 0 && Sequence == sequence;
    }
}