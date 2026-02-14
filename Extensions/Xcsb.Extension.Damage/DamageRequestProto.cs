using System.Runtime.InteropServices;
using Xcsb.Connection;
using Xcsb.Connection.Helpers;
using Xcsb.Connection.Infrastructure.Exceptions;
using Xcsb.Connection.Response;
using Xcsb.Connection.Response.Replies;
using Xcsb.Extension.Damage.Models;
using Xcsb.Extension.Damage.Requests;
using Xcsb.Extension.Damage.Response.Replies;

namespace Xcsb.Extension.Damage;

internal sealed class DamageRequestProto : IDamageRequest
{
    private readonly QueryExtensionReply _response;
    private readonly IXExtensationInternal _extensationInternal;

    public DamageRequestProto(QueryExtensionReply response, IXExtensationInternal extensationInternal)
    {
        _response = response;
        _extensationInternal = extensationInternal;
    }

    public void Add(uint drawable, uint region)
    {
        AddBase(drawable, region);
    }

    private ResponseProto AddBase(uint drawable, uint region)
    {
        var request = new DamageAddType(this._response.MajorOpcode, drawable, region);
        _extensationInternal.Transport.SendRequest(
            MemoryMarshal.AsBytes(MemoryMarshal.CreateReadOnlySpan(ref request, 1)),
            System.Net.Sockets.SocketFlags.None);
        return new ResponseProto(_extensationInternal.Transport.SendSequence, false);
    }

    public void Create(uint damage, uint drawable, ReportLevel reportLavel)
    {
        CreateBase(damage, drawable, reportLavel);
    }

    private ResponseProto CreateBase(uint damage, uint drawable, ReportLevel reportLavel)
    {
        var request = new DamageCreateType(this._response.MajorOpcode, damage, drawable, reportLavel);
        _extensationInternal.Transport.SendRequest(
            MemoryMarshal.AsBytes(MemoryMarshal.CreateReadOnlySpan(ref request, 1)),
            System.Net.Sockets.SocketFlags.None);
        return new ResponseProto(_extensationInternal.Transport.SendSequence, false);
    }

    public void Destroy(uint damage)
    {
        DestroyBase(damage);
    }

    private ResponseProto DestroyBase(uint damage)
    {
        var request = new DamageDestroyType(this._response.MajorOpcode, damage);
        _extensationInternal.Transport.SendRequest(
           MemoryMarshal.AsBytes(MemoryMarshal.CreateReadOnlySpan(ref request, 1)),
           System.Net.Sockets.SocketFlags.None);
        return new ResponseProto(_extensationInternal.Transport.SendSequence, false);
    }

    public DamageQueryVersionReply QueryVersion(uint majorVersion, uint minorVersion)
    {
        var cookie = QueryVersionBase(majorVersion, minorVersion);
        var (result, error) = this._extensationInternal.Transport.ReceivedResponseSpan<DamageQueryVersionReply>(cookie.Id);
        if (error.HasValue)
            throw new XEventException(error.Value);

        return result.AsSpan().ToStruct<DamageQueryVersionReply>();
    }

    private ResponseProto QueryVersionBase(uint majorVersion, uint minorVersion)
    {
        var request = new DamageQueryVersionType(_response.MajorOpcode, majorVersion, minorVersion);
        _extensationInternal.Transport.SendRequest(
            MemoryMarshal.AsBytes(MemoryMarshal.CreateReadOnlySpan(ref request, 1)),
            System.Net.Sockets.SocketFlags.None);
        return new ResponseProto(_extensationInternal.Transport.SendSequence, true);
    }

    public void Subtract(uint damage, uint repair, uint parts)
    {
        SubtractBase(damage, repair, parts);
    }

    private ResponseProto SubtractBase(uint damage, uint repair, uint parts)
    {
        var request = new DamageSubtractType(_response.MajorOpcode, damage, repair, parts);
        _extensationInternal.Transport.SendRequest(
            MemoryMarshal.AsBytes(MemoryMarshal.CreateReadOnlySpan(ref request, 1)),
            System.Net.Sockets.SocketFlags.None);
        return new ResponseProto(_extensationInternal.Transport.SendSequence, true);
    }
}
