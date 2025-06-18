using System.Buffers.Binary;
using System.Runtime.CompilerServices;
using System.Text;

namespace Xcsb.Models;

internal readonly ref struct XAuthority
{
    public readonly ushort Family;
    private readonly DataRange _hostAddress;
    private readonly DataRange _displayNumber;
    private readonly DataRange _name;
    private readonly DataRange _data;
    public XAuthority(Stream stream)
    {
        Span<byte> scratchBuffer = stackalloc byte[2];
        stream.ReadExactly(scratchBuffer);
        Family = Unsafe.As<byte, ushort>(ref scratchBuffer[0]);

        stream.ReadExactly(scratchBuffer);
        var scratchLength = BinaryPrimitives.ReadUInt16BigEndian(scratchBuffer);
        _hostAddress = new DataRange((int)stream.Position, scratchLength);
        stream.Seek(scratchLength, SeekOrigin.Current);

        stream.ReadExactly(scratchBuffer);
        scratchLength = BinaryPrimitives.ReadUInt16BigEndian(scratchBuffer);
        _displayNumber = new DataRange((int)stream.Position, scratchLength);
        stream.Seek(scratchLength, SeekOrigin.Current);

        stream.ReadExactly(scratchBuffer);
        scratchLength = BinaryPrimitives.ReadUInt16BigEndian(scratchBuffer);
        _name = new DataRange((int)stream.Position, scratchLength);
        stream.Seek(scratchLength, SeekOrigin.Current);

        stream.ReadExactly(scratchBuffer);
        scratchLength = BinaryPrimitives.ReadUInt16BigEndian(scratchBuffer);
        _data = new DataRange((int)stream.Position, scratchLength);
        stream.Seek(scratchLength, SeekOrigin.Current);
    }

    public ReadOnlySpan<char> GetHostAddress(Stream stream)
    {
        var oldPosition = stream.Position;
        Span<byte> result = stackalloc byte[_hostAddress.Length];
        stream.Seek(_hostAddress.Position, SeekOrigin.Begin);
        stream.ReadExactly(result);
        stream.Seek(oldPosition, SeekOrigin.Begin);
        return Encoding.ASCII.GetString(result);
    }

    public ReadOnlySpan<char> GetDisplayNumber(Stream stream)
    {
        var oldPosition = stream.Position;
        Span<byte> result = stackalloc byte[_displayNumber.Length];
        stream.Seek(_displayNumber.Position, SeekOrigin.Begin);
        stream.ReadExactly(result);
        stream.Seek(oldPosition, SeekOrigin.Begin);
        return Encoding.ASCII.GetString(result);
    }

    public byte[] GetName(Stream stream)
    {
        var oldPosition = stream.Position;
        var result = new byte[_name.Length];
        stream.Seek(_name.Position, SeekOrigin.Begin);
        stream.ReadExactly(result);
        stream.Seek(oldPosition, SeekOrigin.Begin);
        return result;
    }

    public byte[] GetData(Stream stream)
    {
        var oldPosition = stream.Position;
        var result = new byte[_data.Length];
        stream.Seek(_data.Position, SeekOrigin.Begin);
        stream.ReadExactly(result);
        stream.Seek(oldPosition, SeekOrigin.Begin);
        return result;
    }

    private readonly struct DataRange
    {
        public readonly int Position;
        public readonly int Length;

        public DataRange(int position, int length)
        {
            Position = position;
            Length = length;
        }
    }
}
