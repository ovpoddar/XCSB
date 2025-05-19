using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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