namespace Xcsb;
public interface IXBufferProto : IVoidProto
{
    void FlushChecked();
    Task FlushCheckedAsync();
    void Flush();
    Task FlushAsync();
}
