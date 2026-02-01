using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Connection.Response.Contract;

namespace Xcsb.Response.Contract;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 4)]
public readonly struct ResponseHeader<T> where T : unmanaged
{
    public readonly byte Reply;
    private readonly T _value;
    public readonly ushort Sequence;

    public bool Verify(in int sequence)
    {
        return Sequence == sequence && Unsafe.SizeOf<T>() == 1;
    }

    internal T GetValue() => _value;

    internal readonly XResponseType GetResponseType() => this.Reply switch
    {
        0 => XResponseType.Error,
        1 => XResponseType.Reply,
        11 => XResponseType.Notify,
        2 and <= 34 or 36 => XResponseType.Event,
        _ => XResponseType.Unknown
    };

}