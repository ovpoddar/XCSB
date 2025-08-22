using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Xcsb.Errors;

[StructLayout(LayoutKind.Sequential, Size = 1)]
public readonly struct AtomError
{
    public readonly ushort Sequence;
    public readonly uint BadAtomId;
    public readonly ushort MinorOpcode;
    public readonly byte MajorOpcode;
}
