using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Xcsb.Models;

namespace Src.Response.Contract;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 8)]
internal readonly struct RepliesHeader
{
    public readonly XEventType Type;
    public readonly byte Undecided;
    public readonly ushort Sequence;
    public readonly uint Length;
}
