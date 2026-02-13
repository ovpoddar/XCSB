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

    void ActivateExtensation(ReadOnlySpan<char> name, QueryExtensionReply reply, int newError, int newEvent);
    bool IsExtensationEnable(string name);
    bool CanHandleEvent(byte replyType);
    bool CanHandleError(byte replyType);

    T GetOrCreate<T>(Func<T> factory) where T : class;
    void Clear();
}
