using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Src.Models.Event;
public enum StackMode : byte
{
    Above,
    Below,
    TopIf,
    BottomIf,
    Opposite
}
