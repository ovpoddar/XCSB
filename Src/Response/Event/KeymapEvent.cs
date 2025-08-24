using System.Runtime.InteropServices;
using Xcsb.Response.Contract;

namespace Xcsb.Event;

// todo marker
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct KeymapNotifyEvent : IXEvent
{
    public fixed byte Keys[31];
    public bool Verify(in int sequence)
    {
        return true;
    }
}