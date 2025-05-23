using System.Buffers.Binary;
using System.Runtime.CompilerServices;
using System.Text;

namespace Src.Models;

internal readonly ref struct XAuthority
{
    public ushort Family { get; }
    public Segment<int> HostAddress { get; }
    public Segment<int> DisplayNumber { get; }
    public Segment<int> Name { get; }
    public Segment<int> Data { get; }
    public XAuthority(Stream stream)
    {
        Span<byte> scratchBuffer = stackalloc byte[2];
        stream.ReadExactly(scratchBuffer);
        Family = Unsafe.As<byte, ushort>(ref scratchBuffer[0]);

        stream.ReadExactly(scratchBuffer);
        var scratchLength = BinaryPrimitives.ReadUInt16BigEndian(scratchBuffer);
        HostAddress = new Segment<int>((int)stream.Position, scratchLength);
        stream.Seek(scratchLength, SeekOrigin.Current);

        stream.ReadExactly(scratchBuffer);
        scratchLength = BinaryPrimitives.ReadUInt16BigEndian(scratchBuffer);
        DisplayNumber = new Segment<int>((int)stream.Position, scratchLength);
        stream.Seek(scratchLength, SeekOrigin.Current);

        stream.ReadExactly(scratchBuffer);
        scratchLength = BinaryPrimitives.ReadUInt16BigEndian(scratchBuffer);
        Name = new Segment<int>((int)stream.Position, scratchLength);
        stream.Seek(scratchLength, SeekOrigin.Current);

        stream.ReadExactly(scratchBuffer);
        scratchLength = BinaryPrimitives.ReadUInt16BigEndian(scratchBuffer);
        Data = new Segment<int>((int)stream.Position, scratchLength);
        stream.Seek(scratchLength, SeekOrigin.Current);
    }

    public ReadOnlySpan<char> GetHostAddress(Stream stream)
    {
        var oldPosition = stream.Position;
        Span<byte> result = stackalloc byte[HostAddress.Length];
        stream.Seek(HostAddress.Position, SeekOrigin.Begin);
        stream.ReadExactly(result);
        stream.Seek(oldPosition, SeekOrigin.Begin);
        return Encoding.ASCII.GetString(result);
    }

    public ReadOnlySpan<char> GetDisplayNumber(Stream stream)
    {
        var oldPosition = stream.Position;
        Span<byte> result = stackalloc byte[DisplayNumber.Length];
        stream.Seek(DisplayNumber.Position, SeekOrigin.Begin);
        stream.ReadExactly(result);
        stream.Seek(oldPosition, SeekOrigin.Begin);
        return Encoding.ASCII.GetString(result);
    }

    public byte[] GetName(Stream stream)
    {
        var oldPosition = stream.Position;
        var result = new byte[Name.Length];
        stream.Seek(Name.Position, SeekOrigin.Begin);
        stream.ReadExactly(result);
        stream.Seek(oldPosition, SeekOrigin.Begin);
        return result;
    }

    public byte[] GetData(Stream stream)
    {
        var oldPosition = stream.Position;
        var result = new byte[Data.Length];
        stream.Seek(Data.Position, SeekOrigin.Begin);
        stream.ReadExactly(result);
        stream.Seek(oldPosition, SeekOrigin.Begin);
        return result;
    }
}
