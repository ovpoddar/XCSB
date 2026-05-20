using Xcsb.Connection.Response;

namespace Xcsb.Extension.XInput.Infrastructure.VoidProto;

public interface IVoidProto
{
	ResponseProto AllowDeviceEvents(uint time, byte mode, byte deviceId);
}