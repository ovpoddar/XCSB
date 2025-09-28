using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using Xcsb.Response.Contract;
using Xcsb.Response.Errors;

namespace Xcsb.Errors;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
public readonly struct PixmapError : IXError
{
    public readonly ResponseHeader<ErrorCode> ResponseHeader;
    public readonly uint BadResourceId;
    public readonly ushort MinorOpcode;
    public readonly byte MajorOpcode;

    public bool Verify(in int sequence)
    {
        return this.ResponseHeader.Reply == ResponseType.Error && this.ResponseHeader.Sequence == sequence;
    }
}
