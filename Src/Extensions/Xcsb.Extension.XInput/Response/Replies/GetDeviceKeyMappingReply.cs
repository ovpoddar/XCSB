using Xcsb.Extension.XInput.Models;

namespace Xcsb.Extension.XInput.Response.Replies;

public struct GetDeviceKeyMappingReply
{
    public readonly ResponseType Reply;
    public readonly ushort Sequence;
    public uint[] Keysyms;
}