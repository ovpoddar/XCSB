using System.Runtime.InteropServices;
using Xcsb.Response.Contract;

namespace Xcsb.Event;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
public unsafe struct KeymapNotifyEvent : IXEvent
{
    public readonly ResponseType Reply;
    public fixed byte Keys[31];
    public bool Verify(in int sequence)
    {
        return this.Reply == ResponseType.KeymapNotify;
    }
}