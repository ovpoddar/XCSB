using Xcsb.Extension.XInput.Models;
using Xcsb.Models;

namespace Xcsb.Extension.XInput.Response.Replies;

public struct ListDevicePropertiesReply
{
    public readonly ResponseType Reply;
    public readonly ushort Sequence;
    public ATOM[] Atoms;
}