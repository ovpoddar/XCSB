using System.Runtime.InteropServices;
using Xcsb.Response.Contract;

namespace Xcsb.Response;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
public readonly struct TranslateCoordinatesReply : IXBaseResponse
{
    public readonly ResponseHeader<byte> ResponseHeader;
    public readonly uint Window;
    public readonly ushort DestinationX;
    public readonly ushort DestinationY;

    public bool Verify(in int sequence)
    {
        return ResponseHeader.Length == 0;
    }
    public bool SameScreen => ResponseHeader.GetValue() == 1;
}