using System.Runtime.InteropServices;
using Xcsb.Connection.Response.Contract;
using Xcsb.Extension.Damage.Models;

namespace Xcsb.Extension.Damage.Response.Errors;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
public struct BadDamageError : IXError
{
    public readonly ResponseHeader<byte, byte> ResponseHeader;

    public string GetErrorMessage() =>
        "invalid Damage parameter";

    bool IXBaseResponse.Verify(in int sequence)
    {
        return ResponseHeader.Sequence == sequence
            && ResponseHeader.GetValue() == DamageErrorCode.BadDamage;
    }
}
