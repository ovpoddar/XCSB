namespace Xcsb.Models;

internal readonly struct DataRange
{
    public readonly int Position;
    public readonly int Length;

    public DataRange(int position, int length)
    {
        Position = position;
        Length = length;
    }
}