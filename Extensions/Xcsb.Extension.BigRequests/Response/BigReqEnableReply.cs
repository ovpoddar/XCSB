using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using Xcsb.Connection.Response.Contract;

namespace Xcsb.Extension.BigRequests.Response;

[StructLayout(LayoutKind.Sequential, Size = 32)]
public readonly struct BigReqEnableReply : IXReply
{
    public readonly ResponseHeader<byte> ResponseHeader;
    public readonly uint Length;
    public readonly uint MaximumRequestLength;
    private readonly ushort _pad;
    public bool Verify(in int sequence)
    {
        return ResponseHeader.GetResponseType() == XResponseType.Reply &&
               Length == 0;
    }
}
