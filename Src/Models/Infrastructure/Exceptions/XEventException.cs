using Xcsb.Response.Errors;

namespace Xcsb.Models.Infrastructure.Exceptions;

public sealed class XEventException : Exception
{
    internal XEventException(GenericError error, string methodName = "")
        : base(error.GetErrorMessage()) =>
        base.Source = methodName;
}