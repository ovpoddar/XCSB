using System;
using System.Collections.Generic;
using System.Text;

namespace Xcsb.Response.Contract;
internal interface IGenericResponse
{
    T? ToError<T>() where T : struct, IXError;
    T? ToEvent<T>() where T : struct, IXEvent;
    T? ToReply<T>() where T : struct, IXReply;
}
