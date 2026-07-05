using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Xcsb.Extension.XInput.Models;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 12)]
public readonly struct AxisInfo
{
    public readonly uint Resolution;
    public readonly uint MinimumValue;
    public readonly uint MaximumValue;
}