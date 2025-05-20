using Src.Helpers;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using static Src.Models.Handshake.Depth;
using static Src.Models.Handshake.HandshakeSuccessResponseBody;

namespace Src.Models.Handshake;


internal class Screen
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

    public static Screen Read(Socket socket, ref int currentlyRead)
    {
        Span<byte> scratchBuffer = stackalloc byte[Marshal.SizeOf<_screen>()];
        socket.ReceiveExact(scratchBuffer);
        currentlyRead += scratchBuffer.Length;

        Screen screen = scratchBuffer.ToStruct<_screen>();
        for (var i = 0; i < screen.Depths.Length; i++)
            screen.Depths[i] = Depth.Read(socket, ref currentlyRead);
        return screen;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct _screen
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
}