using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Src.Models.Handshake;

internal class Depth
{
    public byte DepthValue;
    public required Visual[] Visuals;

    public static implicit operator Depth(_Depth depth)
    {
        var dept = new Depth()
        {
            DepthValue = depth.DepthValue,
            Visuals = new Visual[depth.VisualsLength]
        };
        return dept;
    }


    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct _Depth
    {
        public byte DepthValue;
        public byte Pad0;
        public ushort VisualsLength;
        public int Pad1;
    }

}