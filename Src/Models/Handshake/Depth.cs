using Src.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Src.Models.Handshake;

internal class Depth
{
    public byte DepthValue;
    public required Visual[] Visuals;

    public static explicit operator Depth(_Depth depth)
    {
        var dept = new Depth()
        {
            DepthValue = depth.DepthValue,
            Visuals = new Visual[depth.VisualsLength]
        };
        return dept;
    }

    public static Depth Read(Socket socket)
    {
        Span<byte> scratchBuffer = stackalloc byte[Marshal.SizeOf<_Depth>()];
        socket.ReceiveExact(scratchBuffer);
        ref var fixed1 = ref scratchBuffer.AsStruct<_Depth>();

        var result = (Depth)fixed1;
        Span<byte> scratchBudder2 = stackalloc byte[Marshal.SizeOf<Visual>() * fixed1.VisualsLength];
        socket.ReceiveExact(scratchBudder2);
        result.Visuals = MemoryMarshal.Cast<byte, Visual>(scratchBudder2).ToArray();
        return result;
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