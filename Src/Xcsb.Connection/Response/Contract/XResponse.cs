using System.Runtime.InteropServices;

namespace Xcsb.Connection.Response.Contract;

[StructLayout(LayoutKind.Explicit, Pack = 1, Size = 32)]
internal unsafe struct XResponse : IXBaseResponse
{
    [FieldOffset(0)] public readonly byte ReplyType;
    [FieldOffset(0)] private fixed byte _data[32];
    [FieldOffset(2)] public readonly ushort Sequence;
    [FieldOffset(4)] public readonly uint Length;

    public readonly bool Verify(in int sequence)
    {
        return this.Sequence == sequence;
    }

    internal readonly unsafe Span<byte> Bytes
    {
        get
        {
            fixed (byte* ptr = this._data)
                return new Span<byte>(ptr, 32);
        }
    }
}
