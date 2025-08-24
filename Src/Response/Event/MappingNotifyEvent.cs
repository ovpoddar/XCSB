using System.Runtime.InteropServices;
using Xcsb.Response.Contract;

namespace Xcsb.Event;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
public struct MappingNotifyEvent : IXEvent
{
    public readonly ResponseHeader<byte> ResponseHeader;
    public Mapping Request;
    public byte FirstKeyCode;
    public byte Count;

    public bool Verify(in int sequence)
    {
        return this.ResponseHeader.Sequence == sequence && this.ResponseHeader.GetValue() == 0;
    }
}