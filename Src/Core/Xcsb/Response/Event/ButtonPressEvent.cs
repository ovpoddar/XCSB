using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Connection.Response.Contract;
using Xcsb.Masks;
using Xcsb.Models;
using Xcsb.Response.Contract;
using Xcsb.Connection.Helpers;

namespace Xcsb.Response.Event;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
public struct ButtonPressEvent : IXEvent<ButtonPressEvent>
{
    public readonly ResponseHeader<ResponseType, Button> ResponseHeader;
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

    public ref readonly ButtonPressEvent Cast(Span<byte> response)
    {
        ref readonly var result = ref response.AsStruct<ButtonPressEvent>();
        if (result.ResponseHeader.Reply != ResponseType.ButtonPress || response.Length != 32) 
            throw new Exception("Invalid response");
        return ref result;
    }
}