using System;
using Xcsb.Extension.XInput.Infrastructure.ResponseProto;
using Xcsb.Extension.XInput.Infrastructure.VoidProto;
using Xcsb.Extension.XInput.Response.Replies;

namespace Xcsb.Extension.XInput.Infrastructure;

public interface IXinputRequest : IResponseProto, IVoidProto, IVoidProtoChecked, IVoidProtoUnchecked
{
    GetExtensionVersionReply GetExtensionVersion(ReadOnlySpan<byte> name);
}