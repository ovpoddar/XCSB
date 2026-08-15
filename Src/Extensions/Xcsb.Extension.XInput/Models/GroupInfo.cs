using System.Runtime.InteropServices;

namespace Xcsb.Extension.XInput.Models;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 4)]
public struct GroupInfo
{
    public readonly byte Base;
    public readonly byte Latched;
    public readonly byte Locked;
    public readonly byte Effective;
}