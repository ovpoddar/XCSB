namespace Xcsb.Extension.XInput.Models;

public struct XIDeviceInfo
{
    public DeviceInfo DeviceId;
    public DeviceType Type;
    public DeviceInfo Attachment;
    public ushort ClassLength;
    public ushort NameLength;
    public byte Enable;
    private byte _pad0;
    public string PaddedName;
    public uint[] Classes;
}