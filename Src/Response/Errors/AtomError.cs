using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using Xcsb.Response.Contract;
using Xcsb.Response.Errors;

namespace Xcsb.Errors;

[StructLayout(LayoutKind.Sequential, Size = 1)]
public readonly struct AtomError : IXError
{
    public readonly ResponseHeader<ErrorCode> ResponseHeader;
    public readonly uint BadAtomId;
    public readonly ushort MinorOpcode;
    public readonly byte MajorOpcode;

    public bool Verify(in int sequence)
    {
        return this.ResponseHeader.Reply == ResponseType.Error && this.ResponseHeader.Sequence == sequence;
    }
}