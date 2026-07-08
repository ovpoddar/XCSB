using System;
using System.Runtime.CompilerServices;
using Xcsb.Connection.Helpers;
using Xcsb.Extension.XInput.Models;

namespace Xcsb.Extension.XInput.Response.Replies;

public struct GetDeviceMotionEventsReply
{
    public readonly ResponseType Reply;
    public readonly ushort Sequence;
    public readonly byte ReplyType;
    public readonly ValuatorMode DeviceMode;
    public readonly DeviceTimeCoord[] Events;

    public GetDeviceMotionEventsReply(Span<byte> result)
    {
        ref readonly var response = ref result.AsStruct<GetDeviceMotionEventsResponse>();
        Reply = response.ResponseHeader.Reply;
        Sequence = response.ResponseHeader.Sequence;
        ReplyType = response.ResponseHeader.GetValue();
        DeviceMode = response.DeviceMode;
        if (response.NumEvents == 0)
            Events = Array.Empty<DeviceTimeCoord>();
        else
        {
            Events = new DeviceTimeCoord[response.NumEvents];
            var eventAxisLength = (int)response.NumEvents * Unsafe.SizeOf<int>();
            var index = 32;
            for (var i = 0; i < Events.Length; i++)
            {
                Events[i] = DeviceTimeCoord.Parse(result.Slice(index, eventAxisLength));
                index += eventAxisLength;
            }
        }
    }
}