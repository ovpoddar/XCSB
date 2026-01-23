namespace Xcsb.Models.ServerConnection.Handshake;

public enum BackingStores : byte
{
    Never = 0,
    WhenMapped = 1,
    Always = 2
}