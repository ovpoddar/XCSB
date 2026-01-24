using Xcsb.Configuration;
using Xcsb.Models.ServerConnection.Contracts;
using Xcsb.Models.ServerConnection.Handshake;

namespace Xcsb.Models.Infrastructure;

internal static class ConnectionHelper
{
    private static readonly byte[] MagicCookie = "MIT-MAGIC-COOKIE-1"u8.ToArray();

    private static string? _cachedAuthPath;
    private static readonly object AuthPathLock = new();

    internal static IXConnectionInternal TryConnect(ConnectionDetails connectionDetails,
        string display,
        XcsbClientConfiguration configuration)
    {
        var response = MakeHandshake(connectionDetails, display, configuration, [], []);
        if (response is not null && response.HandshakeStatus is HandshakeStatus.Success && response.Connected)
            return response;

        ReadOnlyMemory<char> host = connectionDetails.Host.ToArray();
        ReadOnlyMemory<char> dis = connectionDetails.Display.ToArray();

        foreach (var (authName, authData) in GetAuthInfo(host, dis))
        {
            response = MakeHandshake(connectionDetails, display, configuration, authName, authData);
            if (response is not null && response.HandshakeStatus is HandshakeStatus.Success && response.Connected)
                return response;
        }

        if (response is null)
            throw new UnauthorizedAccessException();

        if (configuration.ShouldCrashOnFailConnection)
            throw new Exception(response.FailReason.ToString());
        return response;
    }

    internal static IXConnectionInternal Connect(in ConnectionDetails connectionDetails,
        string display,
        XcsbClientConfiguration configuration,
        Span<byte> name,
        Span<byte> data)
    {
        var context = MakeHandshake(connectionDetails, display, configuration, name, data)
            ?? throw new UnauthorizedAccessException();

        if (configuration.ShouldCrashOnFailConnection && context.HandshakeStatus is not HandshakeStatus.Success)
            throw new Exception(context.FailReason.ToString());
        return context;
    }

    private static XConnection? MakeHandshake(in ConnectionDetails connectionDetails,
       string display,
       XcsbClientConfiguration configuration,
       Span<byte> authName,
       Span<byte> authData)
    {
        var connection = new XConnection(
            connectionDetails.GetSocketPath(display).ToString(),
            configuration,
            connectionDetails.Protocol);
        if (!connection.Connected)
            throw new Exception("Error connecting to X server");

        if (!connection.EstablishConnection(authName, authData))
        {
            connection.Dispose();
            return null;
        }

        connection.SetUpStatus();
        connection.ProtoOut.Sequence = 0;
        connection.ProtoIn.Sequence = 0;
        return connection;
    }

    private static string GetAuthFilePath()
    {
        Thread.MemoryBarrier();
        if (_cachedAuthPath is not null)
            return _cachedAuthPath;

        lock (AuthPathLock)
        {
            if (_cachedAuthPath is not null)
                return _cachedAuthPath;

            string result;
            var authPath = Environment.GetEnvironmentVariable("XAUTHORITY");
            if (!string.IsNullOrWhiteSpace(authPath))
            {
                result = authPath;
            }
            else
            {
                var homePath = Environment.GetEnvironmentVariable("HOME");
                if (string.IsNullOrWhiteSpace(homePath))
                    throw new InvalidOperationException("Neither XAUTHORITY nor HOME environment variables are set");

                result = Path.Combine(homePath, ".Xauthority");
            }

            Thread.MemoryBarrier();
            _cachedAuthPath = result;

            return result;
        }
    }

    // TODO: move all the logic to a separate file or a separate project for less cluster
    private static IEnumerable<(byte[] authName, byte[] authData)> GetAuthInfo(ReadOnlyMemory<char> host,
        ReadOnlyMemory<char> display)
    {
        var filePath = GetAuthFilePath();
        if (!File.Exists(filePath))
            throw new UnauthorizedAccessException("Failed to connect");

        using var fileStream = File.OpenRead(filePath);
        while (fileStream.Position <= fileStream.Length)
        {
            var context = new XAuthority(fileStream);
            var dspy = context.GetDisplayNumber(fileStream);
            var displayName = context.GetName(fileStream);

            if ((host.Span is "" or " " || host.Span.SequenceEqual(context.GetHostAddress(fileStream)))
                && (dspy is "" || dspy.SequenceEqual(display.Span))
                && displayName.SequenceEqual(MagicCookie))
                yield return (displayName, context.GetData(fileStream));
        }
    }

}