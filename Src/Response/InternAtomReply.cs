using System.Runtime.InteropServices;
using Xcsb.Response.Contract;

namespace Xcsb.Response;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
public readonly struct InternAtomReply : IXBaseResponse
{
    public readonly ResponseHeader<byte>ResponseHeader;
    public readonly uint Atom;

    public bool Verify(in int sequence)
    {
        return ResponseHeader.Length == 0;
    }
}