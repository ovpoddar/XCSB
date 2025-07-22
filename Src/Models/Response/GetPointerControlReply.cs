using System.Runtime.InteropServices;

namespace Xcsb.Models.Response;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
public readonly struct GetPointerControlReply : IXBaseResponse
{
    public readonly byte Reply;
    private readonly byte _pad0;
    public readonly ushort Sequence;
    public readonly uint Length;
    public readonly ushort AccelNumerator;
    public readonly ushort AccelDenominator;
    public readonly ushort Threshold;

    public bool Verify()
    {
        return this.Reply == 1 && this.Length == 0;
    }
}