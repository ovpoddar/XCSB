using System.Runtime.InteropServices;
using Xcsb.Connection.Response.Contract;

namespace Xcsb.Extension.XInput.Response.Replies;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
public readonly struct GetExtensionVersionReply : IXReply
{
    public readonly ResponseHeader<byte, byte> ResponseHeader;
    public readonly uint Length;
    public readonly ushort ServerMajor;
    public readonly ushort ServerMinor;
    public readonly byte Present;

    public readonly bool IsPresent => Present != 0;

    public bool Verify(in int sequence)
    {
        return ResponseHeader.Verify(sequence) && Length == 0;
    }
}