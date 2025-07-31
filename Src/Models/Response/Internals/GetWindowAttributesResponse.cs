using System.Runtime.InteropServices;
using Xcsb.Models.Handshake;
using Xcsb.Models.Response.Contract;

namespace Xcsb.Models.Response.Internals;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
internal readonly struct GetWindowAttributesResponse : IXBaseResponse
{
    public readonly byte Type; // 1
    public readonly BackingStores Stores;
    public readonly ushort Sequence;
    public readonly uint Length;
    public readonly uint VisualId;
    public readonly ClassResponseType Class;
    public readonly Gravity BitGravity;
    public readonly Gravity WinGravity;
    public readonly uint BackingPlane;
    public readonly uint BackingPixel;
    public readonly byte SaveUnder;
    public readonly byte MapIsInstalled;
    public readonly MapState MapState;
    public readonly byte OverrideRedirect;
    public readonly uint ColorMap;

    public bool Verify(in int sequence)
    {
        return this.Type == 1 && this.Length == 3 && this.Sequence == sequence;
    }
}