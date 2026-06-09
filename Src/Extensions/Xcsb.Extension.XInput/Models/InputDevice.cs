using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Xcsb.Extension.XInput.Models;

[DebuggerDisplay("{DebuggerDisplay,nq}")]
[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 2)]
public readonly struct InputDevice
{
    public ushort Id { get; }
    public InputDevice(ushort id) => Id = id;

    private InputDevice(PredefinedInputDevice id) : this((ushort)id) { }
    
    private enum PredefinedInputDevice : ushort
    {
        DeviceAll,
        DeviceAllMaster
    }

    public static readonly InputDevice DeviceAll = new InputDevice(PredefinedInputDevice.DeviceAll);
    public static readonly InputDevice DeviceAllMaster = new InputDevice(PredefinedInputDevice.DeviceAllMaster);
}