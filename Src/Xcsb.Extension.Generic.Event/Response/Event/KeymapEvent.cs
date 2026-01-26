using System.Runtime.InteropServices;
using Xcsb.Extension.Generic.Event.Response.Contract;

namespace Xcsb.Extension.Generic.Event.Response.Event;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
public unsafe struct KeymapNotifyEvent : IXEvent
{
    public readonly ResponseType Reply;
    public fixed byte Keys[31];
    public bool Verify(in int sequence)
    {
        return Reply == ResponseType.KeymapNotify;
    }
}