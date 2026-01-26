namespace Xcsb.Extension.Generic.Event.Models;

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