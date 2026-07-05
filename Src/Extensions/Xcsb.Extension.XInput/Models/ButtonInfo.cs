using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Xcsb.Extension.XInput.Models;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 4)]
public readonly struct ButtonInfo : IInputInfo
{
    public readonly ClassId ClassId { get; }
    public readonly byte Length { get; }
    public readonly ushort ButtonCount;
}