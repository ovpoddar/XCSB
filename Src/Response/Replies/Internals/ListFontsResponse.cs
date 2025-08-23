using System.Runtime.InteropServices;
using Xcsb.Response.Contract;

namespace Xcsb.Response.Internals;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
internal readonly struct ListFontsResponse : IXReply
{
    public readonly ResponseHeader<byte>ResponseHeader;
    public readonly uint Length;
    public readonly ushort NumberOfFonts;

    public bool Verify(in int sequence)
    {
        return ResponseHeader.Verify(in sequence);
    }
}