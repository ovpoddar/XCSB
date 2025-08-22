using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Xcsb.Errors;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public readonly struct RequestError
{
    public readonly ushort Sequence;
    private readonly uint _pad0;
    public readonly ushort MinorOpcode;
    public readonly byte MajorOpcode;
}
