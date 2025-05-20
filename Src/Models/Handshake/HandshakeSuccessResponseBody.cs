using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
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
    public byte FormatsNumber { get; set; }
    public ImageOrder ImageByteOrder { get; set; }
    public BitOrder BitmapBitOrder { get; set; }
    public byte BitmapScanLineUnit { get; set; }
    public byte BitmapScanLinePad { get; set; }
    public byte MinKeyCode { get; set; }
    public byte MaxKeyCode { get; set; }
    public string VendorName { get; set; }
    public Format[] Formats { get; set; }
    public Screen[] Screens { get; set; }
    internal static HandshakeSuccessResponseBody Read(Socket socket)
    {
        throw new NotImplementedException();
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
            FormatsNumber = depth.FormatsNumber,
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
