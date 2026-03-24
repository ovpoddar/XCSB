namespace Xcsb.Connection.Models;

internal readonly struct DataRange
{
    public readonly int Start;
    public readonly int Length;

    public DataRange(int start, int length)
    {
        Start = start;
        Length = length;
    }
}