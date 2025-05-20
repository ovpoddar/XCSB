using Src.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static Src.Models.Handshake.Screen;

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
    // todo: want to use a scoket reader exrensation
    internal static HandshakeSuccessResponseBody Read(Socket socket, short additionalDataLength)
    {
        var readIndex = 0;
        Span<byte> scratchBuffer = stackalloc byte[Marshal.SizeOf<_handshakeSuccessResponseBody>()];
        socket.ReceiveExact(scratchBuffer);
        ref var fixed1 = ref scratchBuffer.AsStruct<_handshakeSuccessResponseBody>();
        readIndex += scratchBuffer.Length;

        var result = (HandshakeSuccessResponseBody)fixed1;
        readIndex += SetVendorName(result, socket, fixed1.VendorLength.AddPadding());
        readIndex += SettFormats(result, socket, fixed1.FormatsNumber);
        result.Screens = new Screen[fixed1.ScreensNumber];
        for (var i = 0; i < result.Screens.Length; i++)
            result.Screens[i] = Screen.Read(socket, ref readIndex);

        return result;
    }

    private static int SettFormats(HandshakeSuccessResponseBody result, Socket socket, int formatLength)
    {
        var requireByte = formatLength * Marshal.SizeOf<Format>();
        if (requireByte < StackAllocThreshold)
        {
            Span<byte> scratchBuffer = stackalloc byte[requireByte];
            socket.ReceiveExact(scratchBuffer);
            result.Formats = MemoryMarshal.Cast<byte, Format>(scratchBuffer).ToArray();
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requireByte);
            socket.ReceiveExact(scratchBuffer[..requireByte]);
            result.Formats = MemoryMarshal.Cast<byte, Format>(scratchBuffer).ToArray();
        }
        return requireByte;
    }

    private static int SetVendorName(HandshakeSuccessResponseBody result, Socket socket, int length)
    {
        if (length < StackAllocThreshold)
        {
            Span<byte> scratchBuffer = stackalloc byte[length];
            socket.ReceiveExact(scratchBuffer);
            result.VendorName = Encoding.ASCII.GetString(scratchBuffer);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(length);
            socket.ReceiveExact(scratchBuffer[..length]);
            result.VendorName = Encoding.ASCII.GetString(scratchBuffer);
        }
        return length;
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
