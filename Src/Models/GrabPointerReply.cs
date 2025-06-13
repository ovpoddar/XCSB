using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Xcsb.Models;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
public readonly struct GrabPointerReply
{
    public readonly byte ResponseType;
    public readonly GrabStatus Status;
    public readonly ushort Sequence;
    public readonly uint Length;
}
