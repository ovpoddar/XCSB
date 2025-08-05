namespace Xcsb.Masks;

[Flags]
public enum KeyboardControlMask : int
{
    KeyClickPercent = 1,
    BellPercent = 2,
    BellPitch = 4,
    BellDuration = 8,
    Led = 16,
    LedMode = 32,
    Key = 64,
    AutoRepeatMode = 128
}