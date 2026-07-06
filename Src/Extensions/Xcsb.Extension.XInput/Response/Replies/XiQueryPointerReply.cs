using Xcsb.Extension.XInput.Models;

namespace Xcsb.Extension.XInput.Response.Replies;

public struct XiQueryPointerReply
{
    public readonly ResponseType Reply;
    public readonly ushort Sequence;
    public int[] Buttons;
}