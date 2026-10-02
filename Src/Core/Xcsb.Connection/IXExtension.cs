using Xcsb.Connection.Models;
using Xcsb.Connection.Response.Replies;

namespace Xcsb.Connection;

public interface IXExtension
{
    ReplyLease<QueryExtensionReply> QueryExtension(ReadOnlySpan<byte> name);
    ReplyLease<ListExtensionsReply> ListExtensions();
}
