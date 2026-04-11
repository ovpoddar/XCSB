using System.Runtime.InteropServices;
using Xcsb.Connection;
using Xcsb.Connection.Helpers;
using Xcsb.Connection.Infrastructure.Exceptions;
using Xcsb.Connection.Response;
using Xcsb.Connection.Response.Replies;
using Xcsb.Extension.BigRequests.Requests;
using Xcsb.Extension.BigRequests.Response;

namespace Xcsb.Extension.BigRequests;

internal sealed class BigRequestProto : IBigRequest
{
    private readonly IXExtensionInternal _extension;
    private readonly QueryExtensionReply _response;

    public BigRequestProto(QueryExtensionReply response, IXExtensionInternal extension)
    {
        _response = response;
        _extension = extension;
    }

    public BigReqEnableReply BigRequestsEnable()
    {
        var cookie = BigRequestsEnableBase();
        var (result, error) = this._extension.Transport.SocketIn.ReceivedResponseSpan<BigReqEnableReply>(cookie.Id);
        if (error.HasValue)
            throw new XEventException(error.Value);

        _extension.ActivateExtension(BigRequestExtension.ExtensionName, _response);
        return result.AsSpan().ToStruct<BigReqEnableReply>();
    }

    private ResponseProto BigRequestsEnableBase()
    {
        var request = new BigReqEnableType(_response.MajorOpcode);
        _extension.Transport.SocketOut.SendRequest(
            MemoryMarshal.AsBytes(MemoryMarshal.CreateReadOnlySpan(ref request, 1)),
            System.Net.Sockets.SocketFlags.None);
        return new ResponseProto(_extension.Transport.SocketOut.Sequence, true);
    }
}