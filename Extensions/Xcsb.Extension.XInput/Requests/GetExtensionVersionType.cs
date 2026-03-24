using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Connection.Helpers;
using Xcsb.Extension.XInput.Models;

namespace Xcsb.Extension.XInput.Requests;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 8)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal struct GetExtensionVersionType(byte majorOpCode, ushort nameLength)
{
    public readonly byte MajorOpCode = majorOpCode;
    public readonly OpCode OpCode = OpCode.GetExtensionVersion;
    public readonly ushort Length = (ushort)(2 + nameLength.AddPadding() / 4);
    public readonly ushort NameLength = nameLength;
}