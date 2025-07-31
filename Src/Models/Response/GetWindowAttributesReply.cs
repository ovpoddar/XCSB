using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using Xcsb.Helpers;
using Xcsb.Models.Handshake;
using Xcsb.Models.Response.Contract;
using Xcsb.Models.Response.Internals;

namespace Xcsb.Models.Response;

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
        this.ResponseType = response.Type;
        this.Stores = response.Stores;
        this.Sequence = response.Sequence;
        this.Length = response.Length;
        this.VisualId = response.VisualId;
        this.Class = response.Class;
        this.BitGravity = response.BitGravity;
        this.WinGravity = response.WinGravity;
        this.BackingPixel = response.BackingPixel;
        this.BackingPlane = response.BackingPlane;
        this.MapState = response.MapState;
        this.ColorMap = response.ColorMap;
        this.SaveUnder = response.SaveUnder == 1;
        this.MapIsInstalled = response.MapIsInstalled == 1;
        this.OverrideRedirect = response.OverrideRedirect == 1;

        Span<byte> buffer = stackalloc byte[(int)(response.Length * 4)];
        Debug.Assert(response.Length * 4 == 12);
        socket.ReceiveExact(buffer);

        this.AllEventMasks = buffer[0..4].ToStruct<uint>();
        this.YourEventMask = buffer[4..8].ToStruct<uint>();
        this.DoNotPropagateMask = buffer[8..10].ToStruct<ushort>();
    }
}