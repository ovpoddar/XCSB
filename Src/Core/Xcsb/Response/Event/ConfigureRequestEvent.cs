using System.Runtime.InteropServices;
using Xcsb.Connection.Helpers;
using Xcsb.Connection.Response.Contract;
using Xcsb.Response.Contract;

namespace Xcsb.Response.Event;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
public struct ConfigureRequestEvent : IXEvent<ConfigureRequestEvent>
{
    public readonly ResponseHeader<ResponseType, StackMode> ResponseHeader;
    public uint Parent;
    public uint Window;
    public uint Sibling;
    public short X;
    public short Y;
    public ushort Width;
    public ushort Height;
    public ushort BorderWidth;
    public ushort ValueMask;

    public ref readonly ConfigureRequestEvent Cast(Span<byte> response)
    {
        ref readonly var result = ref response.AsStruct<ConfigureRequestEvent>();
        if (result.ResponseHeader.Reply != ResponseType.ConfigureRequest || response.Length != 32)
            throw new Exception("Invalid response");
        return ref result;
    }
}