using Xcsb.Infrastructure.VoidProto;

namespace Xcsb.Infrastructure;

public interface IXBufferProto : IVoidBufferProto
{
    void FlushChecked();
    void Flush();
}