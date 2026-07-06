using Xcsb.Extension.XInput.Models;

namespace Xcsb.Extension.XInput.Response.Replies;

public struct XiQueryDeviceReply
{
    public readonly ResponseType Reply;
    public readonly ushort Sequence;
    public XIDeviceInfo[] DeviceInfos;
}