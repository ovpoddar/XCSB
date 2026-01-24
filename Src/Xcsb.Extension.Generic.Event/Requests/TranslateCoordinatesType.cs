using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Models;

namespace Xcsb.Extension.Generic.Event.Requests;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 16)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct TranslateCoordinatesType(uint srcWindow, uint destinationWindow, ushort srcX, ushort srcY)
{
    public readonly Opcode OpCode = Opcode.TranslateCoordinates;
    private readonly byte _pad0 = 0;
    public readonly ushort Length = 4;
    public readonly uint SourceWindow = srcWindow;
    public readonly uint DestinationWindow = destinationWindow;
    public readonly ushort SourceX = srcX;
    public readonly ushort SourceY = srcY;
}