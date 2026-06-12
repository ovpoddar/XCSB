using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Connection.Helpers;
using Xcsb.Extension.XInput.Infrastructure.VoidProto;

namespace Xcsb.Extension.XInput.Models.Writers;

public class HierarchyChangeBuilder
{
    internal int m_length;
    internal byte[] m_data;

    public static HierarchyChangeBuilder Create()
    {
        var items = new HierarchyChangeBuilder()
        {
            m_data = Array.Empty<byte>(),
            m_length = 0
        };
        return items;
    }

    public HierarchyChangeBuilder AddMaster(ReadOnlySpan<byte> name, bool sendCore, bool enable)
    {
        var request = new AddMasterHierarchy((ushort)name.Length, sendCore, enable);
        var position = m_data.Length;
        Insert(request, position);
        name.CopyTo(m_data.AsSpan(position + 8));
        return this;
    }

    public HierarchyChangeBuilder AttachSlave(ushort deviceId, ushort master)
    {
        var request = new AttachSlaveHierarchy(deviceId, master);
        Insert(request);
        return this;
    }

    public HierarchyChangeBuilder RemoveMaster(ushort deviceId, ChangeMode returnMode, ushort returnPointer,
        ushort returnKeyboard)
    {
        var request = new RemoveMasterHierarchy(deviceId, returnMode, returnPointer, returnKeyboard);
        Insert(request);
        return this;
    }

    public HierarchyChangeBuilder DetachSlave(ushort deviceId)
    {
        var request = new DetachSlaveHierarchy(deviceId);
        Insert(request);
        return this;
    }
    
    private void Insert<T>(T item, int? position = null) where T : unmanaged, IHierarchyChange
    {
        position ??= m_data.Length;
        var size = item.Length * 4;
        var newBuffer = new byte[position.Value + size];
        m_data.CopyTo(newBuffer, 0);
        MemoryMarshal.AsBytes(MemoryMarshal.CreateReadOnlySpan(ref item, 1))
            .CopyTo(newBuffer.AsSpan(position.Value));
        m_data = newBuffer;
        m_length++;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 8)]
    [method: MethodImpl(MethodImplOptions.AggressiveInlining)]
    private readonly struct AddMasterHierarchy(ushort nameLength, bool sendCore, bool isEnable) : IHierarchyChange
    {
        public HierarchyChange Type { get; } = HierarchyChange.AddMaster;
        public ushort Length { get; } = (ushort)(2 + (nameLength.AddPadding() / 4));
        public readonly ushort NameLength = nameLength;
        public readonly byte SendCore = (byte)(sendCore ? 1 : 0);
        public readonly byte Enable = (byte)(isEnable ? 1 : 0);
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 8)]
    [method: MethodImpl(MethodImplOptions.AggressiveInlining)]
    private readonly struct AttachSlaveHierarchy(ushort deviceId, ushort master) : IHierarchyChange
    {
        public HierarchyChange Type { get; } = HierarchyChange.AttachSlave;
        public ushort Length { get; } = 2;
        public readonly ushort DeviceId = deviceId;
        public readonly ushort Master = master;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 12)]
    [method: MethodImpl(MethodImplOptions.AggressiveInlining)]
    private readonly struct RemoveMasterHierarchy(ushort deviceId, ChangeMode returnMode, ushort returnPointer,
        ushort returnKeyboard) : IHierarchyChange
    {
        public HierarchyChange Type { get; } = HierarchyChange.RemoveMaster;
        public ushort Length { get; } = 3;
        public readonly ushort DeviceId = deviceId;
        public readonly ChangeMode ReturnMode = returnMode;
        public readonly byte pad0 = 0;
        public readonly ushort ReturnPointer = returnPointer;
        public readonly ushort ReturnKeyboard = returnKeyboard;
    }
    
    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 8)]
    [method:MethodImpl(MethodImplOptions.AggressiveInlining)]
    private readonly struct DetachSlaveHierarchy(ushort deviceId) : IHierarchyChange
    {
        public HierarchyChange Type { get; } = HierarchyChange.DetachSlave;
        public ushort Length { get; } = 2;
        public readonly ushort DeviceId = deviceId;
    }
}