using System.Runtime.InteropServices;
using Xcsb.Models;

namespace Xcsb.Event;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SelectionRequestEvent
{
    private readonly byte Pad0;
    public ushort Sequence;
    public uint Time; // 0 -> current time
    public uint Owner;
    public uint Requestor;
    public ATOM Selection;
    public ATOM Target;
    public ATOM Property;
}