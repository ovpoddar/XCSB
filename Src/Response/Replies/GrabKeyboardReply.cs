using System.Runtime.InteropServices;
using Xcsb.Models;
using Xcsb.Response.Contract;

namespace Xcsb.Response;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
public readonly struct GrabKeyboardReply : IXBaseResponse
{
    public readonly ResponseHeader<GrabStatus> ResponseHeader;

    public bool Verify(in int sequence)
    {
        return ResponseHeader.Length == 0;
    }
    
    public GrabStatus Status => ResponseHeader.GetValue();
}