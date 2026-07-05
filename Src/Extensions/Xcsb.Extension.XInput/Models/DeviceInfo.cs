using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using Xcsb.Connection.Helpers;
using Xcsb.Models;

namespace Xcsb.Extension.XInput.Models;

public struct DeviceInfo
{
    public readonly ATOM DeviceType { get; }
    public readonly byte DeviceId { get; }
    public readonly byte NumClassInfo { get; }
    public readonly DeviceUsage DeviceUse { get; }
    public readonly IInputInfo[] InputInfo { get; }
    public string Name { get; set; }

    private DeviceInfo(_DeviceInfo deviceInfo)
    {
        DeviceType = deviceInfo.DeviceType;
        DeviceId = deviceInfo.DeviceId;
        NumClassInfo = deviceInfo.NumClassInfo;
        DeviceUse = deviceInfo.DeviceUse;
        InputInfo = new IInputInfo[NumClassInfo];
    }

    internal static DeviceInfo Read(Span<byte> stream, ref int offset)
    {
        ref readonly var deviceInfo = ref stream[offset..].AsStruct<_DeviceInfo>();
        offset += Unsafe.SizeOf<_DeviceInfo>();
        var result = new DeviceInfo(deviceInfo);
        return result;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 8)]
    private struct _DeviceInfo
    {
        public ATOM DeviceType;
        public byte DeviceId;
        public byte NumClassInfo;
        public DeviceUsage DeviceUse;
    }
}