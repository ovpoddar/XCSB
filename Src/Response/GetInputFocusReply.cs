using System.Runtime.InteropServices;
using Xcsb.Models;
using Xcsb.Response.Contract;

namespace Xcsb.Response;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
public readonly struct GetInputFocusReply : IXBaseResponse
{
    public readonly byte Reply;
    public readonly InputFocusMode Mode;
    public readonly ushort Sequence;
    public readonly uint Length;
    public readonly uint Focus;

    public bool Verify(in int sequence)
    {
        return Reply == 1 && Length == 0 && Sequence == sequence;
    }
}