using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using Xcsb.Models.Handshake;

namespace Xcsb.Models.Response;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 44)]
public readonly struct GetWindowAttributesReply
{
    public readonly byte ResponseType; // 1
    public readonly BackingStores Stores;
    public readonly ushort Sequence;
    public readonly uint Length;
    public readonly uint VisualId;
    public readonly ClassResponseType Class;
    public readonly byte BitGravity;
    public readonly byte WinGravity;
    public readonly uint BackingPlane;
    public readonly uint BackingPixel;
    private readonly byte _saveUnder;
    private readonly byte _mapIsInstalled;
    public readonly MapState MapState;
    private readonly byte _overrideRedirect;
    public readonly uint Colormap;
    public readonly uint AllEventMasks;
    public readonly uint YourEventMask;
    public readonly ushort DoNotPropagateMask;


    public bool SaveUnder => _saveUnder == 1;
    public bool MapIsInstalled => _mapIsInstalled == 1;
    public bool OverrideRedirect => _overrideRedirect == 1;

    internal bool Verify()
    {
        return this.ResponseType == 1 && this.Length == 3;
    }
}