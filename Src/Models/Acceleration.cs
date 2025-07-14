namespace Xcsb.Models;
public class Acceleration
{
#if !NETSTANDARD
    public required ushort Numerator { get; set; }
    public required ushort Denominator { get; set; }
#else
    public ushort Numerator { get; set; }
    public ushort Denominator { get; set; }

    public Acceleration(ushort numerator, ushort denominator)
    {
        this.Numerator = numerator;
        this.Denominator = denominator;
    }
#endif
}
