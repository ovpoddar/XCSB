using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Connection;
using Xcsb.Connection.Helpers;
using Xcsb.Connection.Infrastructure.Exceptions;
using Xcsb.Connection.Response;
using Xcsb.Connection.Response.Contract;
using Xcsb.Connection.Response.Replies;
using Xcsb.Extension.Damage.Infrastructure;
using Xcsb.Extension.Damage.Models;
using Xcsb.Extension.Damage.Requests;
using Xcsb.Extension.Damage.Response.Errors;
using Xcsb.Extension.Damage.Response.Events;
using Xcsb.Extension.Damage.Response.Replies;

namespace Xcsb.Extension.Damage;

internal sealed class DamageProto : IDamageRequest
{
    private readonly QueryExtensionReply _response;
    private readonly IXExtensionInternal _extensionInternal;

    public DamageProto(QueryExtensionReply response, IXExtensionInternal extensionInternal)
    {
        _response = response;
        _extensionInternal = extensionInternal;

        _extensionInternal.RegisterEvent<DamageNotifyEvent>(DamageErrorCode.DamageNotify,
            (byte?)(response.FirstEvent + DamageErrorCode.DamageNotify));
        _extensionInternal.RegisterError<BadDamageError>((byte)(response.FirstError + DamageErrorCode.BadDamage),
            DamageErrorCode.BadDamage);
    }

    public ResponseProto Subtract(uint damage, uint repair, uint parts) =>
        this.SubtractBase(damage, repair, parts);

    public ResponseProto Add(uint drawable, uint region) =>
        this.AddBase(drawable, region);

    public ResponseProto Destroy(uint damage) =>
        this.DestroyBase(damage);

    public ResponseProto Create(uint damage, uint drawable, ReportLevel reportLevel) =>
        this.CreateBase(damage, drawable, reportLevel);

    private ResponseProto AddBase(uint drawable, uint region)
    {
        var request = new DamageAddType(this._response.MajorOpcode, drawable, region);
        _extensionInternal.Transport.SocketOut.SendRequest(
            MemoryMarshal.AsBytes(MemoryMarshal.CreateReadOnlySpan(ref request, 1)),
            System.Net.Sockets.SocketFlags.None);
        return new ResponseProto(_extensionInternal.Transport.SocketOut.Sequence);
    }

    private ResponseProto CreateBase(uint damage, uint drawable, ReportLevel reportLevel)
    {
        var request = new DamageCreateType(this._response.MajorOpcode, damage, drawable, reportLevel);
        _extensionInternal.Transport.SocketOut.SendRequest(
            MemoryMarshal.AsBytes(MemoryMarshal.CreateReadOnlySpan(ref request, 1)),
            System.Net.Sockets.SocketFlags.None);
        return new ResponseProto(_extensionInternal.Transport.SocketOut.Sequence);
    }

    private ResponseProto DestroyBase(uint damage)
    {
        var request = new DamageDestroyType(this._response.MajorOpcode, damage);
        _extensionInternal.Transport.SocketOut.SendRequest(
            MemoryMarshal.AsBytes(MemoryMarshal.CreateReadOnlySpan(ref request, 1)),
            System.Net.Sockets.SocketFlags.None);
        return new ResponseProto(_extensionInternal.Transport.SocketOut.Sequence);
    }

    public DamageQueryVersionReply QueryVersion(uint majorVersion, uint minorVersion)
    {
        var cookie = QueryVersionBase(majorVersion, minorVersion);
        var (result, error) =
            this._extensionInternal.Transport.SocketIn.ReceivedResponseSpan<DamageQueryVersionReply>(cookie.Id);
        if (error.HasValue)
            throw new XEventException(error.Value);

        return result.AsSpan().ToStruct<DamageQueryVersionReply>();
    }

    private ResponseProto QueryVersionBase(uint majorVersion, uint minorVersion)
    {
        var request = new DamageQueryVersionType(_response.MajorOpcode, majorVersion, minorVersion);
        _extensionInternal.Transport.SocketOut.SendRequest(
            MemoryMarshal.AsBytes(MemoryMarshal.CreateReadOnlySpan(ref request, 1)),
            System.Net.Sockets.SocketFlags.None);
        return new ResponseProto(_extensionInternal.Transport.SocketOut.Sequence, true);
    }

    private ResponseProto SubtractBase(uint damage, uint repair, uint parts)
    {
        var request = new DamageSubtractType(_response.MajorOpcode, damage, repair, parts);
        _extensionInternal.Transport.SocketOut.SendRequest(
            MemoryMarshal.AsBytes(MemoryMarshal.CreateReadOnlySpan(ref request, 1)),
            System.Net.Sockets.SocketFlags.None);
        return new ResponseProto(_extensionInternal.Transport.SocketOut.Sequence, true);
    }
    
    private void SkipErrorForSequence(int sequence, bool shouldThrow, [CallerMemberName] string name = "")
    {
        if (this._extensionInternal.Transport.AvailableData == 0)
            _extensionInternal.Transport.PollRead(1000);

        _extensionInternal.Transport.SocketIn.FlushSocket();
        if (!_extensionInternal.Transport.SocketIn.ReplyBuffer.Remove(sequence, out var response))
            return;

        if (response.Item2.ResponseType != XResponseType.Error)
            throw new Exception($"Unexpected Response Found {response.Item2.ResponseType}");
        var error = new GenericError(response.Item1.AsSpan().ToStruct<XResponse>(), response.Item2.ErrorMessageAction!);
        if (shouldThrow)
            throw new XEventException(error, name);
    }

    public void CreateChecked(uint damage, uint drawable, ReportLevel reportLevel)
    {
        var cookie = CreateBase(damage, drawable, reportLevel);
        SkipErrorForSequence(cookie.Id, true);
    }

    public void DestroyChecked(uint damage)
    {
        var cookie = DestroyBase(damage);
        SkipErrorForSequence(cookie.Id, true);
    }

    public void SubtractChecked(uint damage, uint repair, uint parts)
    {
        var cookie = SubtractBase(damage, repair, parts);
        SkipErrorForSequence(cookie.Id, true);
    }

    public void AddChecked(uint drawable, uint region)
    {
        var cookie = AddBase(drawable, region);
        SkipErrorForSequence(cookie.Id, true);
    }

    public void CreateUnchecked(uint damage, uint drawable, ReportLevel reportLevel)
    {
        var cookie = CreateBase(damage, drawable, reportLevel);
        SkipErrorForSequence(cookie.Id, false);
    }

    public void DestroyUnchecked(uint damage)
    {
        var cookie = DestroyBase(damage);
        SkipErrorForSequence(cookie.Id, false);
    }

    public void SubtractUnchecked(uint damage, uint repair, uint parts)
    {
        var cookie = SubtractBase(damage, repair, parts);
        SkipErrorForSequence(cookie.Id, false);
    }

    public void AddUnchecked(uint drawable, uint region)
    {
        var cookie = AddBase(drawable, region);
        SkipErrorForSequence(cookie.Id, false);
    }
}