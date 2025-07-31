using System.Runtime.InteropServices;
using Xcsb.Models.Response.Contract;

namespace Xcsb.Models.Response;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
public readonly struct GetScreenSaverReply : IXBaseResponse
{
    public readonly ResponseHeader ResponseHeader;
    public readonly ushort Timeout;
    public readonly ushort Interval;
    private readonly byte _preferBlanking;
    private readonly byte _allowExposures;

    public bool Verify(in int sequence)
    {
        return this.ResponseHeader.Verify(in sequence);
    }

    public readonly bool AllowExposures => this._allowExposures == 1;
    public readonly bool PreferBlanking => this._preferBlanking == 1;
}