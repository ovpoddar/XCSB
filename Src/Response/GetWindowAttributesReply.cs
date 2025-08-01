using System.Diagnostics;
using System.Net.Sockets;
using Xcsb.Helpers;
using Xcsb.Models;
using Xcsb.Models.Handshake;
using Xcsb.Response.Internals;

namespace Xcsb.Response;

public readonly struct GetWindowAttributesReply
{
    public readonly byte ResponseType;
    public readonly BackingStores Stores;
    public readonly ushort Sequence;
    public readonly uint Length;
    public readonly uint VisualId;
    public readonly ClassResponseType Class;
    public readonly Gravity BitGravity;
    public readonly Gravity WinGravity;
    public readonly uint BackingPlane;
    public readonly uint BackingPixel;
    public readonly MapState MapState;
    public readonly uint ColorMap;
    public readonly uint AllEventMasks; // EventMask
    public readonly uint YourEventMask; // EventMask
    public readonly ushort DoNotPropagateMask; // EventMask
    public readonly bool SaveUnder;
    public readonly bool MapIsInstalled;
    public readonly bool OverrideRedirect;

    internal GetWindowAttributesReply(GetWindowAttributesResponse response, Socket socket)
    {
        ResponseType = response.Type;
        Stores = response.Stores;
        Sequence = response.Sequence;
        Length = response.Length;
        VisualId = response.VisualId;
        Class = response.Class;
        BitGravity = response.BitGravity;
        WinGravity = response.WinGravity;
        BackingPixel = response.BackingPixel;
        BackingPlane = response.BackingPlane;
        MapState = response.MapState;
        ColorMap = response.ColorMap;
        SaveUnder = response.SaveUnder == 1;
        MapIsInstalled = response.MapIsInstalled == 1;
        OverrideRedirect = response.OverrideRedirect == 1;

        Span<byte> buffer = stackalloc byte[(int)(response.Length * 4)];
        Debug.Assert(response.Length * 4 == 12);
        socket.ReceiveExact(buffer);

        AllEventMasks = buffer[0..4].ToStruct<uint>();
        YourEventMask = buffer[4..8].ToStruct<uint>();
        DoNotPropagateMask = buffer[8..10].ToStruct<ushort>();
    }
}