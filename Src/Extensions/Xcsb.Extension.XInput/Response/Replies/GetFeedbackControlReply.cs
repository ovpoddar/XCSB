using System;
using System.Runtime.CompilerServices;
using Xcsb.Connection.Helpers;
using Xcsb.Connection.Response.Contract;
using Xcsb.Extension.XInput.Models;

namespace Xcsb.Extension.XInput.Response.Replies;

public struct GetFeedbackControlReply  : IXReply<GetFeedbackControlReply, GetFeedbackControlResponse>
{
    public readonly ResponseType Reply;
    public readonly ushort Sequence;
    public readonly byte ReplyType;
    public FeedbackState Feedbacks;

    internal GetFeedbackControlReply(Span<byte> result)
    {
        ref readonly var response = ref result.AsStruct<GetFeedbackControlResponse>();
        Reply = response.ResponseHeader.Reply;
        Sequence = response.ResponseHeader.Sequence;
        ReplyType = response.ResponseHeader.GetValue();
        
        var responseLength = Unsafe.SizeOf<GetFeedbackControlResponse>();
        Feedbacks = new FeedbackState(result[responseLength..]);
    }

    public bool Verify(in int sequence)
    {
        throw new NotImplementedException();
    }

    public GetFeedbackControlReply FromBytes(Span<byte> response)
    {
        return new GetFeedbackControlReply(response);
    }
}