using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using Xcsb.Connection.Helpers;
using Xcsb.Response.Contract;
using Xcsb.Response.Replies.Internals;

namespace Xcsb.Response.Replies;

public readonly struct ListExtensionsReply
{
    public readonly ResponseType Reply;
    public readonly ushort Sequence;
    public readonly string[] Names;

    internal ListExtensionsReply(Span<byte> response)
    {
        ref readonly var context = ref response.AsStruct<ListExtensionsResponse>();
        Reply = context.ResponseHeader.Reply;
        Sequence = context.ResponseHeader.Sequence;
        if (context.ResponseHeader.GetValue() == 0)
            Names = [];
        else
        {
            Names = new string[context.ResponseHeader.GetValue()];
            var cursor = Unsafe.SizeOf<ListExtensionsResponse>();
            var i = 0;
            while (cursor < response.Length)
            {
                var length = response[cursor++];
                if (length == 0)
                    break;

                Names[i++] = cursor + length > response.Length
                    ? Encoding.UTF8.GetString(response[cursor..])
                    : Encoding.UTF8.GetString(response.Slice(cursor, length));
                cursor += length;
            }

            Debug.Assert(i == context.ResponseHeader.GetValue());
        }
    }
}