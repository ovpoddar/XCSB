using System.Net.Sockets;
using System.Runtime.InteropServices;
using Xcsb.Configuration;
using Xcsb.Handlers.Direct;
using Xcsb.Helpers;
using Xcsb.Requests;

namespace Xcsb.Models.Infrastructure;

internal class ClientConnectionContext : IDisposable
{
    public Socket Socket { get; }
    public ProtoOut ProtoOut { get; }
    public ProtoIn ProtoIn { get; }

    private bool _disposed;

    public ClientConnectionContext(string path, XcbClientConfiguration configuration, in ProtocolType type)
    {
        this.Socket = new Socket(AddressFamily.Unix, SocketType.Stream, type);
        Socket.Connect(new UnixDomainSocketEndPoint(path));
        ProtoOut = new ProtoOut(Socket, configuration);
        ProtoIn = new ProtoIn(Socket, configuration);
    }

    public bool Connected => this.Socket.Connected;

    public bool EstablishConnection(ReadOnlySpan<byte> authName, ReadOnlySpan<byte> authData)
    {
        try
        {
            var request = new HandShakeRequestType((ushort)authName.Length, (ushort)authData.Length);
            var length = authName.Length.AddPadding() + authData.Length.AddPadding() + Marshal.SizeOf<HandShakeRequestType>();
            var writeIndex = 12;
            if (length < XcbClientConfiguration.StackAllocThreshold)
            {
                Span<byte> scratchBuffer = stackalloc byte[length];
#if NETSTANDARD
                MemoryMarshal.Write(scratchBuffer[0..writeIndex], ref request);
#else
                MemoryMarshal.Write(scratchBuffer[0..writeIndex], in request);
#endif
                authName.CopyTo(scratchBuffer[writeIndex..]);
                writeIndex += authName.Length;
                scratchBuffer.Slice(writeIndex, authName.Length.Padding()).Clear();
                writeIndex += authName.Length.Padding();

                authData.CopyTo(scratchBuffer[writeIndex..]);
                writeIndex += authData.Length;
                scratchBuffer.Slice(writeIndex, authData.Length.Padding()).Clear();
                ProtoOut.SendExact(scratchBuffer);
            }
            else
            {
                using var scratchBuffer = new ArrayPoolUsing<byte>(length);
                var workingBuffer = scratchBuffer[..length];
#if NETSTANDARD
                MemoryMarshal.Write(workingBuffer[0..writeIndex], ref request);
#else
                MemoryMarshal.Write(workingBuffer[0..writeIndex], in request);
#endif
                authName.CopyTo(workingBuffer[writeIndex..]);
                writeIndex += authName.Length;
                workingBuffer.Slice(writeIndex, authName.Length.Padding()).Clear();
                writeIndex += authName.Length.Padding();

                authData.CopyTo(workingBuffer[writeIndex..]);
                writeIndex += authData.Length;
                workingBuffer.Slice(writeIndex, authName.Length.Padding()).Clear();
                ProtoOut.SendExact(workingBuffer);
            }

                ProtoOut.Sequence = 0;
            ProtoIn.Sequence = 0;
            return true;
        }
        catch (Exception)
        {
            Socket.Dispose();
            return false;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (_disposed)
            return;

        if (disposing)
        {
            ProtoIn?.Dispose();
            ProtoOut?.Dispose();
            Socket?.Dispose();
        }

        _disposed = true;
    }
}
