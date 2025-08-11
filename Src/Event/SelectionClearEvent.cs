using System.Runtime.InteropServices;
using Xcsb.Models;

namespace Xcsb.Event;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SelectionClearEvent
{
    private readonly byte Pad0;
    public ushort Sequence;
    public uint Time;
    public uint Owner;
    public ATOM Selection;
}