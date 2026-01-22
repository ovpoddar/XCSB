using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using Xcsb.Configuration;
using Xcsb.Handlers.Direct;
using Xcsb.Helpers;
using Xcsb.Models.Handshake;
using Xcsb.Requests;

namespace Xcsb.Models.Infrastructure;

internal class XConnection : IXConnection, IDisposable
{
    public Socket Socket { get; }
    public ProtoOut ProtoOut { get; }
    public ProtoIn ProtoIn { get; }

    public HandshakeSuccessResponseBody? SuccessResponse { get; private set; }
    public HandshakeStatus HandshakeStatus { get; private set; }

    private bool _disposed;

    public XConnection(string path, XcbClientConfiguration configuration, in ProtocolType type)
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
            return true;
        }
        catch (Exception)
        {
            // Socket disposal is handled by XConnection.Dispose()
            // The caller (ConnectionHelper.cs) will dispose the context on failure
            return false;
        }
    }

    public void SequenceReset()
    {
        ProtoOut.Sequence = 0;
        ProtoIn.Sequence = 0;
    }

    public void SetUpStatus(ref ReadOnlySpan<char> error)
    {
        Span<byte> tempBuffer = stackalloc byte[Unsafe.SizeOf<HandshakeResponseHead>()];
        this.ProtoIn.ReceiveExact(tempBuffer);
        ref readonly var response = ref tempBuffer.AsStruct<HandshakeResponseHead>();

        HandshakeStatus = response.HandshakeStatus;

        if (response.HandshakeStatus is HandshakeStatus.Success)
        {
            SuccessResponse = HandshakeSuccessResponseBody.Read(this.ProtoIn,
                response.HandshakeResponseHeadSuccess.AdditionalDataLength * 4);
            error = [];
        }
        else
        {
            int dataLength = response.HandshakeStatus == HandshakeStatus.Failed
                ? response.HandshakeResponseHeadFailed.AdditionalDataLength
                : response.HandshakeResponseHeadAuthenticate.AdditionalDataLength;
            if (dataLength == 0) throw new NotSupportedException();

            // todo: stack overflow handler
            Span<byte> buffer = stackalloc byte[dataLength * 4];
            this.ProtoIn.ReceiveExact(buffer);
            error = Encoding.ASCII.GetString(buffer).TrimEnd();
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
            // ProtoIn and ProtoOut only hold references to the Socket, they don't own it.
            // Socket is the only resource that needs disposal.
            Socket?.Dispose();
        }

        _disposed = true;
    }
}
