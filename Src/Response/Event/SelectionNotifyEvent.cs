using System.Runtime.InteropServices;
using Xcsb.Models;

namespace Xcsb.Event;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SelectionNotifyEvent
{
    private readonly byte _pad0;
    public ushort Sequence;
    public uint Time;
    public uint Requestor;
    public ATOM Selection;
    public ATOM Target;
    public ATOM Property;
}