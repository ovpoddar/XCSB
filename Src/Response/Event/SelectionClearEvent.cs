using System.Runtime.InteropServices;
using Xcsb.Models;
using Xcsb.Response.Contract;

namespace Xcsb.Event;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
public struct SelectionClearEvent : IXEvent
{
    public readonly ResponseHeader<byte> ResponseHeader;
    public uint Time;
    public uint Owner;
    public ATOM Selection;
    
    public bool Verify(in int sequence)
    {
        return this.ResponseHeader.Sequence == sequence && this.ResponseHeader.GetValue() == 0;
    }
}