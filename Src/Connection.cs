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

namespace Src;

internal static class Connection
{
    internal static void TryConnect(Socket socket, ReadOnlySpan<char> host, ReadOnlySpan<char> display)
    {
        var (authName, authData) = GetAuthInfo(host, display);
        var namePaddedLength = GenericHelper.AddPadding(authName.Length);
        var scratchBufferSize = 12
                                + namePaddedLength
                                + GenericHelper.AddPadding(authData.Length);
        var writingIndex = 0;
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
        socket.SendMust(scratchBuffer);
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
        while (fileStream.Length <= fileStream.Position)
        {
            var context = new XAuthority(fileStream);
            var dspy = context.GetDisplayNumber(fileStream);
            var contentName = context.GetName(fileStream);
            if (context.Family == ushort.MaxValue
                       || (context.Family == byte.MaxValue && context.GetHostAddress(fileStream) == host)
                       && (dspy == "" || dspy == display)
                       && contentName == "MIT-MAGIC-COOKIE-1")
                return (contentName.ToString(), context.GetData(fileStream).ToString());
        }

        throw new InvalidOperationException("Invalid XAuthority file present.");
    }
}