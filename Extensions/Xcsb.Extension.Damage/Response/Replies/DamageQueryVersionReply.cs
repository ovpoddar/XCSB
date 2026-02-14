using System.Runtime.InteropServices;
using Xcsb.Connection.Response.Contract;

namespace Xcsb.Extension.Damage.Response.Replies;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
public readonly struct DamageQueryVersionReply : IXReply
{
    public readonly ResponseHeader<byte> ResponseHeader;
    public readonly uint Length;
    public readonly uint MajorVersion;
    public readonly uint MinorVersion;

    public bool Verify(in int sequence)
    {
        return ResponseHeader.Verify(sequence) && Length == 0;
    }
}
