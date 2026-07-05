namespace Xcsb.Extension.XInput.Models;

public enum DeviceInputMode : byte
{
    AsyncThisDevice = 0,
    SyncThisDevice = 1,
    ReplayThisDevice = 2,
    AsyncOtherDevices = 3,
    AsyncAll = 4,
    SyncAll = 5,
}