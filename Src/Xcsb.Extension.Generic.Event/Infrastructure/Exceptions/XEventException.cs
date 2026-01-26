using Xcsb.Extension.Generic.Event.Response.Errors;

namespace Xcsb.Extension.Generic.Event.Infrastructure.Exceptions;

public sealed class XEventException : Exception
{
    internal XEventException(GenericError error, string methodName = "")
        : base(error.GetErrorMessage()) =>
        base.Source = methodName;
}