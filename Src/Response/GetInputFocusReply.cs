using System.Runtime.InteropServices;
using Xcsb.Models;
using Xcsb.Response.Contract;

namespace Xcsb.Response;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
public readonly struct GetInputFocusReply : IXBaseResponse
{
    public readonly ResponseHeader<InputFocusMode> ResponseHeader;
    public readonly uint Focus;

    public bool Verify(in int sequence)
    {
        // Mode
        return ResponseHeader.Length == 0;
    }
}