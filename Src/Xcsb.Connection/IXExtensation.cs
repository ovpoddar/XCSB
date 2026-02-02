using Xcsb.Connection.Response.Replies;

namespace Xcsb.Connection;

public interface IXExtensation
{
    QueryExtensionReply QueryExtension(ReadOnlySpan<byte> name);
    ListExtensionsReply ListExtensions();
}
