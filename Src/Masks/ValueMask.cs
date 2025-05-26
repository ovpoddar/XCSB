namespace Xcsb.Masks;
[Flags]
public enum ValueMask : uint
{
    BackgroundPixmap = 1,
    BackgroundPixel = 2,
    BorderPixmap = 4,
    BorderPixel = 8,
    BitGravity = 16,
    WinGravity = 32,
    BackingStore = 64,
    BackingPlanes = 128,
    BackingPixel = 256,
    OverrideRedirect = 512,
    SaveUnder = 1024,
    EventMask = 2048,
    DoNotPropagateMask = 4096,
    Colormap = 8192,
    Cursor = 16384,
}

