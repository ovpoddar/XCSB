using System.Runtime.InteropServices;
using Xcsb.Extension.XInput.Response.Event;

namespace Xcsb.Extension.XInput.Models;

[StructLayout( LayoutKind.Sequential, Pack = 1, Size = 12)]
public struct HierarchyInfo
{
    public readonly InputDevice DeviceId;
    public readonly InputDevice Attachment;
    public readonly DeviceType Type;
    private readonly byte Enabled;
    private readonly ushort _pad0;
    public readonly HierarchyMask Flags;
}