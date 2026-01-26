namespace Xcsb.Models;

public struct Segment(ushort x1, ushort y1, ushort x2, ushort y2)
{
    public ushort X1 = x1;
    public ushort Y1 = y1;
    public ushort X2 = x2;
    public ushort Y2 = y2;
}