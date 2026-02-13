using System.Collections;
using System.Collections.Concurrent;
using System.Runtime.InteropServices;
using Xcsb.Connection.Handlers;
using Xcsb.Connection.Helpers;
using Xcsb.Connection.Infrastructure.Exceptions;
using Xcsb.Connection.Request;
using Xcsb.Connection.Response;
using Xcsb.Connection.Response.Contract;
using Xcsb.Connection.Response.Errors;
using Xcsb.Connection.Response.Replies;
using Xcsb.Connection.Response.Replies.Internals;

namespace Xcsb.Connection.Infrastructure;

internal sealed class XcsbExtensation : IXExtensationInternal
{
    private readonly SoccketAccesser _accesser;
    private int _bigRequestLength = 262140;
    private readonly ConcurrentDictionary<string, ExtensationDetails> _extensitionReply = new ConcurrentDictionary<string, ExtensationDetails>();
    private readonly ConcurrentDictionary<Type, object> _store = new ConcurrentDictionary<Type, object>();

    public SoccketAccesser Transport => _accesser;

    public XcsbExtensation(SoccketAccesser accesser)
    {
        _accesser = accesser;
    }

    public ListExtensionsReply ListExtensions()
    {
        var cookie = ListExtensionsBase();
        var (result, error) = this._accesser.ReceivedResponseSpan<ListExtensionsResponse>(cookie.Id);
        return error.HasValue
            ? throw new XEventException(error.Value)
            : new ListExtensionsReply(result);
    }

    private ResponseProto ListExtensionsBase()
    {
        var request = new ListExtensionsType();
        _accesser.SendRequest(MemoryMarshal.AsBytes(MemoryMarshal.CreateReadOnlySpan(ref request, 1)), System.Net.Sockets.SocketFlags.None);
        return new ResponseProto(_accesser.SendSequence, true);
    }

    public QueryExtensionReply QueryExtension(ReadOnlySpan<byte> name)
    {
        if (name.Length > ushort.MaxValue)
            throw new ArgumentException($"{nameof(name)} is invalid, {nameof(name)} is too long.");
        var cookie = QueryExtensionBase(name);
        var (result, error) = this._accesser.ReceivedResponseSpan<QueryExtensionReply>(cookie.Id);
        return error.HasValue
            ? throw new XEventException(error.Value)
            : result.AsSpan().ToStruct<QueryExtensionReply>();
    }


    private ResponseProto QueryExtensionBase(ReadOnlySpan<byte> name)
    {
        var request = new QueryExtensionType((ushort)name.Length);
        var requiredBuffer = request.Length * 4;
        if (requiredBuffer < _bigRequestLength)
        {
            Span<byte> scratchBuffer = stackalloc byte[requiredBuffer];
            scratchBuffer.WriteRequest(ref request, 8, name);
            _accesser.SendRequest(scratchBuffer, System.Net.Sockets.SocketFlags.None);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            var workingBuffer = scratchBuffer[..requiredBuffer];
            workingBuffer.WriteRequest(ref request, 8, name);
            _accesser.SendRequest(workingBuffer, System.Net.Sockets.SocketFlags.None);
        }

        return new ResponseProto(_accesser.SendSequence, true);
    }

    public void ActivateExtensation(ReadOnlySpan<char> name, QueryExtensionReply reply, int newError, int newEvent)
    {
        var result = new ExtensationDetails
        {
            FirstError = reply.FirstError,
            FirstEvent = reply.FirstEvent,
            ResponseHeader = reply.ResponseHeader,
            MajorOpcode = reply.MajorOpcode,
            ErrorLenght = newError,
            EventLenght = newEvent,
        };
        _extensitionReply.TryAdd(name.ToString(), result);
    }

    public bool IsExtensationEnable(string name) =>
        _extensitionReply.ContainsKey(name);

    public bool CanHandleEvent(byte replyType)
    {
        foreach (var item in _extensitionReply.Values)
        {
            var i = replyType - item.FirstEvent;
            if (item.FirstEvent == 0) continue;
            if (i > 0 && i > item.EventLenght) continue;
            return true;
        }
        return false;
    }

    public bool CanHandleError(byte replyType)
    {
        foreach (var item in _extensitionReply.Values)
        {
            var i = replyType - item.FirstError;
            if (item.FirstEvent == 0) continue;
            if (i > 0 && i > item.ErrorLenght) continue;
            return true;
        }
        return false;
    }

    public T GetOrCreate<T>(Func<T> factory) where T : class
    {
        if (_store.TryGetValue(typeof(T), out var existing))
            return (T)existing;

        var value = factory();
        _store[typeof(T)] = value;
        return value;
    }

    public void Clear()
    {
        _store.Clear();
    }
}
