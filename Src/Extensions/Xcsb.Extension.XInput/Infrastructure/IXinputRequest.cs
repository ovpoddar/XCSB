using System;
using Xcsb.Extension.XInput.Infrastructure.ResponceProto;
using Xcsb.Extension.XInput.Infrastructure.VoidProto;
using Xcsb.Extension.XInput.Response.Replies;

namespace Xcsb.Extension.XInput.Infrastructure;

public interface IXinputRequest : IVoidProto, IResponceProto
{
    GetExtensionVersionReply GetExtensionVersion(ReadOnlySpan<byte> name);
}