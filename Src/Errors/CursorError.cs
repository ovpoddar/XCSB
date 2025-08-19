using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Xcsb.Errors;

[StructLayout(LayoutKind.Sequential, Size = 1)]
public readonly struct CursorError
{
    public readonly ushort Sequence;
    public readonly uint BadResourceId;
    public readonly ushort MinorOpcode;
    public readonly byte MajorOpcode;
}
