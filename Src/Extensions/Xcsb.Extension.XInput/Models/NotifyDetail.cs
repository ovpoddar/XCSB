namespace Xcsb.Extension.XInput.Models;

public enum NotifyDetail : byte
{
    Ancestor = 0,
    Virtual = 1,
    Inferior = 2,
    Nonlinear = 3,
    NonlinearVirtual = 4,
    Pointer = 5,
    PointerRoot = 6,
    None = 7
}