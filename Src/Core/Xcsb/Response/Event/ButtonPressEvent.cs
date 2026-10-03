using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Connection.Response.Contract;
using Xcsb.Masks;
using Xcsb.Models;
using Xcsb.Response.Contract;
using Xcsb.Connection.Helpers;

namespace Xcsb.Response.Event;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
public readonly struct ButtonPressEvent : IXEvent<ButtonPressEvent>
{
    public readonly ResponseHeader<ResponseType, Button> ResponseHeader;
    public readonly uint TimeStamp;
    public readonly uint RootWindow;
    public readonly uint EventWindow;
    public readonly uint ChildWindow;
    public readonly short RootX;
    public readonly short RootY;
    public readonly short EventX;
    public readonly short EventY;
    public readonly KeyButMask State;
    private readonly sbyte _isSameScreen;
    
    public bool IsSameScreen => _isSameScreen == 1;
    public Button Detail => ResponseHeader.GetValue();

    public ButtonPressEvent FromBytes(Span<byte> response)
    {
        return response.ToStruct<ButtonPressEvent>();
    }
}