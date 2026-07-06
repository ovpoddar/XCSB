using Xcsb.Extension.XInput.Models;

namespace Xcsb.Extension.XInput.Response.Replies;

public struct QueryDeviceStateReply
{
    public readonly ResponseType Reply;
    public readonly ushort Sequence;
    public byte[] Keys;
}