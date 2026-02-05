using System;
using System.Collections.Generic;
using System.Text;

namespace Xcsb.Extension.Damage.Models;

public enum ReportLevel : byte
{
    RawRectangles,
    DeltaRectangles,
    BoundingBox,
    NonEmpty
}
