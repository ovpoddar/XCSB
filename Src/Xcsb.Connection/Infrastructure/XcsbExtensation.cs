using System.Collections.Concurrent;
using System.Runtime.InteropServices;
using Xcsb.Connection.Handlers;
using Xcsb.Connection.Helpers;
using Xcsb.Connection.Infrastructure.Exceptions;
using Xcsb.Connection.Models;
using Xcsb.Connection.Request;
using Xcsb.Connection.Response;
using Xcsb.Connection.Response.Contract;
using Xcsb.Connection.Response.Replies;
using Xcsb.Connection.Response.Replies.Internals;

namespace Xcsb.Connection.Infrastructure;

internal sealed class XcsbExtensation : IXExtensationInternal
{
    private int _bigRequestLength = 262140;

    private readonly ConcurrentDictionary<string, QueryExtensionReply> _extensitionReply =
        new ConcurrentDictionary<string, QueryExtensionReply>();

    private readonly ConcurrentDictionary<Type, Lazy<object>> _store = new ConcurrentDictionary<Type, Lazy<object>>();

    public ISocketAccessor Transport { get; }

    public XcsbExtensation(ISocketAccessor accessor)
    {
        Transport = accessor;
    }

    public ListExtensionsReply ListExtensions()
    {
        var cookie = ListExtensionsBase();
        var (result, error) = this.Transport.ReceivedResponseSpan<ListExtensionsResponse>(cookie.Id);
        return error.HasValue
            ? throw new XEventException(error.Value)
            : new ListExtensionsReply(result);
    }

    private ResponseProto ListExtensionsBase()
    {
        var request = new ListExtensionsType();
        Transport.SendRequest(MemoryMarshal.AsBytes(MemoryMarshal.CreateReadOnlySpan(ref request, 1)),
            System.Net.Sockets.SocketFlags.None);
        return new ResponseProto(Transport.SendSequence, true);
    }

    public QueryExtensionReply QueryExtension(ReadOnlySpan<byte> name)
    {
        if (name.Length > ushort.MaxValue)
            throw new ArgumentException($"{nameof(name)} is invalid, {nameof(name)} is too long.");
        var cookie = QueryExtensionBase(name);
        var (result, error) = this.Transport.ReceivedResponseSpan<QueryExtensionReply>(cookie.Id);
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
            Transport.SendRequest(scratchBuffer, System.Net.Sockets.SocketFlags.None);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requiredBuffer);
            var workingBuffer = scratchBuffer[..requiredBuffer];
            workingBuffer.WriteRequest(ref request, 8, name);
            Transport.SendRequest(workingBuffer, System.Net.Sockets.SocketFlags.None);
        }

        return new ResponseProto(Transport.SendSequence, true);
    }

    public void ActivateExtensation(ReadOnlySpan<char> name, QueryExtensionReply reply) =>
        _extensitionReply.TryAdd(name.ToString(), reply);

    public bool IsExtensationEnable(string name) =>
        _extensitionReply.ContainsKey(name);

    public T GetOrCreate<T>(Func<T> factory) where T : class
    {
        var lazy = _store.GetOrAdd(typeof(T), new Lazy<object>(factory, LazyThreadSafetyMode.ExecutionAndPublication));
        return (T)lazy.Value;
    }

    public void Clear()
    {
        _store.Clear();
    }
}