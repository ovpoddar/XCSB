using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Xcsb.Extension.Generic.Event.Response.Contract;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 4)]
public readonly struct ResponseHeader<T> where T : unmanaged
{
    public readonly ResponseType Reply;
    private readonly T Value;
    public readonly ushort Sequence;

    public bool Verify(in int sequence)
    {
        return Sequence == sequence && Unsafe.SizeOf<T>() == 1;
    }

    internal T GetValue() => Value;
}