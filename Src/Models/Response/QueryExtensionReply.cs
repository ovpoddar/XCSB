using System.Runtime.InteropServices;
using Xcsb.Models.Response.Contract;

namespace Xcsb.Models.Response;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
public readonly struct QueryExtensionReply : IXBaseResponse
{
    public readonly ResponseHeader ResponseHeader;
    private readonly byte _present;
    public readonly byte MajorOpcode;
    public readonly byte FirstEvent;
    public readonly byte FirstError;
    public bool Present => _present == 1;

    public bool Verify()
    {
        return this.ResponseHeader.Verify() && this.ResponseHeader.Length == 0;
    }
}