using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using Xcsb.Event;
using Xcsb.Response.Contract;

namespace Xcsb.Errors;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
public readonly struct LengthError : IXError
{
    public readonly ResponseHeader<ErrorCode> ResponseHeader;
    private readonly uint _pad0;
    public readonly ushort MinorOpcode;
    public readonly byte MajorOpcode;

    public bool Verify(in int sequence)
    {
        return this.ResponseHeader.Sequence == sequence && this._pad0 == 0;
    }
}