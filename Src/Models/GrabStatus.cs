using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xcsb.Models;
public enum GrabStatus : byte
{
    Success,
    AlreadyGrabbed,
    InvalidTime,
    NotViewable,
    Frozen,
}
