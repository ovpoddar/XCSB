using System.Runtime.InteropServices;
using Xcsb.Connection.Helpers;
using Xcsb.Connection.Response.Contract;
using Xcsb.Response.Contract;

namespace Xcsb.Response.Event;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
public struct GraphicsExposeEvent : IXEvent<GraphicsExposeEvent>
{
    public readonly ResponseHeader<ResponseType, byte> ResponseHeader;
    public uint Drawable;
    public ushort X;
    public ushort Y;
    public ushort Width;
    public ushort Height;
    public ushort MinorOpcode;
    public ushort Count;
    public byte MajorOpcode;

    public ref readonly GraphicsExposeEvent Cast(Span<byte> response)
    {
        ref readonly var result = ref response.AsStruct<GraphicsExposeEvent>();
        if (result.ResponseHeader.Reply != ResponseType.GraphicsExpose && result.ResponseHeader.GetValue() == 0 ||
            response.Length != 32)
            throw new Exception("Invalid response");
        return ref result;
    }
}