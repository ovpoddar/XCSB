using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using Xcsb.Connection.Models.TypeInfo;
using Xcsb.Connection.Response.Contract;
using Xcsb.Connection.Response.Errors;
using Xcsb.Extension.Damage.Models;

namespace Xcsb.Extension.Damage.Response.Errors;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
public struct BadDamageError : IXError
{
    public readonly ResponseHeader<byte> ResponseHeader;

    public string GetErrorMessage() =>
        "invalid Damage parameter";

    bool IXBaseResponse.Verify(in int sequence)
    {
        return ResponseHeader.Sequence == sequence
            && ResponseHeader.GetValue() == DamageErrorCode.BadDamage;
    }
}
