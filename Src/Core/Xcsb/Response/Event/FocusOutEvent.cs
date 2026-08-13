using System.Runtime.InteropServices;
using Xcsb.Connection.Helpers;
using Xcsb.Connection.Response.Contract;
using Xcsb.Models;
using Xcsb.Response.Contract;

namespace Xcsb.Response.Event;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
public struct FocusOutEvent : IXEvent<FocusOutEvent>
{
    public readonly ResponseHeader<ResponseType, NotifyDetail> ResponseHeader;
    public int Event;
    public NotifyMode Mode;
    
    public ref readonly FocusOutEvent Cast(Span<byte> response)
    {
        ref readonly var result = ref response.AsStruct<FocusOutEvent>();
        if (result.ResponseHeader.Reply != ResponseType.FocusOut || response.Length != 32)
            throw new Exception("Invalid response");
        return ref result;
    }
}