using System.Runtime.InteropServices;
using Xcsb.Response.Contract;

namespace Xcsb.Response;

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
        return ResponseHeader.Verify(in sequence);
    }

    public readonly bool AllowExposures => _allowExposures == 1;
    public readonly bool PreferBlanking => _preferBlanking == 1;
}