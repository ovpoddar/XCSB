using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using Xcsb.Connection.Helpers;

namespace Xcsb.Extension.XInput.Models;

public struct XIDeviceInfo
{
    public DeviceInfo DeviceId;
    public DeviceType Type;
    public DeviceInfo Attachment;
    public bool Enable;
    public string Name;
    public uint[] Classes;

    public static XIDeviceInfo Parse(Span<byte> span, ref int index)
    {
        ref readonly var response = ref span.AsStruct<_xideviceInfo>();
        index += Unsafe.SizeOf<_xideviceInfo>();
        var result = new XIDeviceInfo()
        {
            DeviceId = response.DeviceId,
            Type = response.Type,
            Attachment = response.Attachment,
            Enable = response.Enable == 1,
        };
        
        result.Name = Encoding.UTF8.GetString(span.Slice(index, response.NameLength));
        index += response.NameLength.AddPadding();
        
        result.Classes = MemoryMarshal.Cast<byte, uint>(span.Slice(index, response.ClassLength)).ToArray();
        index += response.ClassLength.AddPadding();
        return result;
    }
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
file readonly struct _xideviceInfo
{
    public readonly DeviceInfo DeviceId;
    public readonly DeviceType Type;
    public readonly DeviceInfo Attachment;
    public readonly ushort ClassLength;
    public readonly ushort NameLength;
    public readonly byte Enable;
    private readonly byte _pad0;
}