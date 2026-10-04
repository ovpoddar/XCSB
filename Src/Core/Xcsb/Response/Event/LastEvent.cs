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
    
    public LastEvent FromBytes(byte[] response)
    {
        return response.AsSpan().ToStruct<LastEvent>();
    }
}
