using Xcsb.Connection.Response.Contract;

namespace Xcsb.Connection.Infrastructure.Exceptions;

public sealed class XEventException : Exception
{
    internal XEventException(GenericError errorMessage, string methodName = "")
        : base(errorMessage.Message) =>
        base.Source = methodName;
}