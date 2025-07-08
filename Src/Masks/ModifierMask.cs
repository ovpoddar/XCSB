using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xcsb.Masks;
public enum ModifierMask : ushort
{
    Shift = 1,
    Lock = 2,
    Control = 4,
    LeftButton = 8,
    RightButton = 16,
    MiddleButton = 32,
    ScrollWellUp = 64, // todo: verify
    ScrollWellDown = 128, // todo: verify
    Any = 32768,
}
