using System.Diagnostics;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using Xcsb.Helpers;
using Xcsb.Response.Contract;
using Xcsb.Response.Replies.Internals;

namespace Xcsb.Response.Replies;

public struct GetFontPathReply
{
    public ResponseType Reply;
    public ushort Sequence;
    public string[] Paths;

    internal GetFontPathReply(Span<byte> response)
    {
        ref readonly var context = ref response.AsStruct<GetFontPathResponse>();
        Reply = context.ResponseHeader.Reply;
        Sequence = context.ResponseHeader.Sequence;
        if (context.StringLength == 0)
            Paths = [];
        else
        {
            Paths = new string[context.StringLength];
            var cursor = Unsafe.SizeOf<GetFontPathResponse>();
            var i = 0;
            while (cursor < response.Length)
            {
                var length = response[cursor++];
                if (length == 0)
                    break;
                
                Paths[i++] = cursor + length > response.Length
                    ? Encoding.UTF8.GetString(response[cursor..])
                    : Encoding.UTF8.GetString(response.Slice(cursor, length));
                cursor += length;
            }
            
            Debug.Assert(i == context.StringLength);
        }
    }
}