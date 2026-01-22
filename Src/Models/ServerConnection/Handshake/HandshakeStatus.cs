namespace Xcsb.Models.ServerConnection.Handshake;

internal enum HandshakeStatus : byte
{
    Failed,
    Success,
    Authenticate
}