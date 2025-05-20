using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Src.Models.Handshake;
public enum BackingStores : byte
{
    Never = 0,
    WhenMapped = 1,
    Always = 2
}
