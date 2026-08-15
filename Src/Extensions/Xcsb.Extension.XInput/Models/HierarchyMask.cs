namespace Xcsb.Extension.XInput.Models;

public enum HierarchyMask : uint
{
    MasterAdded = 1,
    MasterRemoved = 2,
    SlaveAdded = 4,
    SlaveRemoved = 8,
    SlaveAttached = 16,
    SlaveDetached = 32,
    DeviceEnabled = 64,
    DeviceDisabled = 128
}