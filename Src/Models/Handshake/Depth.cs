using Src.Helpers;
using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace Src.Models.Handshake;

public class Depth
{
    public byte DepthValue;
    public required Visual[] Visuals;

    public static Depth Read(Socket socket, ref int currentlyRead)
    {
        Span<byte> scratchBuffer = stackalloc byte[Marshal.SizeOf<_Depth>()];
        socket.ReceiveExact(scratchBuffer);
        currentlyRead += scratchBuffer.Length;

        ref var depth = ref scratchBuffer.AsStruct<_Depth>();
        var result = new Depth()
        {
            DepthValue = depth.DepthValue,
            Visuals = new Visual[depth.VisualsLength]
        };
        currentlyRead += SetVisual(result, socket);
        return result;
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
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
file struct _Depth
{
    public byte DepthValue;
    public byte Pad0;
    public ushort VisualsLength;
    public int Pad1;
}