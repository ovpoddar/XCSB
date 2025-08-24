using System.Runtime.InteropServices;
using Xcsb.Response.Contract;

namespace Xcsb.Event;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
public struct UnMapNotifyEvent : IXEvent
{
    public readonly ResponseHeader<byte> ResponseHeader;
    public uint Event;
    public uint Window;
    public byte FromConfigure; // TODO 1 true 0 false
    
    public bool Verify(in int sequence)
    {
        return this.ResponseHeader.Sequence == sequence && this.ResponseHeader.GetValue() == 0;
    }
}