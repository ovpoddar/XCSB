﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xcsb.Models;
public enum Button : byte
{
    Any = 0,
    LeftButton = 1,
    RightButton = 2,
    MiddleButton = 3,
    ScrollWellUp = 4, //todo :verify
    ScrollWellDown = 5, //todo :verify
}
