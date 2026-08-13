using System.Runtime.InteropServices;
using Xcsb.Connection.Helpers;

namespace Xcsb.Connection.Response.Contract;

internal interface IXEvent<T> where T : struct
{
    ref readonly T Cast(Span<byte> response);
}