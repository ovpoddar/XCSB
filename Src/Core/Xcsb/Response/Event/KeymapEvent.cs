using System.Runtime.InteropServices;
using Xcsb.Connection.Helpers;
using Xcsb.Connection.Response.Contract;
using Xcsb.Response.Contract;

namespace Xcsb.Response.Event;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
public unsafe struct KeymapNotifyEvent : IXEvent<KeymapNotifyEvent>
{
    public readonly ResponseType Reply;
    public fixed byte Keys[31];

    public ref readonly KeymapNotifyEvent Cast(Span<byte> response)
    {
        ref readonly var result = ref response.AsStruct<KeymapNotifyEvent>();
        if (result.Reply == ResponseType.KeymapNotify || response.Length != 32)
            throw new Exception("Invalid response");
        return ref result;
    }
}