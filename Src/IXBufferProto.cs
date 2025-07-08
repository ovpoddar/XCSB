using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xcsb.Models.Event;

namespace Xcsb;
public interface IXBufferProto : IVoidProto
{
    void FlushChecked();
    Task FlushCheckedAsync();
    void Flush();
    Task FlushAsync();
}
