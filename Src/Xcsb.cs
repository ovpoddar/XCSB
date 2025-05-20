
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
        socket.Connect(new UnixDomainSocketEndPoint(connectionDetails.GetSocketPath(display).ToString()));
        if (!socket.Connected)
            throw new Exception("Initialized failed");

        var connectionResult = Connection.TryConnect(socket, connectionDetails.Host, connectionDetails.Display);
        var result = new XProto(socket, connectionResult);
        return result;
    }

    //public static async Task<IXProto> InitializedAsync()
    //{
    //    var display = Environment.GetEnvironmentVariable("DISPLAY") ?? ":0";
    //    var connectionDetails = GetSocketInformation(display);
    //    var socket = new Socket(AddressFamily.Unix, SocketType.Stream, connectionDetails.Protocol);
    //    await socket.ConnectAsync(new UnixDomainSocketEndPoint(connectionDetails.GetSocketPath(display).ToString()));
    //    if (!socket.Connected)
    //        throw new Exception("Initialized failed");

    //    await Connection.TryConnectAsync(socket, connectionDetails.Host, connectionDetails.Display);
    //    var result = new XProto(socket);
    //    return result;
    //}

    private static ConnectionDetails GetSocketInformation(ReadOnlySpan<char> display)
    {
        if (GetDisplayConfiguration(display,
            out var result))
            throw new Exception("Initialized failed");
        return result;
    }

    private static bool GetDisplayConfiguration(ReadOnlySpan<char> display,
        out ConnectionDetails details)
    {
        details = new ConnectionDetails()
        {
            DisplayNumber = 0,
            ScreenNumber = 0,
        };


        if (display.IsEmpty)
            return false;

        var colonIndex = display.LastIndexOf(':');
        if (colonIndex == -1)
            return false;

        if (display[0] == '/')
            details.Socket = display[..colonIndex];
        else
        {
            var slashIndex = display.IndexOf('/');
            if (slashIndex >= 0)
            {
                if (Enum.TryParse(display[..slashIndex], true, out ProtocolType protocol))
                    details.Protocol = protocol;
                else
                    details.Protocol = ProtocolType.Tcp;

                details.Host = display.Slice(slashIndex + 1, colonIndex);
            }
            else
            {
                details.Host = display[..colonIndex];
            }
        }

        var displayNumberStart = display[..];
        if (displayNumberStart.Length == 0)
            return false;

        var dotIndex = displayNumberStart.IndexOf('.');
        if (dotIndex < 0)
        {
            details.Display = displayNumberStart[..];
            var result = int.TryParse(displayNumberStart[(dotIndex + 1)..], out var displayNumber);
            details.DisplayNumber = displayNumber;
            return result;
        }
        else
        {
            details.Display = displayNumberStart[..dotIndex];
            var task1 = int.TryParse(displayNumberStart.Slice(1, dotIndex), out var displayNumber);
            var task2 = int.TryParse(displayNumberStart[(dotIndex + 1)..], out var screenNumber);
            details.DisplayNumber = displayNumber;
            details.ScreenNumber = screenNumber;
            return task1 && task2;
        }
    }
}