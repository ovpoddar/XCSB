using System.Runtime.InteropServices;

namespace Xcsb.Models.Response;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
public readonly struct QueryExtensionReply : IXBaseResponse
{
    public readonly byte Reply;
    private readonly byte _pad0;
    public readonly ushort Sequence;
    public readonly uint Length;
    private readonly byte _present;
    public readonly byte MajorOpcode;
    public readonly byte FirstEvent;
    public readonly byte FirstError;
    public bool Present => _present == 1;

    public bool Verify()
    {
        return this.Reply == 1 && this.Length == 0 && this._pad0 == 0;
    }
}