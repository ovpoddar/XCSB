namespace Src.Masks;

[Flags]
public enum EventMask : uint
{
    NoEventMask = 0,
    KeyPressMask = 1,
    KeyReleaseMask = 2,
    ButtonPressMask = 4,
    ButtonReleaseMask = 8,
    EnterWindowMask = 16,
    LeaveWindowMask = 32,
    PointerMotionMask = 64,
    PointerMotionHintMask = 128,
    Button1MotionMask = 256,
    Button2MotionMask = 512,
    Button3MotionMask = 1024,
    Button4MotionMask = 2048,
    Button5MotionMask = 4096,
    ButtonMotionMask = 8192,
    KeymapStateMask = 16384,
    ExposureMask = 32768,
    VisibilityChangeMask = 65536,
    StructureNotifyMask = 131072,
    ResizeRedirectMask = 262144,
    SubstructureNotifyMask = 524288,
    SubstructureRedirectMask = 1048576,
    FocusChangeMask = 2097152,
    PropertyChangeMask = 4194304,
    ColormapChangeMask = 8388608,
    OwnerGrabButtonMask = 16777216,
}
