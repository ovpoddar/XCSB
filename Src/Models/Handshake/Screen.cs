using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using static Src.Models.Handshake.Depth;

namespace Src.Models.Handshake;


internal class Screen
{
    public uint Root; // Window (XID, usually ulong)
    public uint CMap; // Colormap
    public uint WhitePixel; // pixel value for white
    public uint BlackPixel; // pixel value for black
    public int InputMask; // event mask
    public ushort Width; // screen width in pixels
    public ushort Height; // screen height in pixels
    public ushort MWidth; // width in millimeters
    public ushort MHeight; // height in millimeters
    public ushort MinMaps; // min colormaps
    public ushort MaxMaps; // max colormaps
    public uint RootVisualId; // Visual*
    public BackingStores BackingStore;
    public bool SaveUnders; // Bool (0 or non-zero)
    private byte _rootDepth;
    public Depth? RootDepth => this.Depths.FirstOrDefault(a => a.DepthValue == _rootDepth);
    public required Depth[] Depths;
    public static implicit operator Screen(_screen depth)
    {
        return new Screen()
        {
            BackingStore = depth.BackingStore,
            RootVisualId = depth.RootVisualId,
            BlackPixel = depth.BlackPixel,
            Width = depth.Width,
            Height = depth.Height,
            MWidth = depth.MWidth,
            MHeight = depth.MHeight,
            MinMaps = depth.MinMaps,
            MaxMaps = depth.MaxMaps,
            CMap = depth.CMap,
            WhitePixel = depth.WhitePixel,
            InputMask = depth.InputMask,
            _rootDepth = depth.RootDepth,
            SaveUnders = depth.SaveUnders != 0,
            Root = depth.Root,
            Depths = new Depth[depth.NumberOfDepth],
        };
    }



    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct _screen
    {
        public uint Root; // Window (XID, usually ulong)
        public uint CMap; // Colormap
        public uint WhitePixel; // pixel value for white
        public uint BlackPixel; // pixel value for black
        public int InputMask; // event mask
        public ushort Width; // screen width in pixels
        public ushort Height; // screen height in pixels
        public ushort MWidth; // width in millimeters
        public ushort MHeight; // height in millimeters
        public ushort MinMaps; // min colormaps
        public ushort MaxMaps; // max colormaps
        public uint RootVisualId; // Visual*
        public BackingStores BackingStore;
        public byte SaveUnders; // Bool (0 or non-zero)
        public byte RootDepth; // bits per pixel
        public byte NumberOfDepth; // number of supported depths
    }
    public int FillTheDepth(Span<byte> reader)
    {
        var readIndex = 0;
        for (var i = 0; i < Depths.Length; i++)
        {
            Depth depth = Unsafe.As<byte, _Depth>(ref reader[readIndex]);
            readIndex += Marshal.SizeOf<_Depth>();

            var bufferSlice = reader.Slice(readIndex, depth.Visuals.Length * Marshal.SizeOf<Visual>());
            depth.Visuals = MemoryMarshal.Cast<byte, Visual>(bufferSlice).ToArray();
            readIndex += bufferSlice.Length;
            Depths[i] = depth;
        }
        return readIndex;
    }
}