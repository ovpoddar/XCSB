using System.Runtime.InteropServices;

namespace Xcsb.Models.Response;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
public readonly struct AllocColorReply
{
    public readonly byte ResponseType;
    private readonly byte _pad0;
    public readonly ushort Sequence;
    public readonly uint length;
    public readonly ushort Red;
    public readonly ushort Green;
    public readonly ushort Blue;
    private readonly ushort _pad1;
    public readonly uint pixel;
}