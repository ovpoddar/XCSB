using Xcsb.Extension.XInput.Infrastructure.VoidProto;

namespace Xcsb.Extension.XInput.Infrastructure;

public interface IXInputBufferRequest : IVoidBufferProto
{
    void FlushChecked();
    void Flush();
}