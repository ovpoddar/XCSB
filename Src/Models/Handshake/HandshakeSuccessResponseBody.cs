using System.Diagnostics;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
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
    internal static HandshakeSuccessResponseBody Read(Socket socket, int additionalDataLength)
    {
        var readIndex = 0;
        Span<byte> scratchBuffer = stackalloc byte[Marshal.SizeOf<_handshakeSuccessResponseBody>()];
        socket.ReceiveExact(scratchBuffer);
        readIndex += scratchBuffer.Length;

        ref var successResponseBody = ref scratchBuffer.AsStruct<_handshakeSuccessResponseBody>();
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
        readIndex += SetVendorName(result, socket, successResponseBody.VendorLength.AddPadding());
        readIndex += SettFormats(result, socket);
        for (var i = 0; i < result.Screens.Length; i++)
            result.Screens[i] = Screen.Read(socket, ref readIndex);
        Debug.Assert(readIndex == additionalDataLength);
        return result;
    }

    private static int SettFormats(HandshakeSuccessResponseBody result, Socket socket)
    {
        var requireByte = result.Formats.Length * Marshal.SizeOf<Format>();
        if (requireByte < GlobalSetting.StackAllocThreshold)
        {
            Span<byte> scratchBuffer = stackalloc byte[requireByte];
            socket.ReceiveExact(scratchBuffer);
            MemoryMarshal.Cast<byte, Format>(scratchBuffer)
                .CopyTo(result.Formats);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requireByte);
            socket.ReceiveExact(scratchBuffer[..requireByte]);
            MemoryMarshal.Cast<byte, Format>(scratchBuffer)
                .CopyTo(result.Formats);
        }

        return requireByte;
    }

    private static int SetVendorName(HandshakeSuccessResponseBody result, Socket socket, int length)
    {
        if (length < GlobalSetting.StackAllocThreshold)
        {
            Span<byte> scratchBuffer = stackalloc byte[length];
            socket.ReceiveExact(scratchBuffer);
            result.VendorName = Encoding.ASCII.GetString(scratchBuffer).TrimEnd();
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(length);
            socket.ReceiveExact(scratchBuffer[..length]);
            result.VendorName = Encoding.ASCII.GetString(scratchBuffer).TrimEnd();
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