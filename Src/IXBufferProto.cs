namespace Xcsb;

public interface IXBufferProto : IVoidProto
{
    void FlushChecked();
    void Flush();
}