using System.Runtime.CompilerServices;
using System.Text;
using Xcsb.Connection.Helpers;
using Xcsb.Connection.Response.Contract;
using Xcsb.Response.Contract;
using Xcsb.Response.Replies.Internals;

namespace Xcsb.Response.Replies;

public readonly struct GetAtomNameReply: IXReply<GetAtomNameReply, GetAtomNameResponse>
{
    public readonly ResponseType Reply;
    public readonly ushort Sequence;
    public readonly string Name;

    internal GetAtomNameReply(Span<byte> response)
    {
        ref readonly var context = ref response.AsStruct<GetAtomNameResponse>();
        Reply = (ResponseType)context.ResponseHeader.Reply;
        Sequence = context.ResponseHeader.Sequence;

        if (context.Length == 0)
            Name = string.Empty;
        else
        {
            var cursor = Unsafe.SizeOf<GetAtomNameResponse>();
            Name = Encoding.ASCII.GetString(response.Slice(cursor, context.LengthOfName).ToArray());
        }
    }

    public GetAtomNameReply FromBytes(byte[] response)
    {
        return new GetAtomNameReply(response);
    }
}