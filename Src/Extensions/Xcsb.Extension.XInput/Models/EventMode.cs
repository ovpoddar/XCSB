namespace Xcsb.Extension.XInput.Models;

public enum EventMode : byte
{
    AsyncDevice = 0,
    SyncDevice = 1,
    ReplayDevice = 2,
    AsyncPairedDevice = 3,
    AsyncPair = 4,
    SyncPair = 5,
    AcceptTouch = 6,
    RejectTouch = 7,
}