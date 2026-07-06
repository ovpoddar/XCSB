namespace Xcsb.Extension.XInput.Models;

public struct EventMask
{
    public DeviceInfo DeviceId;
    public ushort Length;
    public XiEventMask[] Mask;
}