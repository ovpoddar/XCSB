using Src.Helpers;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using static Src.Models.Handshake.Depth;
using static Src.Models.Handshake.HandshakeSuccessResponseBody;

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

    public static explicit operator Screen(_screen depth)
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

    public static Screen Read(Socket socket, ref int currentlyRead)
    {
        Span<byte> scratchBuffer = stackalloc byte[Marshal.SizeOf<_screen>()];
        socket.ReceiveExact(scratchBuffer);
        ref var fixed1 = ref scratchBuffer.AsStruct<_screen>();
        currentlyRead += scratchBuffer.Length;

        var screen = (Screen)fixed1;
        for (var i = 0; i < screen.Depths.Length; i++)
            screen.Depths[i] = Depth.Read(socket);
        return screen;
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
}