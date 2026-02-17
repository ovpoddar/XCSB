using System.Runtime.InteropServices;
using Xcsb.Connection.Handlers;
using Xcsb.Connection.Helpers;

namespace Xcsb.Connection.Models.Handshake;

public class Screen
{
    public uint Root;
    public uint DefaultColormap;
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
    public Depth? RootDepth => Depths.FirstOrDefault(a => a.DepthValue == _rootDepth);
#if NETSTANDARD
    public Depth[] Depths = [];
#else
    public required Depth[] Depths;
#endif

    internal static Screen Read(ISoccketAccesser soccketAccesser, ref int currentlyRead)
    {
        Span<byte> scratchBuffer = stackalloc byte[Marshal.SizeOf<_screen>()];
        soccketAccesser.Received(scratchBuffer);
        currentlyRead += scratchBuffer.Length;

        ref readonly var screen = ref scratchBuffer.AsStruct<_screen>();
        var result = new Screen
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
            DefaultColormap = screen.CMap,
            WhitePixel = screen.WhitePixel,
            InputMask = screen.InputMask,
            _rootDepth = screen.RootDepth,
            SaveUnders = screen.SaveUnders != 0,
            Root = screen.Root,
            Depths = new Depth[screen.NumberOfDepth]
        };

        for (var i = 0; i < result.Depths.Length; i++)
            result.Depths[i] = Depth.Read(soccketAccesser, ref currentlyRead);
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