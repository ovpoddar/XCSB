using System.Runtime.InteropServices;
using Xcsb.Models.Response.Contract;

namespace Xcsb.Models.Response;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
public readonly struct TranslateCoordinatesReply : IXBaseResponse
{
    public readonly byte Reply;
    private readonly byte _sameScreen;
    public readonly ushort Sequence;
    public readonly uint Length;
    public readonly uint Window;
    public readonly ushort DestinationX;
    public readonly ushort DestinationY;
    public bool Verify()
    {
        return this.Reply == 1 && this.Length == 0;
    }
    public bool SameScreen => this._sameScreen == 1;
}