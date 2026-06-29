using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Connection.Response.Contract;
using Xcsb.Extension.XInput.Models;

namespace Xcsb.Extension.XInput.Response.Replies;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
public readonly struct XiGetClientPointerReply : IXReply
{
    public readonly ResponseHeader<ResponseType, byte> ResponseHeader;
    public readonly uint Length;
    public readonly byte Set;
    public readonly byte Pad1;
    public readonly ushort Deviceid; //xcb_input_device_id_t

    public bool Verify(in int sequence)
    {
        return  ResponseHeader.Verify(sequence) && ResponseHeader.Reply == ResponseType.Reply;
    }
}