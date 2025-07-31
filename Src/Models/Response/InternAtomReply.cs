using System.Runtime.InteropServices;
using Xcsb.Models.Response.Contract;

namespace Xcsb.Models.Response;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
public readonly struct InternAtomReply : IXBaseResponse
{
    public readonly ResponseHeader ResponseHeader;
    public readonly uint Atom;

    public bool Verify()
    {
        return this.ResponseHeader.Verify() && this.ResponseHeader.Length == 0;
    }
}