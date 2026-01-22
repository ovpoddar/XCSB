using Xcsb.Infrastructure.VoidProto;

namespace Xcsb;

public interface IXBufferProto : IVoidBufferProto
{
    void FlushChecked();
    void Flush();
}