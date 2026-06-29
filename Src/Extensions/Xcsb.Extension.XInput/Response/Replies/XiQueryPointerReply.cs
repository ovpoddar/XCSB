using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Connection.Response.Contract;
using Xcsb.Extension.XInput.Models;

namespace Xcsb.Extension.XInput.Response.Replies;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
public readonly struct XiQueryPointerReply : IXReply
{
    public readonly ResponseHeader<ResponseType, byte> ResponseHeader;
    public readonly uint Length;
    public readonly uint Root;
    public readonly uint Child;
    public readonly int RootX; //xcb_input_fp1616_t
    public readonly int RootY; //xcb_input_fp1616_t
    public readonly int WinX; //xcb_input_fp1616_t
    public readonly int WinY; //xcb_input_fp1616_t
    public readonly byte SameScreen;
    public readonly byte Pad1;
    public readonly ushort ButtonsLen;
    // public readonly xcb_input_modifier_info_t Mods;
    // public readonly xcb_input_group_info_t Group;

    public bool Verify(in int sequence)
    {
        return  ResponseHeader.Verify(sequence) && ResponseHeader.Reply == ResponseType.Reply;
    }
}