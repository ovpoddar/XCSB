using System.Runtime.InteropServices;
using Xcsb.Response.Contract;

namespace Xcsb.Response;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
public readonly struct GetSelectionOwnerReply : IXBaseResponse
{
    public readonly ResponseHeader<byte>ResponseHeader;
    public readonly uint Owner;

    public bool Verify(in int sequence)
    {
        return ResponseHeader.Length == 0;
    }
}