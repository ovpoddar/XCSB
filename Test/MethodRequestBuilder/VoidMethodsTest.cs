using Xcsb;
using Xcsb.Masks;
using Xcsb.Models;

namespace MethodRequestBuilder.Test;

public class VoidMethodsTest
{
    [Theory]
    [InlineData(0, 100, 100, 400, 300, 2, ClassType.InputOutput)]
    public void Create_Window_Test(byte depth, short x, short y, ushort width, ushort height, ushort borderWidth,
        ClassType classType)
    {
        // arrange
        using var client = XcsbClient.Initialized();
        var bufferClient = (XBufferProto)client.BufferClient;
        
        // act
        bufferClient.CreateWindow(depth, client.NewId(), client.HandshakeSuccessResponseBody.Screens[0].Root, x, y,
            width, height, borderWidth, classType, client.HandshakeSuccessResponseBody.Screens[0].RootVisualId,
            ValueMask.BackgroundPixel | ValueMask.EventMask,
            [
                client.HandshakeSuccessResponseBody.Screens[0].WhitePixel,
                (uint)(EventMask.ExposureMask | EventMask.KeyPressMask)
            ]);
        // assert

        Assert.True(true);
    }
}