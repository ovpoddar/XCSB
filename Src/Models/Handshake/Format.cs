using System.Runtime.InteropServices;

namespace Src.Models.Handshake;
[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 8)]
public struct Format
{
    public byte Depth;
    public byte BitsPerPixel;
    public byte ScanLinePad;
}
