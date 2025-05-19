
using Src.Models;
using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace Src;

public static class Xcsb
{
    public static IXProto Initialized()
    {
        var display = Environment.GetEnvironmentVariable("DISPLAY") ?? ":0";
        var connectionDetails = GetSocketInformation(display);
        var socket = new Socket(AddressFamily.Unix, SocketType.Stream, connectionDetails.Protocol);
        socket.Connect(new UnixDomainSocketEndPoint(connectionDetails.SocketPath.ToString()));
        if (!socket.Connected)
            throw new Exception("Initialized failed");
        
        Connection.TryConnect(socket, connectionDetails.Host, connectionDetails.Display);
        var result = new XProto(socket);
        return result;
    }

    public static async Task<IXProto> InitializedAsync()
    {
        var display = Environment.GetEnvironmentVariable("DISPLAY") ?? ":0";
        var connectionDetails = GetSocketInformation(display);
        var socket = new Socket(AddressFamily.Unix, SocketType.Stream, connectionDetails.Protocol);
        await socket.ConnectAsync(new UnixDomainSocketEndPoint(connectionDetails.SocketPath.ToString()));
        if (!socket.Connected)
            throw new Exception("Initialized failed");
        var result = new XProto(socket);
        return result;
    }

    private static ConnectionDetails GetSocketInformation(ReadOnlySpan<char> display)
    {
        var result = new ConnectionDetails();
        if (GetDisplayConfiguration(display,
            out var socket,
            out var host,
            out var dspy,
            out var displayNumber,
            out var screenNumber,
            out var protocol))
            throw new Exception("Initialized failed");
        if (socket.Length != 0)
            result.SocketPath = display;
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            result.SocketPath = $"{host}:{6000 + displayNumber}";
        else
            result.SocketPath = $"/tmp/.X11-unix/X{displayNumber}";
        result.Protocol = protocol;
        result.Host = host;
        result.Display = dspy;
        return result;
    }
    // todo: map it directly to the struct
    private static bool GetDisplayConfiguration(ReadOnlySpan<char> display,
        out ReadOnlySpan<char> socket,
        out ReadOnlySpan<char> host,
        out ReadOnlySpan<char> dspy,
        out int displayNumber,
        out int screenNumber,
        out ProtocolType protocol)
    {
        socket = ReadOnlySpan<char>.Empty;
        host = ReadOnlySpan<char>.Empty;
        displayNumber = 0;
        screenNumber = 0;
        protocol = ProtocolType.IP;
        dspy = ReadOnlySpan<char>.Empty;

        if (display.IsEmpty)
            return false;

        var colonIndex = display.LastIndexOf(':');
        if (colonIndex == -1)
            return false;

        if (display[0] == '/')
            socket = display[..colonIndex];
        else
        {
            var slashIndex = display.IndexOf('/');
            if (slashIndex >= 0)
            {
                if (!Enum.TryParse(display[..slashIndex], true, out protocol))
                    protocol = ProtocolType.Tcp;

                host = display.Slice(slashIndex + 1, colonIndex);
            }
            else
            {
                host = display[..colonIndex];
            }
        }

        var displayNumberStart = display[..];
        if (displayNumberStart.Length == 0)
            return false;

        var dotIndex = displayNumberStart.IndexOf('.');
        if (dotIndex < 0)
        {
            dspy = displayNumberStart[..];
            return int.TryParse(displayNumberStart[(dotIndex + 1)..], out displayNumber);
        }
        else
        {
            dspy = displayNumberStart[..dotIndex];
            return int.TryParse(displayNumberStart.Slice(1, dotIndex), out displayNumber)
                && int.TryParse(displayNumberStart[(dotIndex + 1)..], out screenNumber);
        }
    }
}