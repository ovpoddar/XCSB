using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xcsb.Models.Event;

namespace Xcsb;
public interface IXBufferProto : IVoidProto
{
    IEnumerable<ErrorEvent> Flush(bool ignoreReturn = false);
    Task<IEnumerable<ErrorEvent>> FlushAsync(bool ignoreReturn = false);
}
