using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xcsb.Models.Event;

namespace Xcsb;
public interface IXBufferProto : IVoidProto
{
    IEnumerable<ErrorEvent> Flush();
    Task<IEnumerable<ErrorEvent>> FlushAsync();
}
