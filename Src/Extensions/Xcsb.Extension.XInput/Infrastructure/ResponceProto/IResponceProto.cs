using Xcsb.Extension.XInput.Requests;
using Xcsb.Extension.XInput.Response.Replies;

namespace Xcsb.Extension.XInput.Infrastructure.ResponceProto;

public interface IResponceProto
{
    ChangeDeviceControlReply ChangeDeviceControl(ushort controlId, byte deviceId);
    ListInputDevicesReply ListInputDevices();
}