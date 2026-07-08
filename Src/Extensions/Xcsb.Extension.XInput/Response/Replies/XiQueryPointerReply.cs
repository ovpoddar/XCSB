using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Connection.Helpers;
using Xcsb.Extension.XInput.Models;

namespace Xcsb.Extension.XInput.Response.Replies;

public struct XiQueryPointerReply
{
    public readonly ResponseType Reply;
    public readonly ushort Sequence;
    public readonly uint Root;
    public readonly uint Child;
    public readonly int RootX;
    public readonly int RootY;
    public readonly int WindowX;
    public readonly int WindowY;
    public readonly bool SameScreen;
    public readonly int[] Buttons;

    public XiQueryPointerReply(Span<byte> result)
    {
        ref readonly var response = ref result.AsStruct<XiQueryPointerResponse>();
        Reply = response.ResponseHeader.Reply;
        Sequence = response.ResponseHeader.Sequence;
        Root = response.Root;
        Child = response.Child;
        RootX = response.RootX;
        RootY = response.RootY;
        WindowX = response.WinX;
        WindowY = response.WinY;
        SameScreen = response.SameScreen == 1;
        if (response.ButtonsLen == 0)
            Buttons = Array.Empty<int>();
        else
        {
            var responseLength = Unsafe.SizeOf<XiQueryPointerResponse>();
            Buttons = MemoryMarshal.Cast<byte, int>(result[responseLength..]).ToArray();
        }
    }
}