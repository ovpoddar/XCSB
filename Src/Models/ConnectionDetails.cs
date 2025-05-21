using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace Src.Models;
internal ref struct ConnectionDetails
{
    public ReadOnlySpan<char> Socket { get; set; }
    public ReadOnlySpan<char> Host { get; set; }
    public ReadOnlySpan<char> Display { get; set; }
    public int DisplayNumber { get; set; }
    public int ScreenNumber { get; set; }
    public ProtocolType Protocol { get; set; }

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
