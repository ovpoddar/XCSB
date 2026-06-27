using System;
using System.Diagnostics;
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

        var readIndex = 32;
        for (var i = 0; i < DeviceInfos.Length; i++)
            DeviceInfos[i] = DeviceInfo.Read(data, ref readIndex);

        foreach (var info in DeviceInfos)
        {
            for (var i = 0; i < info.NumClassInfo; i++)
            {
                ref readonly var skipper = ref data[readIndex..].AsStruct<Skipper>();
                switch (skipper.ClassId)
                {
                    case ClassId.Key:
                        info.InputInfo[i] = data[readIndex..].AsStruct<KeyInfo>();
                        break;
                    case ClassId.Button:
                        info.InputInfo[i] = data[readIndex..].AsStruct<ButtonInfo>();
                        break;
                    case ClassId.Valuator:
                        info.InputInfo[i] = new ValuatorInfo(data[readIndex..]);
                        break;
                }

                readIndex += skipper.Length;
            }
        }

        for (var i = 0; i < DeviceInfos.Length; i++)
        {
            var nullbyte = data[readIndex++];
            DeviceInfos[i].Name = nullbyte == 0
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