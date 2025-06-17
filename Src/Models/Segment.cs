namespace Xcsb.Models;

// todo:remove segment use range
internal readonly ref struct Segment<T> where T : struct
{
    public T Position { get; }
    public T Length { get; }

    public Segment(T position, T length)
    {
        Position = position;
        Length = length;
    }
}

public struct Segment
{
    public ushort X1;
    public ushort Y1;
    public ushort X2;
    public ushort Y2;
}