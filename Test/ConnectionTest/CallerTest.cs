using ConnectionTest.TestFunctionBuilder;
using System.Diagnostics;

namespace ConnectionTest;

public class CallerTest : IClassFixture<SetupTestEnviroment>
{
    private readonly SetupTestEnviroment _setupTestEnviroment;

    public CallerTest(SetupTestEnviroment setupTestEnviroment)
    {
        _setupTestEnviroment = setupTestEnviroment;
    }
    
    [Theory]
    [InlineData("GrabServer", true)]
    [InlineData("UngrabServer", true)]
    [InlineData("GetInputFocus", false)]
    [InlineData("QueryKeymap", false)]
    [InlineData("GetFontPath", false)]
    [InlineData("ListExtensions", false)]
    [InlineData("GetModifierMapping", false)]
    [InlineData("GetKeyboardControl", false)]
    [InlineData("GetPointerMapping", false)]
    [InlineData("GetPointerControl", false)]
    [InlineData("GetScreenSaver", false)]
    [InlineData("ListHosts", false)]
    public void Test1(string methodName, bool isVoidReturn, params int[] args)
    {
        if (!OperatingSystem.IsLinux())
            return;

        // arrange
        using var cProcessBuilder = new CFunctionBuilder(_setupTestEnviroment);
        using var csProcessBuilder = new CSFunctionBuilder(_setupTestEnviroment);
        // act
        var csResponse = csProcessBuilder.GetFunctionContent(methodName, isVoidReturn, args);
        var cResponse = cProcessBuilder.GetFunctionContent(methodName, isVoidReturn, args);
        // assert

        Assert.NotNull(methodName);
        Assert.NotEqual(0, csResponse.Length);
        Assert.NotEqual(0, cResponse.Length);
        Assert.True(csResponse.SequenceEqual(cResponse));
    }
}