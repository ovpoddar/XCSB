using System;
using System.Net.Sockets;
using Xcsb.Connection;
using Xcsb.Connection.Helpers;
using Xcsb.Connection.Infrastructure.Exceptions;
using Xcsb.Connection.Response;
using Xcsb.Connection.Response.Replies;
using Xcsb.Extension.XInput.Infrastructure;
using Xcsb.Extension.XInput.Requests;
using Xcsb.Extension.XInput.Response.Replies;

namespace Xcsb.Extension.XInput.Implementation;

internal sealed class XInputProto : IXinputRequest
{
    private readonly QueryExtensionReply _response;
    private readonly IXExtensionInternal _extensionInternal;
    const int _minStackSupport = 512;

    public XInputProto(QueryExtensionReply response, IXExtensionInternal extensionInternal)
    {
        _response = response;
        _extensionInternal = extensionInternal;
    }

    public GetExtensionVersionReply GetExtensionVersion(ReadOnlySpan<byte> name)
    {
        var cookie = GetExtensionVersionBase(name);
        var (result, error) = this._extensionInternal.Transport.SocketIn.ReceivedResponseSpan<GetExtensionVersionReply>(cookie.Id);
        return error.HasValue
            ? throw new XEventException(error.Value)
            : result.AsSpan().ToStruct<GetExtensionVersionReply>();
    }

    private ResponseProto GetExtensionVersionBase(ReadOnlySpan<byte> name)
    {
        var request = new GetExtensionVersionType(this._response.MajorOpcode, (ushort)name.Length);
        var requestSize = request.Length * 4;
        if (requestSize < _minStackSupport)
        {
            Span<byte> scratchBuffer = stackalloc byte[requestSize];
            scratchBuffer.WriteRequest(ref request, 8, name);
            _extensionInternal.Transport.SocketOut.SendRequest(scratchBuffer, SocketFlags.None);
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(requestSize);
            scratchBuffer[..requestSize].WriteRequest(ref request, 8, name);
            _extensionInternal.Transport.SocketOut.SendRequest(scratchBuffer[..requestSize], SocketFlags.None);
        }
        
        return new ResponseProto(_extensionInternal.Transport.SocketOut.Sequence);
    }
    

    public ResponseProto CloseDevice(byte deviceId)
    {
        var request = new CloseDeviceType(this._response.MajorOpcode, deviceId);
        _extensionInternal.Transport.SocketOut.Send(ref request);
        return new ResponseProto(_extensionInternal.Transport.SocketOut.Sequence);
    }

    public ResponseProto UngrabDevice(uint time, byte deviceId)
    {
        var request = new UngrabDeviceType(this._response.MajorOpcode, time, deviceId);
        _extensionInternal.Transport.SocketOut.Send(ref request);
        return new ResponseProto(_extensionInternal.Transport.SocketOut.Sequence);
    }

}