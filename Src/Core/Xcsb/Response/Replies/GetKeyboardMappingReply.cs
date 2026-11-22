using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xcsb.Connection.Helpers;
using Xcsb.Connection.Response.Contract;
using Xcsb.Response.Contract;
using Xcsb.Response.Replies.Internals;

namespace Xcsb.Response.Replies;

public readonly struct GetKeyboardMappingReply : IXReply<GetKeyboardMappingReply, GetKeyboardMappingResponse>, IVerify
{
    private readonly byte[] _data;

    private GetKeyboardMappingReply(byte[] data)
    {
        if (data is null) throw new ArgumentNullException(nameof(data));
        if (data.Length < 33)
            throw new ArgumentException("Buffer too small for GetKeyboardMappingReply.", nameof(data));
        _data = data;
    }

    public byte ResponseType => _data[0];
    public ushort Sequence => Unsafe.As<byte, ushort>(ref _data[2]);
    public uint Length => Unsafe.As<byte, uint>(ref _data[4]);
    public int ByteSize => checked(32 + (int)Length * 4);
    public bool Verify(in ushort sequence) => ResponseType == 1 && Sequence == sequence;
    public byte KeysymsPerKeycode => _data[1];
    public ReadOnlySpan<uint> Keysyms => MemoryMarshal.Cast<byte, uint>(new ReadOnlySpan<byte>(_data, 
        Unsafe.SizeOf<GetKeyboardMappingResponse>(), 
        ((int)Length) * 4));

    public GetKeyboardMappingReply FromBytes(byte[] response) => new GetKeyboardMappingReply(response);
}