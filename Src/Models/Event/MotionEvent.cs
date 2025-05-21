namespace Src.Models.Event;

public struct MotionEvent
{
    public EventType EventType;
    public ushort Sequence;
    public byte Detail;
    public Timestamp Time;
    public Window Root;
    public Window Event;
    public Window Child;
    public int16 RootX;
    public int16 RootY;
    public int16 EventX;
    public int16 EventY;
    public uint16 State;
    public sbyte SameScreen; // 1 true 0 false
}