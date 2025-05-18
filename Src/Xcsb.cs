
using Src.Models;
using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace Src;

public class Xcsb
{

    public IXProto Initialized()
    {
        var display = Environment.GetEnvironmentVariable("DISPLAY") ?? ":0";
        var connectionDetails = GetSocketInformation(display);
        var socket = new Socket(AddressFamily.Unix, SocketType.Stream, connectionDetails.Protocol);
        socket.Connect(new UnixDomainSocketEndPoint(connectionDetails.SocketPath.ToString()));
        if (!socket.Connected)
            throw new Exception("Initialized failed");
        var result = new XProto(socket);
        return result;
    }

    public async Task<IXProto> InitializedAsync()
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
    private ConnectionDetails GetSocketInformation(ReadOnlySpan<char> display)
    {
        var result = new ConnectionDetails();
        if (GetDisplayConfiguration(display,
            out var socket,
            out var host,
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
        return result;
    }

    private bool GetDisplayConfiguration(ReadOnlySpan<char> display,
        out ReadOnlySpan<char> socket,
        out ReadOnlySpan<char> host,
        out int displayNumber,
        out int screenNumber,
        out ProtocolType protocol)
    {
        socket = ReadOnlySpan<char>.Empty;
        host = ReadOnlySpan<char>.Empty;
        displayNumber = 0;
        screenNumber = 0;
        protocol = ProtocolType.IP;

        if (display.IsEmpty)
            return false;

        var colonIndex = display.LastIndexOf(':');
        if (colonIndex == -1)
            return false;

        if (display[0] == '/')
            socket = display.Slice(0, colonIndex);
        else
        {
            var slashIndex = display.IndexOf('/');
            if (slashIndex >= 0)
            {
                if (!Enum.TryParse(display.Slice(0, slashIndex).ToString(), true, out protocol))
                    protocol = ProtocolType.Tcp;

                host = display.Slice(slashIndex + 1, colonIndex);
            }
            else
            {
                host = display.Slice(0, colonIndex);
            }
        }

        var displayNumberStart = display.Slice(0);
        if (displayNumberStart.Length == 0)
            return false;

        var dotIndex = displayNumberStart.IndexOf('.');
        if (dotIndex < 0)
        {
            return int.TryParse(displayNumberStart.Slice(dotIndex + 1), out displayNumber);
        }
        else
        {
            return int.TryParse(displayNumberStart.Slice(1, dotIndex), out displayNumber)
                && int.TryParse(displayNumberStart.Slice(dotIndex + 1), out screenNumber);
        }
    }
}