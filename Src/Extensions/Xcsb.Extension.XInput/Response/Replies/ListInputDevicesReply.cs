using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using Xcsb.Connection.Helpers;
using Xcsb.Extension.XInput.Models;
using Xcsb.Extension.XInput.Response.Replies.Internals;
using Xcsb.Models;

namespace Xcsb.Extension.XInput.Response.Replies;

public struct ListInputDevicesReply
{
    public readonly byte DevicesLength;
    public readonly DeviceInfo[] DeviceInfos;

    internal ListInputDevicesReply(Span<byte> data)
    {
        ref readonly var context = ref data.AsStruct<ListInputDevicesResponse>();
        DevicesLength = context.DevicesLen;
        DeviceInfos = DevicesLength == 0
            ? Array.Empty<DeviceInfo>()
            : new DeviceInfo[DevicesLength];
        
        var readIndex = Unsafe.SizeOf<ListInputDevicesResponse>();
        for (var i = 0; i < DeviceInfos.Length; i++)
            DeviceInfos[i] = DeviceInfo.Read(data, ref readIndex);

        for (var i = 0; i < DeviceInfos.Length; i++)
        {
            ref var info = ref DeviceInfos[i];
            for (var j = 0; j < info.NumClassInfo; j++)
            {
                ref readonly var skipper = ref data[readIndex..].AsStruct<Skipper>();
                info.InputInfo[j] = skipper.ClassId switch
                {
                    ClassId.Key => data[readIndex..].AsStruct<KeyInfo>(),
                    ClassId.Button => data[readIndex..].AsStruct<ButtonInfo>(),
                    ClassId.Valuator => new ValuatorInfo(data[readIndex..]),
                    _ => info.InputInfo[j]
                };
                readIndex += skipper.Length;
            }
        }
        for (var i = 0; i < DeviceInfos.Length; i++)
        {
            ref var device = ref DeviceInfos[i];
            var nullbyte = data[readIndex++];
            device.Name = nullbyte == 0
                ? string.Empty
                : Encoding.ASCII.GetString(data.Slice(readIndex, nullbyte));
            readIndex += nullbyte;
        }
    }
}

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 2)]
file readonly struct Skipper
{
    public readonly ClassId ClassId;
    public readonly byte Length;
}