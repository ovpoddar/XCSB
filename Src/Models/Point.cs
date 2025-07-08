using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xcsb.Models;
public struct Point(ushort x, ushort y)
{
    public ushort X = x;
    public ushort Y = y;
}
