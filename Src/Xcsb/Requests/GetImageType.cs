using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Models;

namespace Xcsb.Requests;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 20)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct GetImageType(ImageFormat format, uint drawable, ushort @ushort, ushort y, ushort width, ushort height, uint planeMask)
{
    public readonly Opcode Opcode = Opcode.GetImage;
    public readonly ImageFormat Format = format;
    public readonly ushort Length = 5;
    public readonly uint Drawable = drawable;
    public readonly ushort X = @ushort;
    public readonly ushort Y = y;
    public readonly ushort Width = width;
    public readonly ushort Height = height;
    public readonly uint PlaneMask = planeMask;
}