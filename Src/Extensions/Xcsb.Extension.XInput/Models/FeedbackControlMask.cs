using System;

namespace Xcsb.Extension.XInput.Models;

[Flags]
public enum FeedbackControlMask : uint
{
    KeyClickPercent = 1,
    String = 1,
    Integer = 1,
    AccelNum = 1,
    Percent = 2,
    AccelDenom = 2,
    Threshold = 4,
    Pitch = 4,
    Duration = 8,
    Led = 16,
    LedMode = 32,
    Key = 64,
    AutoRepeatMode = 128,
}