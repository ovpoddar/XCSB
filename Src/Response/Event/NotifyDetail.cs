namespace Xcsb.Event;

public enum NotifyDetail : byte
{
    Ancestor,
    Virtual,
    Inferior,
    Nonlinear,
    NonlinearVirtual,
    Pointer,
    PointerRoot,
    None
}