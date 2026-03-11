using System;
using System.Collections.Generic;
using System.Text;
using Xcsb.Connection.Response.Contract;

namespace Xcsb.Connection.Models;

public abstract record XEventType
{
    private readonly byte _value;
    private readonly string _name;

    public XEventType(byte value, string name)
    {
        _value = value;
        _name = name;
    }

    public static implicit operator byte(XEventType type) =>
        type._value;

    public static explicit operator string (XEventType type) =>
        type._name;
}
