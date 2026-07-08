using Xcsb.Extension.XInput.Models;

namespace Xcsb.Extension.XInput.Response.Replies;

public struct GetSelectedExtensionEventsReply
{
    public readonly ResponseType Reply;
    public readonly ushort Sequence;
    public uint[] ThisClasses;
    public uint[] AllClasses;
}