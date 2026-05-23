using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Extension.XInput.Models;

namespace Xcsb.Extension.XInput.Requests;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 36)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct XiWarpPointerType(
    byte majorOpCode,
    uint srcWin,
    uint dstWin,
    int srcX,
    int srcY,
    ushort srcWidth,
    ushort srcHeight,
    int dstX,
    int dstY,
    ushort deviceId)
{
    public readonly byte MajorOpcode = majorOpCode;
    public readonly OpCode Opcode = OpCode.XiWarpPointer;
    public readonly ushort Length = 9;
    public readonly uint SrcWin = srcWin;
    public readonly uint DstWin = dstWin;
    public readonly int SrcX = srcX;
    public readonly int SrcY = srcY;
    public readonly ushort SrcWidth = srcWidth;
    public readonly ushort SrcHeight = srcHeight;
    public readonly int DstX = dstX;
    public readonly int DstY = dstY;
    public readonly ushort DeviceId = deviceId;
}