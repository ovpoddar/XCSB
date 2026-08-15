using System.Runtime.InteropServices;

namespace Xcsb.Extension.XInput.Response.Event;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 8)]
public readonly struct Fp3232
{
    public readonly int Integral;
    public readonly int Fractional;
}