using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Src.Models;
public enum EventsMode : byte
{
    AsyncPointer,
    SyncPointer,
    ReplayPointer,
    AsyncKeyboard,
    SyncKeyboard,
    ReplayKeyboard,
    AsyncBoth,
    SyncBoth
}
