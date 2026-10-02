using System;
using Xcsb.Connection.Models;
using Xcsb.Extension.XInput.Infrastructure.ResponceProto;
using Xcsb.Extension.XInput.Infrastructure.VoidProto;
using Xcsb.Extension.XInput.Response.Replies;

namespace Xcsb.Extension.XInput.Infrastructure;

public interface IXinputRequest : IResponceProto, IVoidProto, IVoidProtoChecked, IVoidProtoUnchecked
{
    ReplyLease<GetExtensionVersionReply> GetExtensionVersion(ReadOnlySpan<byte> name);
}