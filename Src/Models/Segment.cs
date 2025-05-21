namespace Src.Models;
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