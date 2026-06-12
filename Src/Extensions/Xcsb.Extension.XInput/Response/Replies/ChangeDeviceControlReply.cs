using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Connection.Response.Contract;

namespace Xcsb.Extension.XInput.Requests;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 8)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
public readonly struct ChangeDeviceControlReply(byte majorOpCode) : IXReply
{
    public readonly ResponseHeader<byte, byte> ResponseHeader;
    public readonly uint Length;
    public readonly byte Status;
    private readonly byte Pad0;
    public bool Verify(in int sequence)
    {
        return ResponseHeader.Verify(sequence) && Length == 2;
    }
}