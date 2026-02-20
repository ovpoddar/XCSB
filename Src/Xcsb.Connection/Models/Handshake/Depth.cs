using System.Runtime.InteropServices;
using Xcsb.Connection.Configuration;
using Xcsb.Connection.Handlers;
using Xcsb.Connection.Helpers;

namespace Xcsb.Connection.Models.Handshake;

public class Depth
{
    public byte DepthValue;
#if NETSTANDARD
    public Visual[] Visuals = [];
#else
    public required Visual[] Visuals;
#endif

    internal static Depth Read(ISocketAccessor socketAccessor, ref int currentlyRead)
    {
        Span<byte> scratchBuffer = stackalloc byte[Marshal.SizeOf<_Depth>()];
        socketAccessor.Received(scratchBuffer);
        currentlyRead += scratchBuffer.Length;

        ref readonly var depth = ref scratchBuffer.AsStruct<_Depth>();
        var result = new Depth
        {
            DepthValue = depth.DepthValue,
            Visuals = new Visual[depth.VisualsLength]
        };
        currentlyRead += SetVisual(result, socketAccessor);
        return result;
    }

    private static int SetVisual(Depth depth, ISocketAccessor socketAccessor)
    {
        var requireByte = Marshal.SizeOf<Visual>() * depth.Visuals.Length;
        if (requireByte < XcsbClientConfiguration.StackAllocThreshold)
        {
            Span<byte> scratchBuffer = stackalloc byte[requireByte];
            socketAccessor.Received(scratchBuffer);
            MemoryMarshal.Cast<byte, Visual>(scratchBuffer)
                .CopyTo(depth.Visuals);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requireByte);
            socketAccessor.Received(scratchBuffer[..requireByte]);
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