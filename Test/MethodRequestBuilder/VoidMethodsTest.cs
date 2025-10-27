using Xcsb;
using Xcsb.Handlers;
using Xcsb.Masks;
using Xcsb.Models;

namespace MethodRequestBuilder.Test;

public class VoidMethodsTest : IDisposable
{
    private readonly IXProto _xProto;
    public VoidMethodsTest()
    {
        _xProto = XcsbClient.Initialized();
    }

    [Theory]
    [InlineData(0, 100, 100, 400, 300, 2, ClassType.InputOutput, new byte[] { 1, 2, })]
    public void Create_Window_Test(byte depth, short x, short y, ushort width, ushort height, ushort borderWidth,
        ClassType classType, byte[] result)
    {
        // arrange
        var workingField = typeof(BufferProtoOut)
            .GetField("_buffer", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var screen = _xProto.HandshakeSuccessResponseBody.Screens[0];
        var bufferClient = (XBufferProto)_xProto.BufferClient;

        // act
        bufferClient.CreateWindow(depth, _xProto.NewId(), screen.Root, x, y,
            width, height, borderWidth, classType, screen.RootVisualId,
            ValueMask.BackgroundPixel | ValueMask.EventMask,
            [
                screen.WhitePixel,
                (uint)(EventMask.ExposureMask | EventMask.KeyPressMask)
            ]);
        var buffer = (List<byte>?)workingField?.GetValue(bufferClient.BufferProtoOut);

        // assert
        Assert.NotNull(buffer);
        Assert.True(result.SequenceEqual([.. buffer]));
    }

    public void Dispose() => 
        _xProto.Dispose();
}