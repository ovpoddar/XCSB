using System.Runtime.InteropServices;

namespace Xcsb.Models;
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct TimeCoord(uint time, ushort x, ushort y)
{
    public uint Time = time;
    public Point Coord = new(x, y);
}