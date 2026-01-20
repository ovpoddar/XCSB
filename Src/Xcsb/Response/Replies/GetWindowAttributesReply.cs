using System.Runtime.InteropServices;
using Xcsb.Models;
using Xcsb.Models.Handshake;
using Xcsb.Response.Contract;

namespace Xcsb.Response.Replies;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 44)]
public readonly struct GetWindowAttributesReply : IXReply
{
    public readonly ResponseHeader<BackingStores> ResponseHeader;
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
    public readonly uint AllEventMasks; // EventMask
    public readonly uint YourEventMask; // EventMask
    public readonly ushort DoNotPropagateMask; // EventMask
    private readonly ushort _pad0;

    public bool Verify(in int sequence)
    {
        return ResponseHeader.Reply == ResponseType.Reply &&
               Length == 3 && _pad0 == 0;
    }

    public BackingStores Stores => ResponseHeader.GetValue();
}