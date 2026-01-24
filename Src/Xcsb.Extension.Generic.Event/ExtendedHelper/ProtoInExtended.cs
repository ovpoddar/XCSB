using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using Xcsb.Configuration;
using Xcsb.Extension.Generic.Event.Response.Replies;
using Xcsb.Extension.Generic.Event.Response.Replies.Internals;
using Xcsb.Handlers.Direct;
using Xcsb.Helpers;
using Xcsb.Response.Contract;
using Xcsb.Response.Errors;

namespace Xcsb.Extension.Generic.Event.ExtendedHelper;

internal class ProtoInExtended : ProtoIn
{
    internal ProtoInExtended(ProtoIn protoIn) : base(protoIn.Socket, protoIn.Configuration)
    {
    }


    public (ListFontsWithInfoReply[], GenericError?) ReceivedResponseArray(int sequence, int maxNames, int timeOut = 1000)
    {
        while (true)
        {
            if (sequence > Sequence)
            {
                if (Socket.Available == 0)
                    Socket.Poll(timeOut, SelectMode.SelectRead);
                FlushSocket();
                continue;
            }

            if (!ReplyBuffer.Remove(sequence, out var reply))
                throw new Exception("Should not happen.");

            var response = reply.AsSpan().AsStruct<ListFontsWithInfoResponse>();
            return response.Verify(sequence)
                ? (GetListFontsReply(reply, sequence, maxNames), null)
                : ([], reply.AsSpan().ToStruct<GenericError>());
        }
    }

    private ListFontsWithInfoReply[] GetListFontsReply(Span<byte> reply, int sequence, int maxNames)
    {
        var result = new ArrayPoolUsing<ListFontsWithInfoReply>(maxNames);
        var count = 0;
        var cursor = 0;

        while (cursor < reply.Length)
        {
            ref readonly var response = ref reply[cursor..].AsStruct<ListFontsWithInfoResponse>();
            if (!response.HasMore) return result[0..count].ToArray();

            if (count == result.Length)
            {
                var larger = new ArrayPoolUsing<ListFontsWithInfoReply>(result.Length << 1);
                result[0..result.Length].CopyTo(larger);
                result.Dispose();
                result = larger;
            }
            cursor += Unsafe.SizeOf<ListFontsWithInfoResponse>();
            var responseLength = (int)(response.Length * 4) - 28;
            result[count++] = new ListFontsWithInfoReply(in response, reply.Slice(cursor, responseLength));
            cursor += responseLength;
        }

        Span<byte> headerBuffer = stackalloc byte[(Unsafe.SizeOf<XResponse>())];

        while (true)
        {
            _ = Received(headerBuffer);
            var packet = ComputeResponse(ref headerBuffer).AsSpan();

            ref readonly var response = ref packet.AsStruct<ListFontsWithInfoResponse>();
            Debug.Assert(response.ResponseHeader.Sequence == sequence);
            if (!response.HasMore) return result[0..count].ToArray();

            result[count++] = new ListFontsWithInfoReply(in response, packet[60..]);

            if (count != result.Length) continue;

            var larger = new ArrayPoolUsing<ListFontsWithInfoReply>(result.Length << 1);
            result[0..result.Length].CopyTo(larger);
            result.Dispose();
            result = larger;
        }

    }

}
