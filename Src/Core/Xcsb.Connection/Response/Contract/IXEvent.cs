using System.Runtime.InteropServices;
using Xcsb.Connection.Helpers;

namespace Xcsb.Connection.Response.Contract;

internal interface IXEvent<out T> : IXBaseResponse<T> where T : struct
{
}