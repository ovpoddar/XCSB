using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Src.Models;
[Flags]
public enum GCMask : int
{
    Function = 1,
    PlaneMask = 2,
    Foreground = 4,
    Background = 8,
    LineWidth = 16,
    LineStyle = 32,
    CapStyle = 64,
    JoinStyle = 128,
    FillStyle = 256,
    FillRule = 512,
    Tile = 1024,
    Stipple = 2048,
    TileStippleOriginX = 4096,
    TileStippleOriginY = 8192,
    Font = 16384,
    SubwindowMode = 32768,
    GraphicsExposures = 65536,
    ClipOriginX = 131072,
    ClipOriginY = 262144,
    ClipMask = 524288,
    DashOffset = 1048576,
    DashList = 2097152,
    ArcMode = 4194304,
}
