using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using Xcsb.Configuration;
using Xcsb.Handlers.Direct;
using Xcsb.Helpers;

namespace Xcsb.Models.Handshake;

public class HandshakeSuccessResponseBody
{
    public uint ReleaseNumber { get; private set; }
    public uint ResourceIDBase { get; private set; }
    public uint ResourceIDMask { get; private set; }
    public uint MotionBufferSize { get; private set; }
    public ushort MaxRequestLength { get; private set; }
    public ImageOrder ImageByteOrder { get; private set; }
    public BitOrder BitmapBitOrder { get; private set; }
    public byte BitmapScanLineUnit { get; private set; }
    public byte BitmapScanLinePad { get; private set; }
    public byte MinKeyCode { get; private set; }
    public byte MaxKeyCode { get; private set; }
    public string VendorName { get; private set; } = null!;
#if NETSTANDARD
    public Format[] Formats { get; private set; } = [];
    public Screen[] Screens { get; private set; } = [];
#else
    public required Format[] Formats { get; set; }
    public required Screen[] Screens { get; set; }
#endif
    internal static HandshakeSuccessResponseBody Read(ProtoIn protoIn, int additionalDataLength)
    {
        var readIndex = 0;
        Span<byte> scratchBuffer = stackalloc byte[Marshal.SizeOf<_handshakeSuccessResponseBody>()];
        protoIn.ReceiveExact(scratchBuffer);
        readIndex += scratchBuffer.Length;

        ref readonly var successResponseBody = ref scratchBuffer.AsStruct<_handshakeSuccessResponseBody>();
        var result = new HandshakeSuccessResponseBody
        {
            ReleaseNumber = successResponseBody.ReleaseNumber,
            ResourceIDBase = successResponseBody.ResourceIDBase,
            ResourceIDMask = successResponseBody.ResourceIDMask,
            MotionBufferSize = successResponseBody.MotionBufferSize,
            MaxRequestLength = successResponseBody.MaxRequestLength,
            ImageByteOrder = successResponseBody.ImageByteOrder,
            BitmapBitOrder = successResponseBody.BitmapBitOrder,
            BitmapScanLineUnit = successResponseBody.BitmapScanLineUnit,
            BitmapScanLinePad = successResponseBody.BitmapScanLinePad,
            MinKeyCode = successResponseBody.MinKeyCode,
            MaxKeyCode = successResponseBody.MaxKeyCode,
            Formats = new Format[successResponseBody.FormatsNumber],
            Screens = new Screen[successResponseBody.ScreensNumber]
        };
        readIndex += SetVendorName(result, protoIn, successResponseBody.VendorLength);
        readIndex += SettFormats(result, protoIn);
        for (var i = 0; i < result.Screens.Length; i++)
            result.Screens[i] = Screen.Read(protoIn, ref readIndex);
        Debug.Assert(readIndex == additionalDataLength);
        return result;
    }

    private static int SettFormats(HandshakeSuccessResponseBody result, ProtoIn protoIn)
    {
        var requireByte = result.Formats.Length * Marshal.SizeOf<Format>();
        if (requireByte < XcbClientConfiguration.StackAllocThreshold)
        {
            Span<byte> scratchBuffer = stackalloc byte[requireByte];
            protoIn.ReceiveExact(scratchBuffer);
            MemoryMarshal.Cast<byte, Format>(scratchBuffer)
                .CopyTo(result.Formats);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requireByte);
            protoIn.ReceiveExact(scratchBuffer[..requireByte]);
            MemoryMarshal.Cast<byte, Format>(scratchBuffer)
                .CopyTo(result.Formats);
        }

        return requireByte;
    }

    private static int SetVendorName(HandshakeSuccessResponseBody result, ProtoIn protoIn, int contentLength)
    {
        var length = contentLength.AddPadding();
        if (length < XcbClientConfiguration.StackAllocThreshold)
        {
            Span<byte> scratchBuffer = stackalloc byte[length];
            protoIn.ReceiveExact(scratchBuffer);
            result.VendorName = Encoding.ASCII.GetString(scratchBuffer[..contentLength]).TrimEnd();
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(length);
            protoIn.ReceiveExact(scratchBuffer[..length]);
            result.VendorName = Encoding.ASCII.GetString(scratchBuffer, 0, contentLength).TrimEnd();
        }

        return length;
    }
}

[StructLayout(LayoutKind.Sequential)]
file struct _handshakeSuccessResponseBody
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
    private int Padding { get; set; }
}