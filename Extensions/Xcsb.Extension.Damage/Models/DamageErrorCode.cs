using System;
using System.Collections.Generic;
using System.Text;
using Xcsb.Connection.Models;
using Xcsb.Connection.Models.TypeInfo;

namespace Xcsb.Extension.Damage.Models;

public sealed record DamageErrorCode : XEventType
{
    private DamageErrorCode(byte value, string name) : base(value, name){}

    public static readonly DamageErrorCode BadDamage = new DamageErrorCode(0, "BadDamage");
    public static readonly DamageErrorCode DamageNotify = new DamageErrorCode(0, "DamageNotify");
    
}
