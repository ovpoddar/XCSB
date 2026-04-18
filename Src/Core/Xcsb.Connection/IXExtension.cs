using Xcsb.Connection.Response.Replies;

namespace Xcsb.Connection;

public interface IXExtension
{
    QueryExtensionReply QueryExtension(ReadOnlySpan<byte> name);
    ListExtensionsReply ListExtensions();
}
