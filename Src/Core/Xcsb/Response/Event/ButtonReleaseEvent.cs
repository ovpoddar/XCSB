using System.Runtime.InteropServices;
using Xcsb.Connection.Helpers;
using Xcsb.Connection.Response.Contract;
using Xcsb.Masks;
using Xcsb.Response.Contract;

namespace Xcsb.Response.Event;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
public struct ButtonReleaseEvent : IXEvent<ButtonReleaseEvent>
{
    public readonly ResponseHeader<ResponseType, byte> ResponseHeader;
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

    public ref readonly ButtonReleaseEvent Cast(Span<byte> response)
    {
        ref readonly var result = ref response.AsStruct<ButtonReleaseEvent>();
        if (result.ResponseHeader.Reply != ResponseType.ButtonRelease || response.Length != 32) 
            throw new Exception("Invalid response");
        return ref result;
    }
}