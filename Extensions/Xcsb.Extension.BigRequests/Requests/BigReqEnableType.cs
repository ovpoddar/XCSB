using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Text;

namespace Xcsb.Extension.BigRequests.Requests;

[StructLayout(LayoutKind.Sequential, Size = 4)]
internal struct BigReqEnableType (byte opcode)
{
    public byte OpCode = opcode;
    public readonly byte Value = 0;
    public readonly ushort Length = 1;
}
