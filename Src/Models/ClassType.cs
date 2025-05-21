using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Src.Models;

public enum ClassType : ushort
{
    CopyFromParent = 0,
    InputOutput = 1,
    InputOnly = 2,
}
