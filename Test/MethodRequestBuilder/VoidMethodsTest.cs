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
    [InlineData(0, 100, 100, 400, 300, 2, ClassType.InputOutput, new byte[] { 1, 0, 10, 0, 0, 0, 96, 0, 56, 4, 0, 0, 100, 0, 100, 0, 144, 1, 44, 1, 2, 0, 1, 0, 35, 0, 0, 0, 2, 8, 0, 0, 1, 128, 0, 0, 255, 255, 255, 0 })]
    public void Create_Window_Test(byte depth, short x, short y, ushort width, ushort height, ushort borderWidth,
        ClassType classType, byte[] result)
    {
        // arrange
        var workingField = typeof(BufferProtoOut)
            .GetField("_buffer", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var screen = _xProto.HandshakeSuccessResponseBody.Screens[0];
        var bufferClient = (XBufferProto)_xProto.BufferClient;
        uint windowID = 23068672; // 6291456;
        var c = _xProto.NewId();
        Assert.Equal(windowID, c);
        // act
        bufferClient.CreateWindow(depth, windowID, screen.Root, x, y,
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