using System.Runtime.InteropServices;

namespace Xcsb.Event;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct GenericEvent
{
    public fixed byte Pad[22];
}