using System.Runtime.InteropServices;
using Xcsb.Connection.Helpers;
using Xcsb.Connection.Response.Contract;
using Xcsb.Masks;
using Xcsb.Response.Contract;

namespace Xcsb.Response.Event;

// TODO: need a way to access similar event in a single type
[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
public struct KeyPressEvent : IXEvent<KeyPressEvent>
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

    public byte Detail => ResponseHeader.GetValue();

    public ref readonly KeyPressEvent Cast(Span<byte> response)
    {
        ref readonly var result = ref response.AsStruct<KeyPressEvent>();
        if (result.ResponseHeader.Reply == ResponseType.KeyPress || response.Length != 32)
            throw new Exception("Invalid response");
        return ref result;
    }
}