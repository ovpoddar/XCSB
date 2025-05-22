using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Src.Models.Event;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct KeymapEvent
{
    public fixed byte Keys[31];
}
