using Xcsb.Extension.XInput.Models;

namespace Xcsb.Extension.XInput.Response.Replies;

public struct GetDeviceMotionEventsReply
{
    public readonly ResponseType Reply;
    public readonly ushort Sequence;
    public DeviceTimeCoord[] Events;
}