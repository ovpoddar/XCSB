using System.Runtime.InteropServices;
using Xcsb.Connection.Helpers;
using Xcsb.Connection.Response.Contract;
using Xcsb.Response.Contract;

namespace Xcsb.Response.Event;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
public struct ExposeEvent : IXEvent<ExposeEvent>
{
    public readonly ResponseHeader<ResponseType, byte> ResponseHeader;
    public uint Window;
    public ushort X;
    public ushort Y;
    public ushort Width;
    public ushort Height;
    public ushort Count;

    public ref readonly ExposeEvent Cast(Span<byte> response)
    {
        ref readonly var result = ref response.AsStruct<ExposeEvent>();
        if (result.ResponseHeader.Reply != ResponseType.Expose && result.ResponseHeader.GetValue() == 0 || response.Length != 32)
            throw new Exception("Invalid response");
        return ref result;
    }
}