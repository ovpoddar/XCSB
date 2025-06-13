namespace Xcsb.Models;
public enum GrabStatus : byte
{
    Success,
    AlreadyGrabbed,
    InvalidTime,
    NotViewable,
    Frozen,
}
