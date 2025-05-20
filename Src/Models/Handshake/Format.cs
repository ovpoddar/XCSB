using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Src.Models.Handshake;
[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 8)]
public struct Format
{
    public byte Depth;
    public byte BitsPerPixel;
    public byte ScanLinePad;
}
