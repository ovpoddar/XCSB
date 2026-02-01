using Xcsb.Connection.Response.Errors;

namespace Xcsb.Connection.Infrastructure.Exceptions;

public sealed class XEventException : Exception
{
    internal XEventException(GenericError error, string methodName = "")
        : base(error.GetErrorMessage()) =>
        base.Source = methodName;
}