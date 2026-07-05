using System;

namespace Xcsb.Extension.XInput.Models;

[Flags]
public enum XiEventMask : uint
{
    DeviceChanged = 2,
    KeyPress = 4,
    KeyRelease = 8,
    ButtonPress = 16,
    ButtonRelease = 32,
    Motion = 64,
    Enter = 128,
    Leave = 256,
    FocusIn = 512,
    FocusOut = 1024,
    Hierarchy = 2048,
    Property = 4096,
    RawKeyPress = 8192,
    RawKeyRelease = 16384,
    RawButtonPress = 32768,
    RawButtonRelease = 65536,
    RawMotion = 131072,
    TouchBegin = 262144,
    TouchUpdate = 524288,
    TouchEnd = 1048576,
    TouchOwnership = 2097152,
    RawTouchBegin = 4194304,
    RawTouchUpdate = 8388608,
    RawTouchEnd = 16777216,
    BarrierHit = 33554432,
    BarrierLeave = 67108864
}