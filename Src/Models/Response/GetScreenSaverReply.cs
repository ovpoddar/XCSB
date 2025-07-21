using System.Runtime.InteropServices;

namespace Xcsb.Models.Response;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
public readonly struct GetScreenSaverReply : IXBaseResponse
{
    public readonly byte Reply;
    private readonly byte _pad0;
    public readonly ushort Sequence;
    public readonly uint Length;
    public readonly ushort Timeout;
    public readonly ushort Interval;
    private readonly byte _preferBlanking;
    private readonly byte _allowExposures;

    public bool Verify()
    {
        return this.Reply == 1;
    }

    public readonly bool AllowExposures => this._allowExposures == 1;
    public readonly bool PreferBlanking => this._preferBlanking == 1;
}