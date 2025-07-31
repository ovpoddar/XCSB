using Xcsb.Models.Response.Contract;

namespace Xcsb.Models.Response;

public readonly struct GrabKeyboardReply : IXBaseResponse
{
    public readonly byte Reply;
    public readonly GrabStatus Status;
    public readonly ushort Sequence;
    public readonly uint Length;
    public bool Verify()
    {
        return this.Reply == 1 && this.Length == 0;
    }
}