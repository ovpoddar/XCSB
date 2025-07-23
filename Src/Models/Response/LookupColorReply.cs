using System.Runtime.InteropServices;

namespace Xcsb.Models.Response;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
public readonly struct LookupColorReply : IXBaseResponse
{
    public readonly byte Reply;
    private readonly byte _pad0;
    public readonly ushort Sequence;
    public readonly uint Length;
    public readonly ushort ExactRed;
    public readonly ushort ExactGreen;
    public readonly ushort ExactBlue;
    public readonly ushort VisualRed;
    public readonly ushort VisualGreen;
    public readonly ushort VisualBlue;
    public bool Verify()
    {
        return this.Reply == 1 && this.Length == 0 && this._pad0 == 0;
    }
}