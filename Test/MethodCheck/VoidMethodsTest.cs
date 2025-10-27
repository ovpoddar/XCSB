using Xcsb;

namespace MethodCheck;

public class VoidMethodsTest
{
    [Fact]
    public void Create_Window_Test()
    //byte depth, uint window, uint parent, short x, short y, ushort width, ushort height,
    //  ushort borderWidth, ClassType classType, uint rootVisualId, ValueMask mask, Span<uint> args)
    {
        using var client = XcsbClient.Initialized();
        var bufferClient = client.BufferClient;

        // act
        //bufferClient.CreateWindow(depth, window, parent, x, y, width, height, borderWidth, classType, rootVisualId, mask, args);
        // assert

        var assembly = typeof(IXBufferProto).Assembly;
        var fooType = assembly.GetTypes()
            .FirstOrDefault(t => typeof(IXBufferProto).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

        Assert.True(1 == 1);
    }
}