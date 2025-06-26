using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xcsb.Models;
public struct Arc(ushort x, ushort y, short width, short height, ushort angle1, ushort angle2)
{
    public ushort X = x;
    public ushort Y = y;
    public short Width = width;
    public short Height = height;
    public ushort Angle1 = angle1;
    public ushort Angle2 = angle2;
}
