using Xcsb.Connection.Models;

namespace Xcsb.Extension.XInput.Models.TypeInfo;

public sealed record XiInputEventType : XEventType
{
    private XiInputEventType(byte value, string name) : base(value, name)
    {
    }

    public static readonly XiInputEventType DeviceValuator = new XiInputEventType(0, "DeviceValuator");
    public static readonly XiInputEventType DeviceKeyPress = new XiInputEventType(1, "DeviceKeyPress");
    public static readonly XiInputEventType DeviceKeyRelease = new XiInputEventType(2, "DeviceKeyRelease");
    public static readonly XiInputEventType DeviceButtonPress = new XiInputEventType(3, "DeviceButtonPress");
    public static readonly XiInputEventType DeviceButtonRelease = new XiInputEventType(4, "DeviceButtonRelease");
    public static readonly XiInputEventType DeviceMotionNotify = new XiInputEventType(5, "DeviceMotionNotify");
    public static readonly XiInputEventType DeviceFocusIn = new XiInputEventType(6, "DeviceFocusIn");
    public static readonly XiInputEventType DeviceFocusOut = new XiInputEventType(7, "DeviceFocusOut");
    public static readonly XiInputEventType ProximityIn = new XiInputEventType(8, "ProximityIn");
    public static readonly XiInputEventType ProximityOut = new XiInputEventType(9, "ProximityOut");
    public static readonly XiInputEventType DeviceStateNotify = new XiInputEventType(10, "DeviceStateNotify");
    public static readonly XiInputEventType DeviceMappingNotify = new XiInputEventType(11, "DeviceMappingNotify");
    public static readonly XiInputEventType ChangeDeviceNotify = new XiInputEventType(12, "ChangeDeviceNotify");
    public static readonly XiInputEventType DeviceKeyStateNotify = new XiInputEventType(13, "DeviceKeyStateNotify");
    public static readonly XiInputEventType DeviceButtonStateNotify = new XiInputEventType(14, "DeviceButtonStateNotify");
    public static readonly XiInputEventType DevicePresenceNotify = new XiInputEventType(15, "DevicePresenceNotify");
    public static readonly XiInputEventType DevicePropertyNotify = new XiInputEventType(16, "DevicePropertyNotify");
    
    public static readonly XiInputEventType DeviceChanged = new XiInputEventType(1, "DeviceChanged");
    public static readonly XiInputEventType KeyPress = new XiInputEventType(2, "KeyPress");
    public static readonly XiInputEventType KeyRelease = new XiInputEventType(3, "KeyRelease");
    public static readonly XiInputEventType ButtonPress = new XiInputEventType(4, "ButtonPress");
    public static readonly XiInputEventType ButtonRelease = new XiInputEventType(5, "ButtonRelease");
    public static readonly XiInputEventType Motion = new XiInputEventType(6, "Motion");
    public static readonly XiInputEventType Enter = new XiInputEventType(7, "Enter");
    public static readonly XiInputEventType Leave = new XiInputEventType(8, "Leave");
    public static readonly XiInputEventType FocusIn = new XiInputEventType(9, "FocusIn");
    public static readonly XiInputEventType FocusOut = new XiInputEventType(10, "FocusOut");   
    public static readonly XiInputEventType Hierarchy = new XiInputEventType(11, "Hierarchy");
    public static readonly XiInputEventType Property = new XiInputEventType(12, "Property");
    public static readonly XiInputEventType RawKeyPress = new XiInputEventType(13, "RawKeyPress");
    public static readonly XiInputEventType RawKeyRelease = new XiInputEventType(14, "RawKeyRelease");
    public static readonly XiInputEventType RawButtonPress = new XiInputEventType(15, "RawButtonPress");
    public static readonly XiInputEventType RawButtonRelease = new XiInputEventType(16, "RawButtonRelease");
    public static readonly XiInputEventType RawMotion = new XiInputEventType(17, "RawMotion");
    public static readonly XiInputEventType TouchBegin = new XiInputEventType(18, "TouchBegin");
    public static readonly XiInputEventType TouchUpdate = new XiInputEventType(19, "TouchUpdate");
    public static readonly XiInputEventType TouchEnd = new XiInputEventType(20, "TouchEnd");
    public static readonly XiInputEventType TouchOwnership = new XiInputEventType(21, "TouchOwnership");
    public static readonly XiInputEventType RawTouchBegin = new XiInputEventType(22, "RawTouchBegin");
    public static readonly XiInputEventType RawTouchUpdate = new XiInputEventType(23, "RawTouchUpdate");
    public static readonly XiInputEventType RawTouchEnd = new XiInputEventType(24, "RawTouchEnd");
    public static readonly XiInputEventType BarrierHit = new XiInputEventType(25, "BarrierHit");
    public static readonly XiInputEventType BarrierLeave = new XiInputEventType(26, "BarrierLeave");
}