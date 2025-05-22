using System;
using System.Runtime.InteropServices;

namespace Src.Models.Event;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct MotionEvent
{
    public Motion Detail;
    public ushort Sequence;
    public uint Time;
    public int Root;
    public int Event;
    public int Child;
    public short RootX;
    public short RootY;
    public short EventX;
    public short EventY;
    public ushort State;
    public sbyte SameScreen; // 1 true 0 false
}