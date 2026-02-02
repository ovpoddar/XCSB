using System.Runtime.InteropServices;
using Xcsb.Connection.Handlers;
using Xcsb.Connection.Helpers;
using Xcsb.Connection.Infrastructure.Exceptions;
using Xcsb.Connection.Request;
using Xcsb.Connection.Response;
using Xcsb.Connection.Response.Replies;
using Xcsb.Connection.Response.Replies.Internals;

namespace Xcsb.Connection.Infrastructure;

internal sealed class XcsbExtensation : IXExtensation
{
    private SoccketAccesser _accesser;
    private int _bigRequestLength = 262140;
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

}
