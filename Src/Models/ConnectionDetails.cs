using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Src.Models;
internal ref struct ConnectionDetails
{
    public ReadOnlySpan<char> SocketPath { get; set; }
    public ProtocolType Protocol { get; set; }

}
