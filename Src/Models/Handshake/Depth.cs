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

    public static implicit operator Depth(_Depth depth)
    {
        var dept = new Depth()
        {
            DepthValue = depth.DepthValue,
            Visuals = new Visual[depth.VisualsLength]
        };
        return dept;
    }

    public static Depth Read(Socket socket, ref int currentlyRead)
    {
        Span<byte> scratchBuffer = stackalloc byte[Marshal.SizeOf<_Depth>()];
        socket.ReceiveExact(scratchBuffer);
        currentlyRead += scratchBuffer.Length;

        Depth depth = scratchBuffer.ToStruct<_Depth>();
        currentlyRead += SetVisual(depth, socket);
        return depth;
    }

    private static int SetVisual(Depth depth, Socket socket)
    {
        var requireByte = Marshal.SizeOf<Visual>() * depth.Visuals.Length;
        if (requireByte < GlobalSetting.StackAllocThreshold)
        {
            Span<byte> scratchBuffer = stackalloc byte[requireByte];
            socket.ReceiveExact(scratchBuffer);
            MemoryMarshal.Cast<byte, Visual>(scratchBuffer)
                .CopyTo(depth.Visuals);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requireByte);
            socket.ReceiveExact(scratchBuffer[..requireByte]);
            MemoryMarshal.Cast<byte, Visual>(scratchBuffer)
                .CopyTo(depth.Visuals);
        }
        return requireByte;
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