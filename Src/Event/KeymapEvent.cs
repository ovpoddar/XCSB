using System.Runtime.InteropServices;

namespace Xcsb.Event;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct KeymapEvent
{
    public fixed byte Keys[31];
}