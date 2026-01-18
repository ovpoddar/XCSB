using System.Net.Sockets;
using Xcsb.Configuration;
using Xcsb.Models;
using Xcsb.Models.Infrastructure;

namespace Xcsb;

public static class XcsbClient
{
    public static IXProto Initialized(string? display = null, XcbClientConfiguration? configuration = null)
    {
        display = string.IsNullOrWhiteSpace(display) ? Environment.GetEnvironmentVariable("DISPLAY") ?? ":0" : display;
        configuration ??= XcbClientConfiguration.Default;

        var connectionDetails = GetDisplayConfiguration(display);
        var connectionResult = Connection.TryConnect(connectionDetails, display, configuration);
        var result = new XProto(
            connectionResult.Item2,
            connectionResult.Item1,
            configuration);
        return result;
    }
    
    public static IXProto Initialized(string display, Span<byte> name, Span<byte> data, XcbClientConfiguration? configuration = null)
    {
        if (string.IsNullOrWhiteSpace(display)) throw new ArgumentNullException(nameof(display));
        if (name.IsEmpty) throw new ArgumentNullException(nameof(name));
        if (data.IsEmpty) throw new ArgumentNullException(nameof(data));
        configuration ??= XcbClientConfiguration.Default;

        var connectionDetails = GetDisplayConfiguration(display);
        var connectionResult = Connection.Connect(connectionDetails, display, configuration, name, data);
        var result = new XProto(
            connectionResult.Item2, 
            connectionResult.Item1,
            configuration);
        return result;
    }
    
    private static ConnectionDetails GetDisplayConfiguration(ReadOnlySpan<char> input)
    {
        var details = new ConnectionDetails
        {
            DisplayNumber = 0,
            ScreenNumber = 0
        };
        if (input.IsEmpty)
            throw new Exception("Initialized failed");

        var colonIndex = input.LastIndexOf(':');
        if (colonIndex == -1)
            throw new Exception("Initialized failed");

        if (input[0] == '/')
        {
            details.Socket = input[..colonIndex];
        }
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
                // todo this does not looks right 
                // verify this
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
            var task1 = int.TryParse(displayNumberStart[..dotIndex], out var displayNumber);
            var task2 = int.TryParse(displayNumberStart[(dotIndex + 1)..], out var screenNumber);
            details.DisplayNumber = displayNumber;
            details.ScreenNumber = screenNumber;
            result = task1 && task2;
        }

        return result ? details : throw new Exception("Initialized failed");
    }
}