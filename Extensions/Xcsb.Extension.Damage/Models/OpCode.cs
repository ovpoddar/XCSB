using System;
using System.Collections.Generic;
using System.Text;

namespace Xcsb.Extension.Damage.Models;

internal enum OpCode : byte
{
    QueryVersion,
    Create,
    Destroy,
    Subtract,
    Add,

}
