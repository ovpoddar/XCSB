using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Xcsb.Errors;

[StructLayout(layoutKind: LayoutKind.Sequential, Pack = 1)]
public readonly struct ValueError
{
    public readonly ushort Sequence;
    public readonly uint BadValue;
    public readonly ushort MinorOpcode;
    public readonly byte MajorOpcode;
}
