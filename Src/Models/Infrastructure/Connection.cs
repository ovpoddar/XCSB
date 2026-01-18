using System;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using Xcsb.Configuration;
using Xcsb.Helpers;
using Xcsb.Models;
using Xcsb.Models.Handshake;
using Xcsb.Requests;
using Xcsb.Response.Contract;

namespace Xcsb.Models.Infrastructure;

internal static class Connection
{
    private static readonly byte[] MagicCookie = "MIT-MAGIC-COOKIE-1"u8.ToArray();

    private static string? _cachedAuthPath;
    private static readonly object AuthPathLock = new();

    internal static (HandshakeSuccessResponseBody, ClientConnectionContext) TryConnect(ConnectionDetails connectionDetails,
        string display,
        XcbClientConfiguration configuration)
    {
        ReadOnlySpan<char> error = [];
        var (response, context) = Connect(connectionDetails, display, configuration, [], [], ref error);
        if (response is not null && context is not null)
            return (response, context);

        ReadOnlyMemory<char> host = connectionDetails.Host.ToArray();
        ReadOnlyMemory<char> dis = connectionDetails.Display.ToArray();

        foreach (var (authName, authData) in GetAuthInfo(host, dis))
        {
            (response, context) = Connect(connectionDetails, display, configuration, authName, authData, ref error);
            if (response is not null && context is not null)
                return (response, context);
        }

        throw new UnauthorizedAccessException(error.ToString());
    }

    internal static (HandshakeSuccessResponseBody, ClientConnectionContext) Connect(in ConnectionDetails connectionDetails,
        string display,
        XcbClientConfiguration configuration,
        Span<byte> name,
        Span<byte> data)
    {
        ReadOnlySpan<char> error = [];
        var (response, context) = Connect(connectionDetails, display, configuration, name, data, ref error);
        return response is null || context is null 
            ? throw new UnauthorizedAccessException(error.ToString()) 
            : (response, context);
    }

    private static (HandshakeSuccessResponseBody?, ClientConnectionContext?) Connect(in ConnectionDetails connectionDetails,
        string display,
        XcbClientConfiguration configuration,
        Span<byte> name,
        Span<byte> data,
        ref ReadOnlySpan<char> error)
    {
        var (response, context) = MakeHandshake(connectionDetails, display, configuration, name, data);
        if (response.HandshakeStatus is HandshakeStatus.Success && context is not null)
        {
            var successBody = HandshakeSuccessResponseBody.Read(context.ProtoIn,
                response.HandshakeResponseHeadSuccess.AdditionalDataLength * 4);
            return (successBody, context);
        }

        if (context is not null)
            error = response.GetStatusMessage(context.Socket);
        context?.Dispose();
        return (null, null);
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

    private static (HandshakeResponseHead, ClientConnectionContext?) MakeHandshake(in ConnectionDetails connectionDetails,
       string display,
       XcbClientConfiguration configuration,
       Span<byte> authName,
       Span<byte> authData)
    {
        var connection = new ClientConnectionContext(
            connectionDetails.GetSocketPath(display).ToString(),
            configuration,
            connectionDetails.Protocol);
        if (!connection.Connected)
            throw new Exception("Error connecting to X server");

        if (!connection.EstablishConnection(authName, authData))
            return (new HandshakeResponseHead(), null);
        
        Span<byte> tempBuffer = stackalloc byte[Marshal.SizeOf<HandshakeResponseHead>()];
        connection.ProtoIn.ReceiveExact(tempBuffer);
        return (tempBuffer.ToStruct<HandshakeResponseHead>(), connection);
    }

}