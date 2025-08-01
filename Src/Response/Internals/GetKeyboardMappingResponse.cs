using System.Runtime.InteropServices;
using Xcsb.Response.Contract;

namespace Xcsb.Response.Internals;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
internal readonly struct GetKeyboardMappingResponse : IXBaseResponse
{
    public readonly byte Reply;
    public readonly byte KeyPerKeyCode;
    public readonly ushort Sequence;
    public readonly uint Length;

    public bool Verify(in int sequence)
    {
        return Reply == 1 && Sequence == sequence;
    }
}