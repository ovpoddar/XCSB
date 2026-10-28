using System;
using System.Runtime.InteropServices;
using Xcsb.Connection.Helpers;
using Xcsb.Connection.Response.Contract;
using Xcsb.Extension.XInput.Models;
using Xcsb.Extension.XInput.Models.TypeInfo;

namespace Xcsb.Extension.XInput.Response.Event;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
public struct ChangeDeviceNotifyEvent : IXEvent<ChangeDeviceNotifyEvent>
{
    public readonly ResponseHeader<ResponseType, byte> ResponseHeader;
    public readonly uint Time;
    public readonly ChangeDevice Request;


    public ChangeDeviceNotifyEvent FromBytes(byte[] response)
    {
        return response.AsSpan().ToStruct<ChangeDeviceNotifyEvent>();
    }
}