﻿using System.Runtime.InteropServices;

namespace Src.Models.Event;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct FocusEvent
{
    public NotifyDetail Detail;
    public ushort Sequence;
    public int Event;
    public NotifyMode Mode;
}