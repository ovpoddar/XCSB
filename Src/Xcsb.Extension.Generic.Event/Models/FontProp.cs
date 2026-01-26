using System.Runtime.InteropServices;

namespace Xcsb.Extension.Generic.Event.Models;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 8)]
public struct FontProp(uint atomName, uint value)
{
    public uint AtomName = atomName;
    public uint Value = value;
}