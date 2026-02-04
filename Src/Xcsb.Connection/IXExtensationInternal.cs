using System;
using System.Collections.Generic;
using System.Text;
using Xcsb.Connection.Handlers;
using Xcsb.Connection.Response.Contract;
using Xcsb.Connection.Response.Errors;
using Xcsb.Connection.Response.Replies;

namespace Xcsb.Connection;

internal interface IXExtensationInternal : IXExtensation
{
    SoccketAccesser Transport { get; }

    void ActivateExtensation(ReadOnlySpan<char> name, QueryExtensionReply reply);
    bool IsExtensationEnable(string name);
}
