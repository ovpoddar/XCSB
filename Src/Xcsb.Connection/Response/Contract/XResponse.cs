using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using Xcsb.Connection.Helpers;

namespace Xcsb.Connection.Response.Contract;

[StructLayout(LayoutKind.Explicit, Pack = 1, Size = 32)]
internal unsafe struct XResponseNew : IXBaseResponse
{
    [FieldOffset(0)] private byte _replyType;
    [FieldOffset(0)] private fixed byte _data[32];
    [FieldOffset(2)] public readonly ushort Sequence;
    [FieldOffset(4)] public readonly uint Length;

    public readonly bool Verify(in int sequence)
    {
        return this.Sequence == sequence;
    }

    internal readonly XResponseType GetResponseType() => this._replyType switch
    {
        0 => XResponseType.Error,
        1 => XResponseType.Reply,
        11 => XResponseType.Notify,
        2 and <= 34 or 36 => XResponseType.Event,
        _ => XResponseType.Unknown
    };

    internal readonly unsafe Span<byte> Bytes
    {
        get
        {
            fixed (byte* ptr = this._data)
                return new Span<byte>(ptr, 32);
        }
    }
}
