using Xcsb.Connection.Helpers;

namespace MethodRequestBuilder.Test;

public class GenericHelperTest
{
    [Theory]
    [InlineData(0, 0)]
    [InlineData(1, 4)]
    [InlineData(2, 4)]
    [InlineData(3, 4)]
    [InlineData(4, 4)]
    [InlineData(5, 8)]
    [InlineData(7, 8)]
    public void AddPadding_Byte(byte input, byte expected)
    {
        var result = input.AddPadding();
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData((ushort)0, 0)]
    [InlineData((ushort)1, 4)]
    [InlineData((ushort)2, 4)]
    [InlineData((ushort)3, 4)]
    [InlineData((ushort)4, 4)]
    [InlineData((ushort)123, 124)]
    public void AddPadding_UShort(ushort input, ushort expected)
    {
        var result = input.AddPadding();
        Assert.Equal(expected, result);
    }
    
    [Theory]
    [InlineData((short)0, 0)]
    [InlineData((short)1, 4)]
    [InlineData((short)2, 4)]
    [InlineData((short)3, 4)]
    [InlineData((short)4, 4)]
    [InlineData((short)123,124)]
    [InlineData(short.MinValue, short.MinValue)]
    public void AddPadding_Short(short input, short expected)
    {
        var result = input.AddPadding();
        Assert.Equal(expected, result);
    }
    
    [Theory]
    [InlineData(0, 0)]
    [InlineData(1, 4)]
    [InlineData(2, 4)]
    [InlineData(3, 4)]
    [InlineData(4, 4)]
    [InlineData(100, 100)]
    [InlineData(int.MinValue, int.MinValue)]
    public void AddPadding_Int(int input, int expected)
    {
        var result = input.AddPadding();
        Assert.Equal(expected, result);
    }
    
    [Theory]
    [InlineData((uint)0, 0)]
    [InlineData((uint)1, 4)]
    [InlineData((uint)2, 4)]
    [InlineData((uint)3, 4)]
    [InlineData((uint)4, 4)]
    [InlineData((uint)100, 100)]
    public void AddPadding_UInt(uint input, uint expected)
    {
        var result = input.AddPadding();
        Assert.Equal(expected, result);
    }

    [Fact]
    public void AddPadding_Thorw()
    {
#if NETSTANDARD
        Assert.Throws<ArgumentException>(() => 1.0f.AddPadding());
#endif
    }
    
    [Theory]
    [InlineData(0, 0)]
    [InlineData(1, 3)]
    [InlineData(2, 2)]
    [InlineData(3, 1)]
    [InlineData(4, 0)]
    [InlineData(5, 3)]
    [InlineData(7, 1)]
    public void Padding_Byte(byte input, byte expected)
    {
        var result = input.Padding();
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData((ushort)0, 0)]
    [InlineData((ushort)1, 3)]
    [InlineData((ushort)2, 2)]
    [InlineData((ushort)3, 1)]
    [InlineData((ushort)4, 0)]
    [InlineData((ushort)123, 1)]
    public void Padding_UShort(ushort input, ushort expected)
    {
        var result = input.Padding();
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData((short)0, 0)]
    [InlineData((short)1, 3)]
    [InlineData((short)2, 2)]
    [InlineData((short)3, 1)]
    [InlineData((short)4, 0)]
    [InlineData((short)123, 1)]
    [InlineData(short.MinValue, 0)]
    public void Padding_Short(short input, short expected)
    {
        var result = input.Padding();
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(1, 3)]
    [InlineData(2, 2)]
    [InlineData(3, 1)]
    [InlineData(4, 0)]
    [InlineData(100, 0)]
    [InlineData(int.MinValue, 0)]
    public void Padding_Int(int input, int expected)
    {
        var result = input.Padding();
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData((uint)0, 0)]
    [InlineData((uint)1, 3)]
    [InlineData((uint)2, 2)]
    [InlineData((uint)3, 1)]
    [InlineData((uint)4, 0)]
    [InlineData((uint)100, 0)]
    public void Padding_UInt(uint input, uint expected)
    {
        var result = input.Padding();
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Padding_Thorw()
    {
#if NETSTANDARD
        Assert.Throws<ArgumentException>(() => 1.0f.Padding());
#endif
    }
}