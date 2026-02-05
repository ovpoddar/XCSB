using System;
using System.Collections.Generic;
using System.Text;
using Xcsb.Extension.Damage.Models;

namespace Xcsb.Extension.Damage;

public interface IDamageRequest
{
    int QueryVersion(uint majorVersion, uint minorVersion);
    void Create(uint Damage, uint drawable, ReportLevel reportLavel);
    void Destroy(uint damage);
    void Subtract(uint damage, uint repair, uint parts);
    void Add(uint drawable, uint region);
}
