using Xcsb.Extension.XInput.Models;
using Xcsb.Models;

namespace Xcsb.Extension.XInput.Response.Replies;

public struct XiListPropertiesReply
{
    public readonly ResponseType Reply;
    public readonly ushort Sequence;
    public ATOM[] Properties;
}