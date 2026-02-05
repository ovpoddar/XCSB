using System;
using System.Collections.Generic;
using System.Text;
using Xcsb.Extension.Damage.Models;
using Xcsb.Extension.Damage.Response.Replies;

namespace Xcsb.Extension.Damage;

public interface IDamageRequest
{
    DamageQueryVersionReply QueryVersion(uint majorVersion, uint minorVersion);
    void Create(uint damage, uint drawable, ReportLevel reportLavel);
    void Destroy(uint damage);
    void Subtract(uint damage, uint repair, uint parts);
    void Add(uint drawable, uint region);
}
