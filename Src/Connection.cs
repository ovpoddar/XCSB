using Src.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Src.Models;
using Src.Models.Handshake;
using System.Diagnostics;

namespace Src;

internal static class Connection
{
    private const string MAGICCOOKIE = "MIT-MAGIC-COOKIE-1";
    internal static HandshakeSuccessResponseBody TryConnect(Socket socket, ReadOnlySpan<char> host, ReadOnlySpan<char> display)
    {
        var result = MakeHandshake(socket, [], []);
        if (result.HandshakeStatus == HandshakeStatus.Authenticate)
        {
            Debug.WriteLine($"Connection: Authenticate fail {result.GetStatusMessage(socket)}");
            var (authName, authData) = GetAuthInfo(host, display);
            result = MakeHandshake(socket, authName, authData);
        }
        else if (result.HandshakeStatus == HandshakeStatus.Failed)
            throw new Exception(result.GetStatusMessage(socket).ToString());

        if (result.HandshakeStatus != HandshakeStatus.Success)
            throw new Exception("Could not connect to x11");

        var successResponseBody = HandshakeSuccessResponseBody.Read(socket, result.HandshakeResponseHeadSuccess.AdditionalDataLength * 4);
        return successResponseBody;
    }

    private static (string authName, string authData) GetAuthInfo(ReadOnlySpan<char> host,
        ReadOnlySpan<char> display)
    {
        var filePath = Environment.GetEnvironmentVariable("XAUTHORITY");
        if (string.IsNullOrWhiteSpace(filePath))
        {
            filePath = Environment.GetEnvironmentVariable("HOME");
            if (string.IsNullOrWhiteSpace(filePath))
                throw new InvalidOperationException("XAUTHORITY not set and HOME not set");
            filePath = Path.Join(filePath, ".XAuthority");
        }

        if (!File.Exists(filePath))
            return ("", "");

        using var fileStream = File.OpenRead(filePath);
        while (fileStream.Position <= fileStream.Length)
        {
            var context = new XAuthority(fileStream);
            var dspy = context.GetDisplayNumber(fileStream);
            var contentName = context.GetName(fileStream);
            if (context.Family == ushort.MaxValue
                       || (context.Family == byte.MaxValue && context.GetHostAddress(fileStream) == host)
                       && (dspy == "" || dspy == display)
                       && contentName.SequenceEqual(MAGICCOOKIE))
                return (contentName.ToString(), context.GetData(fileStream).ToString());
        }

        throw new InvalidOperationException("Invalid XAuthority file present.");
    }

    private static HandshakeResponseHead MakeHandshake(Socket socket, ReadOnlySpan<char> authName, ReadOnlySpan<char> authData)
    {
        var namePaddedLength = authName.Length.AddPadding();
        var scratchBufferSize = 12 + namePaddedLength + authData.Length.AddPadding();
        var writingIndex = 0;

        if (scratchBufferSize < GlobalSetting.StackAllocThreshold)
        {
            Span<byte> scratchBuffer = stackalloc byte[scratchBufferSize];

            scratchBuffer[writingIndex++] = (byte)(BitConverter.IsLittleEndian ? 'l' : 'B');
            scratchBuffer[writingIndex++] = 0;
            MemoryMarshal.Write<ushort>(scratchBuffer[writingIndex..], 11);
            writingIndex += 2;
            MemoryMarshal.Write<ushort>(scratchBuffer[writingIndex..], 0);
            writingIndex += 2;
            MemoryMarshal.Write(scratchBuffer[writingIndex..], (ushort)authName.Length);
            writingIndex += 2;
            MemoryMarshal.Write(scratchBuffer[writingIndex..], (ushort)authData.Length);
            writingIndex += 2;
            MemoryMarshal.Write<ushort>(scratchBuffer[writingIndex..], 0);
            writingIndex += 2;
            Encoding.ASCII.GetBytes(authName, scratchBuffer[writingIndex..]);
            writingIndex += namePaddedLength;
            Encoding.ASCII.GetBytes(authData, scratchBuffer[writingIndex..]);
            socket.SendExact(scratchBuffer);

            scratchBuffer = scratchBuffer.Slice(0, Marshal.SizeOf<HandshakeResponseHead>());
            socket.ReceiveExact(scratchBuffer);
            return scratchBuffer.AsStruct<HandshakeResponseHead>();
        }
        else
        {
            using var scratchBuffer = new ArrayPoolUsing<byte>(scratchBufferSize);
            scratchBuffer[writingIndex++] = (byte)(BitConverter.IsLittleEndian ? 'l' : 'B');
            scratchBuffer[writingIndex++] = 0;
            MemoryMarshal.Write<ushort>(scratchBuffer[writingIndex..], 11);
            writingIndex += 2;
            MemoryMarshal.Write<ushort>(scratchBuffer[writingIndex..], 0);
            writingIndex += 2;
            MemoryMarshal.Write(scratchBuffer[writingIndex..], (ushort)authName.Length);
            writingIndex += 2;
            MemoryMarshal.Write(scratchBuffer[writingIndex..], (ushort)authData.Length);
            writingIndex += 2;
            MemoryMarshal.Write<ushort>(scratchBuffer[writingIndex..], 0);
            writingIndex += 2;
            Encoding.ASCII.GetBytes(authName, scratchBuffer[writingIndex..]);
            writingIndex += namePaddedLength;
            Encoding.ASCII.GetBytes(authData, scratchBuffer[writingIndex..]);
            socket.SendExact(scratchBuffer);

            Span<byte> tempBuffer = stackalloc byte[ Marshal.SizeOf<HandshakeResponseHead>()];
            socket.ReceiveExact(tempBuffer);
            return tempBuffer.AsStruct<HandshakeResponseHead>();
        }
    }
}