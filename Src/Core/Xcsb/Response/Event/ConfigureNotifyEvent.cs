using System.Runtime.InteropServices;
using Xcsb.Connection.Helpers;
using Xcsb.Connection.Response.Contract;
using Xcsb.Response.Contract;

namespace Xcsb.Response.Event;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
public struct ConfigureNotifyEvent : IXEvent<ConfigureNotifyEvent>
{
    public readonly ResponseHeader<ResponseType, byte> ResponseHeader;
    public uint Event;
    public uint Window;
    public uint AboveSibling;
    public short X;
    public short Y;
    public ushort Width;
    public ushort Height;
    public ushort BorderWidth;
    public byte OverrideRedirect;


    public ref readonly ConfigureNotifyEvent Cast(Span<byte> response)
    {
        ref readonly var result = ref response.AsStruct<ConfigureNotifyEvent>();
        if (result.ResponseHeader.Reply != ResponseType.ConfigureNotify && result.ResponseHeader.GetValue() == 0 
            || response.Length != 32)
            throw new Exception("Invalid response");
        return ref result;
    }
}