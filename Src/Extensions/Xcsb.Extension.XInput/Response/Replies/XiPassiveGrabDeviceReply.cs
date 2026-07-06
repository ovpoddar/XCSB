using Xcsb.Extension.XInput.Models;

namespace Xcsb.Extension.XInput.Response.Replies;

public struct XiPassiveGrabDeviceReply
{
    public readonly ResponseType Reply;
    public readonly ushort Sequence;
    public GrabModifierInfo[] Modifier;
}