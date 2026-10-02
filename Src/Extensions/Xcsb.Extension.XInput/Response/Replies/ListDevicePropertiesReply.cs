using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Connection.Helpers;
using Xcsb.Connection.Response.Contract;
using Xcsb.Extension.XInput.Models;
using Xcsb.Models;

namespace Xcsb.Extension.XInput.Response.Replies;

public struct ListDevicePropertiesReply : IXReply<ListDevicePropertiesReply, ListDevicePropertiesResponse>
{
    public readonly ResponseType Reply;
    public readonly ushort Sequence;
    public ATOM[] Atoms;

    internal ListDevicePropertiesReply(Span<byte> result)
    {
        ref readonly var response = ref result.AsStruct<ListDevicePropertiesResponse>();
        Reply = response.ResponseHeader.Reply;
        Sequence = response.ResponseHeader.Sequence;
        if (response.NumAtoms == 0)
            Atoms = Array.Empty<ATOM>();
        else
        {
            var responseSize = Unsafe.SizeOf<ListDevicePropertiesResponse>();
            Atoms = MemoryMarshal.Cast<byte, ATOM>(result[responseSize..]).ToArray();
        }
    }

    public bool Verify(in int sequence)
    {
        throw new NotImplementedException();
    }

    public ListDevicePropertiesReply FromBytes(Span<byte> response)
    {
        return new ListDevicePropertiesReply(response);
    }
}