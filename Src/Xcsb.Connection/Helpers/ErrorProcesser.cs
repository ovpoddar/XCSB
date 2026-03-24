using System.Runtime.CompilerServices;
using Xcsb.Connection.Response.Contract;

namespace Xcsb.Connection.Helpers;

internal class ErrorProcesser
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string GetErrorMessage<T>(Span<byte> data) where T : unmanaged, IXError =>
        data.ToStruct<T>().GetErrorMessage();
}