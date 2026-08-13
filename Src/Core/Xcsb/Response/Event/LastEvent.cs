using System.Runtime.InteropServices;
using Xcsb.Connection.Helpers;
using Xcsb.Connection.Response.Contract;
using Xcsb.Models.TypeInfo;
using Xcsb.Response.Contract;

namespace Xcsb.Response.Event;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
public readonly struct LastEvent() : IXEvent<LastEvent>
{
    public readonly EventType Reply = EventType.LastEvent;
    private readonly byte _pad = 0;
    
    public ref readonly LastEvent Cast(Span<byte> response)
    {
        ref readonly var result = ref response.AsStruct<LastEvent>();
        if (result.Reply == EventType.LastEvent || response.Length != 32)
            throw new Exception("Invalid response");
        return ref result;
    }
}
