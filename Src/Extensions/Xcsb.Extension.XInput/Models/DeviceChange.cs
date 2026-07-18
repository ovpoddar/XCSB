namespace Xcsb.Extension.XInput.Models;

public enum DeviceChange : byte
{
    Added = 0,
    Removed = 1,
    Enabled = 2,
    Disabled = 3,
    Unrecoverable = 4,
    ControlChanged = 5
}