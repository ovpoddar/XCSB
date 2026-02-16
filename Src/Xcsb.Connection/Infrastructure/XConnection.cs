using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using Xcsb.Connection.Configuration;
using Xcsb.Connection.Handlers;
using Xcsb.Connection.Helpers;
using Xcsb.Connection.Models.Handshake;

namespace Xcsb.Connection.Infrastructure;

internal class XConnection : IXConnectionInternal
{
    private readonly Socket _socket;
    private readonly ConcurrentQueue<byte[]> _bufferEvents;
    private readonly ConcurrentDictionary<int, byte[]> _replyBuffer;
    private bool _disposed;

    public int GlobalId;
    public HandshakeSuccessResponseBody? HandshakeSuccessResponseBody { get; private set; }
    public HandshakeStatus HandshakeStatus { get; private set; }
    public string FailReason { get; private set; } = string.Empty;
    public bool Connected => this._socket.Connected;
    public SoccketAccesser Accesser { get; }
    public IXExtensation Extensation { get; }

    public XConnection(string path, XcsbClientConfiguration configuration, in ProtocolType type)
    {
        this._socket = new Socket(AddressFamily.Unix, SocketType.Stream, type);
        this._socket.Connect(new UnixDomainSocketEndPoint(path));
        this._replyBuffer = new ConcurrentDictionary<int, byte[]>();
        this._bufferEvents = new ConcurrentQueue<byte[]>();
        this.Accesser = new SoccketAccesser(_socket, _bufferEvents, _replyBuffer, configuration);
        this.Extensation = new XcsbExtensation(this.Accesser);
        this.GlobalId = 0;
    }

    public bool EstablishConnection(ReadOnlySpan<byte> authName, ReadOnlySpan<byte> authData)
    {
        try
        {
            var request = new HandShakeRequestType((ushort)authName.Length, (ushort)authData.Length);
            var length = authName.Length.AddPadding() + authData.Length.AddPadding() + Marshal.SizeOf<HandShakeRequestType>();
            var writeIndex = 12;
            if (length < XcsbClientConfiguration.StackAllocThreshold)
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
                this.Accesser.SendData(scratchBuffer, SocketFlags.None);
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
                this.Accesser.SendData(workingBuffer, SocketFlags.None);
            }
            return true;
        }
        catch (Exception)
        {
            // Socket disposal is handled by XConnection.Dispose()
            // The caller (ConnectionHelper.cs) will dispose the context on failure
            return false;
        }
    }

    public void SetUpStatus()
    {
        Span<byte> tempBuffer = stackalloc byte[Unsafe.SizeOf<HandshakeResponseHead>()];
        this.Accesser.Received(tempBuffer);
        ref readonly var response = ref tempBuffer.AsStruct<HandshakeResponseHead>();

        HandshakeStatus = response.HandshakeStatus;

        if (response.HandshakeStatus is HandshakeStatus.Success)
        {
            HandshakeSuccessResponseBody = HandshakeSuccessResponseBody.Read(this.Accesser,
                response.HandshakeResponseHeadSuccess.AdditionalDataLength * 4);
            FailReason = string.Empty;
        }
        else
        {
            int dataLength = response.HandshakeStatus == HandshakeStatus.Failed
                ? response.HandshakeResponseHeadFailed.AdditionalDataLength
                : response.HandshakeResponseHeadAuthenticate.AdditionalDataLength;
            if (dataLength == 0) throw new NotSupportedException();

            var requiredBuffer = dataLength * 4;
            if (requiredBuffer < 255)
            {
                Span<byte> buffer = stackalloc byte[requiredBuffer];
                this.Accesser.Received(buffer);
                FailReason = Encoding.ASCII.GetString(buffer).TrimEnd();
            }
            else
            {
                using var buffer = new ArrayPoolUsing<byte>(dataLength);
                var workingBuffer = buffer.Slice(0, dataLength);
                this.Accesser.Received(workingBuffer);
                FailReason = Encoding.ASCII.GetString(workingBuffer).TrimEnd();
            }
        }
    }


    public uint NewId() => HandshakeSuccessResponseBody is null
        ? throw new InvalidOperationException()
        : (uint)((HandshakeSuccessResponseBody.ResourceIDMask & this.GlobalId++) | HandshakeSuccessResponseBody.ResourceIDBase);

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
            // ProtoIn and ProtoOut only hold references to the Socket, they don't own it.
            // Socket is the only resource that needs disposal.
            Accesser.BufferEvents.Clear();
            Accesser.ReplyBuffer.Clear();
            if (Extensation is IXExtensationInternal extensationInternal)
                extensationInternal.Clear();
            _socket?.Dispose();
        }

        _disposed = true;
    }
}
