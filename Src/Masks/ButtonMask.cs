﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xcsb.Masks;

[Flags]
public enum ButtonMask
{
    LeftButton = 256,
    RightButton = 512,
    MiddleButton = 1024,
    ScrollWellUp = 2048, //todo :verify
    ScrollWellDown = 4096, //todo :verify
    Any = 32768
}
