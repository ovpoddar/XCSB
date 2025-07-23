namespace Xcsb.Models;
//todo: verify the mapping
public struct TimeCoord(uint time, ushort x, ushort y)
{
    public uint Time = time;
    public Point Coord = new(x, y);
}