using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Connection.Response.Contract;
using Xcsb.Extension.XInput.Models;

namespace Xcsb.Extension.XInput.Requests;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
public readonly struct ChangeDeviceControlReply(byte majorOpCode) : IXReply
{
    public readonly ResponseHeader<ResponseType, byte> ResponseHeader;
    public readonly uint Length;
    public readonly byte Status;
    public bool Verify(in int sequence)
    {
        return ResponseHeader.Verify(sequence) && Length == 2 && ResponseHeader.Reply == ResponseType.Reply;
    }
}