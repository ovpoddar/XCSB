using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Xcsb.Connection.Response.Contract;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 4)]
public readonly struct ResponseHeader<R, T> where T : unmanaged where R : unmanaged
{
    public readonly R Reply;
    private readonly T _value;
    public readonly ushort Sequence;

    public bool Verify(in int sequence)
    {
        return Sequence == sequence && Unsafe.SizeOf<T>() == 1;
    }

    internal T GetValue() => _value;

}