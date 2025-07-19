using System.Runtime.InteropServices;

namespace Xcsb.Models.Response;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
public readonly struct GetGeometryReply : IXBaseResponse
{
    public readonly byte ResponseType; // 1
    public readonly byte Depth;
    public readonly ushort Sequence;
    public readonly uint Length;
    public readonly uint Root;
    public readonly ushort X;
    public readonly ushort Y;
    public readonly ushort Width;
    public readonly ushort Height;
    public readonly ushort BorderWidth;
    
    public bool Verify()
    {
        return this.ResponseType == 1 && this.Length == 0;
    }
}