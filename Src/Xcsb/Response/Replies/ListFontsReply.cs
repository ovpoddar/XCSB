using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using Xcsb.Helpers;
using Xcsb.Response.Contract;
using Xcsb.Response.Replies.Internals;

namespace Xcsb.Response.Replies;

public readonly struct ListFontsReply
{
    public readonly ResponseType Reply;
    public readonly ushort Sequence;
    public readonly string[] Fonts;
    internal ListFontsReply(Span<byte> response)
    {

        ref readonly var context = ref response.AsStruct<ListFontsResponse>();
        Reply = context.ResponseHeader.Reply;
        Sequence = context.ResponseHeader.Sequence;
        if (context.NumberOfFonts == 0)
            Fonts = [];
        else
        {
            var cursor = Unsafe.SizeOf<ListFontsResponse>();
            Fonts = new string[context.NumberOfFonts];
            var i = 0;
            while (cursor < response.Length)
            {
                var length = response[cursor++];
                if (length == 0)
                    break;

                Fonts[i++] = cursor + length > response.Length
                    ? Encoding.UTF8.GetString(response[cursor..])
                    : Encoding.UTF8.GetString(response.Slice(cursor, length));
                cursor += length;
            }

            Debug.Assert(i == context.NumberOfFonts);
        }
    }
}