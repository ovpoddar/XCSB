using System.Runtime.InteropServices;
using Xcsb.Models;

namespace Xcsb.Extension.XInput.Models;

[StructLayout( LayoutKind.Sequential, Pack = 1, Size = 8)]
public struct GrabModifierInfo
{
    public int Modifier;
    public GrabStatus Status;
}