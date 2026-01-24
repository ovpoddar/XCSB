using System.Runtime.InteropServices;
using Xcsb.Models;
using Xcsb.Response.Contract;

namespace Xcsb.Extension.Generic.Event.Response.Replies.Internals;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 52)]
internal unsafe struct GetKeyboardControlResponse : IXReply
{
    public readonly ResponseHeader<AutoRepeatMode> ResponseHeader;
    public readonly uint Length;
    public readonly uint LedMask;
    public readonly byte KeyClickPercent;
    public readonly byte BellPercent;
    public readonly ushort BellPitch;
    public readonly ushort BellDuration;
    private readonly ushort _pad0;
    public fixed byte Repeats[32];

    public bool Verify(in int sequence)
    {
        return ResponseHeader.Reply == ResponseType.Reply &&
               Length == 5;
    }
}