namespace Xcsb.Models;

public enum EventsMode : byte
{
    AsyncPointer,
    SyncPointer,
    ReplayPointer,
    AsyncKeyboard,
    SyncKeyboard,
    ReplayKeyboard,
    AsyncBoth,
    SyncBoth
}