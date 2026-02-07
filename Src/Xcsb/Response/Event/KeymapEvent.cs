using System.Runtime.InteropServices;
using Xcsb.Connection.Response.Contract;
using Xcsb.Response.Contract;

namespace Xcsb.Response.Event;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
public unsafe struct KeymapNotifyEvent : IXEvent
{
    public readonly ResponseType Reply;
    public fixed byte Keys[31];
    public readonly bool Verify()
    {
        return Reply == ResponseType.KeymapNotify;
    }
}