using System.Net.Sockets;
using System.Runtime.InteropServices;
using Xcsb.Models;
using Xcsb.Models.Event;

namespace Xcsb;

public static class XcsbClient
{
    public static IXProto Initialized()
    {
        var display = Environment.GetEnvironmentVariable("DISPLAY") ?? ":0";
        var connectionDetails = GetDisplayConfiguration(display);
        var connectionResult = Connection.TryConnect(connectionDetails, display);
        var result = new XProto(connectionResult.Item2, connectionResult.Item1);
        return result;
    }

    private static ConnectionDetails GetDisplayConfiguration(ReadOnlySpan<char> input)
    {
        var details = new ConnectionDetails()
        {
            DisplayNumber = 0,
            ScreenNumber = 0,
        };
        if (input.IsEmpty)
            throw new Exception("Initialized failed");

        var colonIndex = input.LastIndexOf(':');
        if (colonIndex == -1)
            throw new Exception("Initialized failed");

        if (input[0] == '/')
            details.Socket = input[..colonIndex];
        else
        {
            var slashIndex = input.IndexOf('/');
            if (slashIndex >= 0)
            {
                details.Protocol = 
#if NETSTANDARD
                    Enum.TryParse(input[..slashIndex].ToString(), true, out ProtocolType protocol)
#else
                    Enum.TryParse(input[..slashIndex], true, out ProtocolType protocol)
#endif
                    ? protocol
                    : ProtocolType.Tcp;
                details.Host = input.Slice(slashIndex + 1, colonIndex);
            }
            else
            {
                details.Host = input[..colonIndex];
            }
        }

        var displayNumberStart = input[(colonIndex + 1)..];
        if (displayNumberStart.Length == 0)
            throw new Exception("Initialized failed");

        var dotIndex = displayNumberStart.IndexOf('.');
        bool result;
        if (dotIndex < 0)
        {
            details.Display = displayNumberStart[..];
            result = int.TryParse(displayNumberStart[..], out var displayNumber);
            details.DisplayNumber = displayNumber;
        }
        else
        {
            details.Display = displayNumberStart[..dotIndex];
            var task1 = int.TryParse(displayNumberStart.Slice(0, dotIndex), out var displayNumber);
            var task2 = int.TryParse(displayNumberStart[(dotIndex + 1)..], out var screenNumber);
            details.DisplayNumber = displayNumber;
            details.ScreenNumber = screenNumber;
            result = task1 && task2;
        }

        if (!result)
            throw new Exception("Initialized failed");
        return details;
    }
}