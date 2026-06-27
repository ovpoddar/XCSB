using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Xcsb.Extension.XInput.Models;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 8)]
public readonly struct KeyInfo : IInputInfo
{
    public readonly ClassId ClassId { get; }
    public readonly byte Length { get; }
    public readonly byte MinimumKeyCode;
    public readonly byte MaximumKeyCode;
    public readonly ushort KeyCount;
}