using Src.Helpers;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using static Src.Models.Handshake.Depth;
using static Src.Models.Handshake.HandshakeSuccessResponseBody;

namespace Src.Models.Handshake;


public class Screen
{
    public uint Root;
    public uint CMap;
    public uint WhitePixel;
    public uint BlackPixel;
    public int InputMask;
    public ushort Width;
    public ushort Height;
    public ushort MWidth;
    public ushort MHeight;
    public ushort MinMaps;
    public ushort MaxMaps;
    public uint RootVisualId;
    public BackingStores BackingStore;
    public bool SaveUnders;
    private byte _rootDepth;
    public Depth? RootDepth => this.Depths.FirstOrDefault(a => a.DepthValue == _rootDepth);
    public required Depth[] Depths;

    public static Screen Read(Socket socket, ref int currentlyRead)
    {
        Span<byte> scratchBuffer = stackalloc byte[Marshal.SizeOf<_screen>()];
        socket.ReceiveExact(scratchBuffer);
        currentlyRead += scratchBuffer.Length;

        ref var screen = ref scratchBuffer.AsStruct<_screen>();
        var result = new Screen()
        {
            BackingStore = screen.BackingStore,
            RootVisualId = screen.RootVisualId,
            BlackPixel = screen.BlackPixel,
            Width = screen.Width,
            Height = screen.Height,
            MWidth = screen.MWidth,
            MHeight = screen.MHeight,
            MinMaps = screen.MinMaps,
            MaxMaps = screen.MaxMaps,
            CMap = screen.CMap,
            WhitePixel = screen.WhitePixel,
            InputMask = screen.InputMask,
            _rootDepth = screen.RootDepth,
            SaveUnders = screen.SaveUnders != 0,
            Root = screen.Root,
            Depths = new Depth[screen.NumberOfDepth],
        };

        for (var i = 0; i < result.Depths.Length; i++)
            result.Depths[i] = Depth.Read(socket, ref currentlyRead);
        return result;
    }

}


[StructLayout(LayoutKind.Sequential, Pack = 1)]
file struct _screen
{
    public uint Root;
    public uint CMap;
    public uint WhitePixel;
    public uint BlackPixel;
    public int InputMask;
    public ushort Width;
    public ushort Height;
    public ushort MWidth;
    public ushort MHeight;
    public ushort MinMaps;
    public ushort MaxMaps;
    public uint RootVisualId;
    public BackingStores BackingStore;
    public byte SaveUnders;
    public byte RootDepth;
    public byte NumberOfDepth;
}