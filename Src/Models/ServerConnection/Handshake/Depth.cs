using System.Runtime.InteropServices;
using Xcsb.Configuration;
using Xcsb.Handlers.Direct;
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

    internal static Depth Read(ProtoIn protoIn, ref int currentlyRead)
    {
        Span<byte> scratchBuffer = stackalloc byte[Marshal.SizeOf<_Depth>()];
        protoIn.ReceiveExact(scratchBuffer);
        currentlyRead += scratchBuffer.Length;

        ref readonly var depth = ref scratchBuffer.AsStruct<_Depth>();
        var result = new Depth
        {
            DepthValue = depth.DepthValue,
            Visuals = new Visual[depth.VisualsLength]
        };
        currentlyRead += SetVisual(result, protoIn);
        return result;
    }

    private static int SetVisual(Depth depth, ProtoIn protoIn)
    {
        var requireByte = Marshal.SizeOf<Visual>() * depth.Visuals.Length;
        if (requireByte < XcbClientConfiguration.StackAllocThreshold)
        {
            Span<byte> scratchBuffer = stackalloc byte[requireByte];
            protoIn.ReceiveExact(scratchBuffer);
            MemoryMarshal.Cast<byte, Visual>(scratchBuffer)
                .CopyTo(depth.Visuals);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requireByte);
            protoIn.ReceiveExact(scratchBuffer[..requireByte]);
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