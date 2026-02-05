using System.Runtime.InteropServices;
using Xcsb.Connection.Response.Contract;
using Xcsb.Models;
using Xcsb.Response.Contract;

namespace Xcsb.Extension.Damage.Response.Events;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 32)]
public class DamageNotifyEvent : IXEvent
{
    public readonly ResponseHeader<byte> ResponseHeader;
    public readonly uint Drawable;
    public readonly uint Damage;
    public readonly uint Timestamp;
    public readonly Rectangle Area;
    public readonly Rectangle Geometry;

    public bool Verify()
    {
        return true;
    }
}
