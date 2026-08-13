using System.Runtime.InteropServices;
using Xcsb.Connection.Helpers;
using Xcsb.Connection.Response.Contract;
using Xcsb.Response.Contract;

namespace Xcsb.Response.Event;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
public struct ColorMapNotifyEvent : IXEvent<ColorMapNotifyEvent>
{
    public readonly ResponseHeader<ResponseType, byte> ResponseHeader;
    public uint Window;
    public uint ColorMap;
    public byte New;
    public ColormapState State;


    public ref readonly ColorMapNotifyEvent Cast(Span<byte> response)
    {
        ref readonly var result = ref response.AsStruct<ColorMapNotifyEvent>();
        if (result.ResponseHeader.Reply != ResponseType.ColormapNotify || response.Length != 32)
            throw new Exception("Invalid response");
        return ref result;
    }
}