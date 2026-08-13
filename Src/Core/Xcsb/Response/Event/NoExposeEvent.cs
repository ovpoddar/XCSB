using System.Runtime.InteropServices;
using Xcsb.Connection.Helpers;
using Xcsb.Connection.Response.Contract;
using Xcsb.Response.Contract;

namespace Xcsb.Response.Event;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
public struct NoExposeEvent : IXEvent<NoExposeEvent>
{
    public readonly ResponseHeader<ResponseType, byte> ResponseHeader;
    public uint Drawable;
    public ushort MinorOpcode;
    public byte MajorOpcode;


    public ref readonly NoExposeEvent Cast(Span<byte> response)
    {
        ref readonly var result = ref response.AsStruct<NoExposeEvent>();
        if (result.ResponseHeader.Reply == ResponseType.NoExpose && result.ResponseHeader.GetValue() == 0||
            response.Length != 32)
            throw new Exception("Invalid response");
        return ref result;
    }
}