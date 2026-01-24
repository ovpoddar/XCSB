using Xcsb.Extension.Generic.Event.Infrastructure.VoidProto;

namespace Xcsb.Extension.Generic.Event.Infrastructure;

public interface IXBufferProto : IVoidBufferProto
{
    void FlushChecked();
    void Flush();
}