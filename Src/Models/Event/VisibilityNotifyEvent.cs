﻿using System.Runtime.InteropServices;

namespace Src.Models.Event;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct VisibilityNotifyEvent
{
    private byte Pad0;
    public ushort SequenceNumber;
    public uint Window;
    public Visibility State;
}