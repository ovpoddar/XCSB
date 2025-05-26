using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Src.Models;
public struct ColorItem
{
    public uint Pixel;
    public ushort Red;
    public ushort Green;
    public ushort Blue;
    public ColorFlag ColorFlag;
    private byte Pad0; 
}
