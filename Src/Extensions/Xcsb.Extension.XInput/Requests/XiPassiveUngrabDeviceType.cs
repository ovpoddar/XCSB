using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Extension.XInput.Models;

namespace Xcsb.Extension.XInput.Requests;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 20)]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
internal readonly struct XiPassiveUngrabDeviceType(
    byte majorOpCode,
    uint grabWindow,
    uint detail,
    ushort deviceId,
    ushort numModifiers,
    byte grabType)
{
    public readonly byte MajorOpcode = majorOpCode;
    public readonly OpCode Opcode = OpCode.XiPassiveUngrabDevice;
    public readonly ushort Length = 5;
    public readonly uint GrabWindow = grabWindow;
    public readonly uint Detail = detail;
    public readonly ushort DeviceId = deviceId;
    public readonly ushort NumModifiers = numModifiers;
    public readonly byte GrabType = grabType;
}