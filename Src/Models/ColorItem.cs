namespace Xcsb.Models;
public struct ColorItem
{
    public uint Pixel;
    public ushort Red;
    public ushort Green;
    public ushort Blue;
    public ColorFlag ColorFlag;
    private readonly byte Pad0;
}
