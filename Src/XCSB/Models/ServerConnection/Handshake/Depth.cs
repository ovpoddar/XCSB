using System.Runtime.InteropServices;
using Xcsb.Configuration;
using Xcsb.Handlers;
using Xcsb.Helpers;

namespace Xcsb.Models.ServerConnection.Handshake;

public class Depth
{
    public byte DepthValue;
#if NETSTANDARD
    public Visual[] Visuals = [];
#else
    public required Visual[] Visuals;
#endif

    internal static Depth Read(SoccketAccesser soccketAccesser, ref int currentlyRead)
    {
        Span<byte> scratchBuffer = stackalloc byte[Marshal.SizeOf<_Depth>()];
        soccketAccesser.Received(scratchBuffer);
        currentlyRead += scratchBuffer.Length;

        ref readonly var depth = ref scratchBuffer.AsStruct<_Depth>();
        var result = new Depth
        {
            DepthValue = depth.DepthValue,
            Visuals = new Visual[depth.VisualsLength]
        };
        currentlyRead += SetVisual(result, soccketAccesser);
        return result;
    }

    private static int SetVisual(Depth depth, SoccketAccesser soccketAccesser)
    {
        var requireByte = Marshal.SizeOf<Visual>() * depth.Visuals.Length;
        if (requireByte < XcsbClientConfiguration.StackAllocThreshold)
        {
            Span<byte> scratchBuffer = stackalloc byte[requireByte];
            soccketAccesser.Received(scratchBuffer);
            MemoryMarshal.Cast<byte, Visual>(scratchBuffer)
                .CopyTo(depth.Visuals);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requireByte);
            soccketAccesser.Received(scratchBuffer[..requireByte]);
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