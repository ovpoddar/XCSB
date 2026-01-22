using System;
using System.Collections.Generic;
using System.Text;

namespace Xcsb;

internal interface IXConnection
{
    bool IsEventAvailable();
    void WaitForEvent();
    uint NewId();
}
