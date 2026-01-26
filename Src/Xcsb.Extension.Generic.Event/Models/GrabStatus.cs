namespace Xcsb.Extension.Generic.Event.Models;

public enum GrabStatus : byte
{
    Success,
    AlreadyGrabbed,
    InvalidTime,
    NotViewable,
    Frozen
}