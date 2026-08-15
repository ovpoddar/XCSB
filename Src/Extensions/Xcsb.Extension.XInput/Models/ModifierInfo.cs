using System.Runtime.InteropServices;

namespace Xcsb.Extension.XInput.Models;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 16)]
public struct ModifierInfo
{
    public readonly uint Base;
    public readonly uint Latched;
    public readonly uint Locked;
    public readonly uint Effective;
}