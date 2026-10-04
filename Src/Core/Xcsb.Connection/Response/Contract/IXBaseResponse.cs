using Xcsb.Connection.Helpers;

namespace Xcsb.Connection.Response.Contract;

public interface IXBaseResponse<out T> where T : struct
{
    T FromBytes(byte[] response);
}
