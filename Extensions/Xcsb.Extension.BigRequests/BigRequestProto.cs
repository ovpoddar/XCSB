using System.Runtime.InteropServices;
using Xcsb.Connection;
using Xcsb.Connection.Helpers;
using Xcsb.Connection.Infrastructure.Exceptions;
using Xcsb.Connection.Models;
using Xcsb.Connection.Response;
using Xcsb.Connection.Response.Replies;
using Xcsb.Extension.BigRequests.Requests;
using Xcsb.Extension.BigRequests.Response;

namespace Xcsb.Extension.BigRequests;

internal sealed class BigRequestProto : IBigRequest
{
    private readonly IXExtensationInternal _extensation;
    private readonly QueryExtensionReply _response;

    public BigRequestProto(QueryExtensionReply response, IXExtensationInternal extensation)
    {
        _response = response;
        _extensation = extensation;
    }

    public BigReqEnableReply BigRequestsEnable()
    {
        var cookie = BigRequestsEnableBase();
        var (result, error) = this._extensation.Transport.ReceivedResponseSpan<BigReqEnableReply>(cookie.Id);
        if (error.HasValue)
            throw new XEventException(error.Value);

        _extensation.ActivateExtensation(BigRequestExtensation.ExtensationName, _response, 0, 0);
        return result.AsSpan().ToStruct<BigReqEnableReply>();
    }

    private ResponseProto BigRequestsEnableBase()
    {
        var request = new BigReqEnableType(_response.MajorOpcode);
        _extensation.Transport.SendRequest(
            MemoryMarshal.AsBytes(MemoryMarshal.CreateReadOnlySpan(ref request, 1)),
            System.Net.Sockets.SocketFlags.None);
        return new ResponseProto(_extensation.Transport.SendSequence, true);
    }
}