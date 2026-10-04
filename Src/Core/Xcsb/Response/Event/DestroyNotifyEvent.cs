using System.Runtime.InteropServices;
using Xcsb.Connection.Helpers;
using Xcsb.Connection.Response.Contract;
using Xcsb.Response.Contract;

namespace Xcsb.Response.Event;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
public struct DestroyNotifyEvent : IXEvent<DestroyNotifyEvent>
{
    public readonly ResponseHeader<ResponseType, byte> ResponseHeader;
    public uint Event;
    public uint Window;

    public ref readonly DestroyNotifyEvent Cast(Span<byte> response)
    {
        ref readonly var result = ref response.AsStruct<DestroyNotifyEvent>();
            if (result.ResponseHeader.Reply != ResponseType.DestroyNotify || response.Length != 32)
            throw new Exception("Invalid response");
        return ref result;
    }

    public DestroyNotifyEvent FromBytes(byte[] response)
    {
        return response.AsSpan().ToStruct<DestroyNotifyEvent>();
    }
}