using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Src.Models.Handshake;
internal class HandshakeSuccessResponseBody
{
    public uint ReleaseNumber { get; set; }
    public uint ResourceIDBase { get; set; }
    public uint ResourceIDMask { get; set; }
    public uint MotionBufferSize { get; set; }
    public ushort MaxRequestLength { get; set; }
    public ImageOrder ImageByteOrder { get; set; }
    public BitOrder BitmapBitOrder { get; set; }
    public byte BitmapScanLineUnit { get; set; }
    public byte BitmapScanLinePad { get; set; }
    public byte MinKeyCode { get; set; }
    public byte MaxKeyCode { get; set; }
    public string VendorName { get; set; }
    public Format[] Formats { get; set; }
    public Screen[] Screens { get; set; }

    private const int StackAllocThreshold = 1024;
    internal static HandshakeSuccessResponseBody Read(Socket socket, short additionalDataLength)
    {
        Span<byte> scratchBuffer = stackalloc byte[Marshal.SizeOf<_handshakeSuccessResponseBody>()];
        socket.ReceiveExact(scratchBuffer);
        ref var fixed1 = ref scratchBuffer.AsStruct<_handshakeSuccessResponseBody>();
        var result = (HandshakeSuccessResponseBody)fixed1;
        result.VendorName = GetVendorName(socket, fixed1.VendorLength.AddPadding());
        result.Formats = GetFormats(socket, fixed1.FormatsNumber);
        result.Screens = new Screen[fixed1.ScreensNumber];
        for (var i = 0; i < fixed1.ScreensNumber; i++)
        {
        }
        return result;
    }

    private static Format[] GetFormats(Socket socket, int formatLength)
    {
        var requireByte = formatLength * Marshal.SizeOf<Format>();
        if (requireByte < StackAllocThreshold)
        {
            Span<byte> scratchBuffer = stackalloc byte[requireByte];
            socket.ReceiveExact(scratchBuffer);
            return MemoryMarshal.Cast<byte, Format>(scratchBuffer).ToArray();
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requireByte);
            socket.ReceiveExact(scratchBuffer[..requireByte]);
            return MemoryMarshal.Cast<byte, Format>(scratchBuffer).ToArray();
        }
    }

    private static string GetVendorName(Socket socket, int length)
    {
        if (length < StackAllocThreshold)
        {
            Span<byte> scratchBuffer = stackalloc byte[length];
            socket.ReceiveExact(scratchBuffer);
            return Encoding.ASCII.GetString(scratchBuffer);
        }
        else
    {
            using var scratchBuffer = new ArrayPoolUsing<byte>(length);
            socket.ReceiveExact(scratchBuffer[..length]);
            return Encoding.ASCII.GetString(scratchBuffer);
        }
    }

    public static explicit operator HandshakeSuccessResponseBody(_handshakeSuccessResponseBody depth)
    {
        return new HandshakeSuccessResponseBody
        {
            ReleaseNumber = depth.ReleaseNumber,
            ResourceIDBase = depth.ResourceIDBase,
            ResourceIDMask = depth.ResourceIDMask,
            MotionBufferSize = depth.MotionBufferSize,
            MaxRequestLength = depth.MaxRequestLength,
            ImageByteOrder = depth.ImageByteOrder,
            BitmapBitOrder = depth.BitmapBitOrder,
            BitmapScanLineUnit = depth.BitmapScanLineUnit,
            BitmapScanLinePad = depth.BitmapScanLinePad,
            MinKeyCode = depth.MinKeyCode,
            MaxKeyCode = depth.MaxKeyCode,
        };
    }

    internal struct _handshakeSuccessResponseBody
    {
        public uint ReleaseNumber { get; set; }
        public uint ResourceIDBase { get; set; }
        public uint ResourceIDMask { get; set; }
        public uint MotionBufferSize { get; set; }
        public short VendorLength { get; set; }
        public ushort MaxRequestLength { get; set; }
        public byte ScreensNumber { get; set; }
        public byte FormatsNumber { get; set; }
        public ImageOrder ImageByteOrder { get; set; }
        public BitOrder BitmapBitOrder { get; set; }
        public byte BitmapScanLineUnit { get; set; }
        public byte BitmapScanLinePad { get; set; }
        public byte MinKeyCode { get; set; }
        public byte MaxKeyCode { get; set; }
        public int Padding { get; set; }
    }
}
