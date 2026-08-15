namespace Xcsb.Extension.XInput.Models;

public enum NotifyMode : byte
{
    Normal = 0,
    Grab = 1,
    Ungrab = 2,
    WhileGrabbed = 3,
    PassiveGrab = 4,
    PassiveUngrab = 5
}