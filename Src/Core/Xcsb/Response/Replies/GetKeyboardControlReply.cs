using Xcsb.Connection.Helpers;
using Xcsb.Connection.Response.Contract;
using Xcsb.Models;
using Xcsb.Response.Contract;
using Xcsb.Response.Replies.Internals;

namespace Xcsb.Response.Replies;

public struct GetKeyboardControlReply: IXReply<GetKeyboardControlReply, GetKeyboardControlResponse>
{
    public readonly ResponseType Reply;
    public readonly AutoRepeatMode AutoRepeatMode;
    public readonly ushort Sequence;
    public readonly uint Length;
    public readonly uint LedMask;
    public readonly byte KeyClickPercent;
    public readonly byte BellPercent;
    public readonly ushort BellPitch;
    public readonly ushort BellDuration;
    public readonly byte[] Repeats = new byte[32];

    internal GetKeyboardControlReply(Span<byte> span)
    {
        unsafe
        {
            ref readonly GetKeyboardControlResponse result = ref span.AsStruct<GetKeyboardControlResponse>();
            Reply = (ResponseType)result.ResponseHeader.Reply;
            AutoRepeatMode = result.ResponseHeader.GetValue();
            Sequence = result.ResponseHeader.Sequence;
            Length = result.Length;
            LedMask = result.LedMask;
            KeyClickPercent = result.KeyClickPercent;
            BellPercent = result.BellPercent;
            BellPitch = result.BellPitch;
            BellDuration = result.BellDuration;
            fixed(byte* p  = result.Repeats)
                Repeats = new Span<byte>(p, 32).ToArray();
        }
    }

    public GetKeyboardControlReply FromBytes(byte[] response)
    {
        return new GetKeyboardControlReply(response.AsSpan());
    }
}