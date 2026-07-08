using System;
using Xcsb.Connection.Helpers;
using Xcsb.Extension.XInput.Models;

namespace Xcsb.Extension.XInput.Response.Replies;

public struct GetFeedbackControlReply
{
    public readonly ResponseType Reply;
    public readonly ushort Sequence;
    public readonly byte ReplyType;
    public FeedbackState[] Feedbacks;

    public GetFeedbackControlReply(Span<byte> result)
    {
        ref readonly var response = ref result.AsStruct<GetFeedbackControlResponse>();
        Reply = response.ResponseHeader.Reply;
        Sequence = response.ResponseHeader.Sequence;
        ReplyType = response.ResponseHeader.GetValue();
    }
}