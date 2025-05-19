using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Src.Models;
internal ref struct ConnectionDetails
{
    public ReadOnlySpan<char> Socket { get; set; }
    public ReadOnlySpan<char> Host { get; set; }
    public ReadOnlySpan<char> Display { get; set; }
    public ref int DisplayNumber;
    public ref int ScreenNumber;
    public ProtocolType Protocol;

    public readonly ReadOnlySpan<char> GetSocketPath(ReadOnlySpan<char> display)
    {
        if (Socket.Length != 0)
            return display;
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            return $"{Host}:{6000 + DisplayNumber}";
        else
            return $"/tmp/.X11-unix/X{DisplayNumber}";
    }
}
