using Xcsb.Connection.Helpers;

namespace Xcsb.Connection.Response.Contract;

public interface IXBaseResponse<T> where T : struct
{
    ref readonly T Cast(Span<byte> response)
    {
        return ref response.AsStruct<T>();
    }
}
