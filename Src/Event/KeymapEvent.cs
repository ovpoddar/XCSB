using System.Runtime.InteropServices;

namespace Xcsb.Event;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct KeymapNotifyEvent
{
    public fixed byte Keys[31];
}