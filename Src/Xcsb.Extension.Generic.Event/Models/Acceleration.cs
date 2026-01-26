namespace Xcsb.Models;

public struct Acceleration
{
    public Acceleration(ushort numerator, ushort denominator)
    {
        Numerator = numerator;
        Denominator = denominator;
    }
#if !NETSTANDARD
    public required ushort Numerator { get; set; }
    public required ushort Denominator { get; set; }
#else
    public ushort Numerator { get; set; } = 0;
    public ushort Denominator { get; set; } = 0;
#endif
}