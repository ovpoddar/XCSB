using System.Runtime.InteropServices;
using Xcsb.Extension.Generic.Event.Models;
using Xcsb.Extension.Generic.Event.Response.Contract;

namespace Xcsb.Extension.Generic.Event.Response.Event;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
public struct ButtonPressEvent : IXEvent
{
    public readonly ResponseHeader<Button> ResponseHeader;
    public uint TimeStamp;
    public uint RootWindow;
    public uint EventWindow;
    public uint ChildWindow;
    public short RootX;
    public short RootY;
    public short EventX;
    public short EventY;
    public KeyButMask State;
    private sbyte _isSameScreen;
    public bool IsSameScreen => _isSameScreen == 1;
    public Button Detail => ResponseHeader.GetValue();

    public bool Verify(in int sequence)
    {
        return ResponseHeader.Reply == ResponseType.ButtonPress;
    }
}