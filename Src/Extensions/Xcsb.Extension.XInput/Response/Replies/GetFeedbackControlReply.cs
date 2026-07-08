using Xcsb.Extension.XInput.Models;

namespace Xcsb.Extension.XInput.Response.Replies;

public struct GetFeedbackControlReply
{
    public readonly ResponseType Reply;
    public readonly ushort Sequence;
    public FeedbackState[] Feedbacks;
}