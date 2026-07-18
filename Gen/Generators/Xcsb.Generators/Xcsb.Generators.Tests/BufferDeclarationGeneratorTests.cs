using Xunit;
using Xcsb.Generators.SourceGenerator;
using Xcsb.Generators.Tests.Utils;

namespace Xcsb.Generators.Tests;

public class BufferDeclarationGeneratorTests
{
    private const string AttributeSource = @"
using System;
namespace Xcsb.Generators
{
    [AttributeUsage(AttributeTargets.Interface)]
    public class BufferDeclarationAttribute : Attribute {}
}";

    [Fact]
    public void Generator_ShouldGenerateClass_WhenInterfaceHasAttribute()
    {
        var source = @"
using Xcsb.Generators;
namespace TestNamespace
{
    [BufferDeclaration]
    public partial interface ITestService
    {
    }
}";

        var generatedSource = TestHelper.GenerateSource<BufferDeclarationGenerator>(source, AttributeSource, "ITestServiceBuffer.g.cs");

        Assert.Contains("public interface ITestServiceBuffer", generatedSource);
        Assert.Contains("namespace TestNamespace", generatedSource);
    }

    [Fact]
    public void Generator_ShouldGenerateClass_ContainsMethods_WhenInterfaceHasAttribute()
    {
        var source = @"
using Xcsb.Generators;
namespace TestNamespace
{
    [BufferDeclaration]
    public partial interface ITestService
    {
        int DoStaff();
        byte Check { get; set; }
        byte Check12 { get; }
    }
}";

        var generatedSource = TestHelper.GenerateSource<BufferDeclarationGenerator>(source, AttributeSource, "ITestServiceBuffer.g.cs");

        Assert.Contains("void DoStaff();", generatedSource);
        Assert.DoesNotContain("DoStaffBuffer", generatedSource);
    }

    [Fact]
    public void Generator_ShouldGenerateClass_ContainsConstrainMethods_WhenInterfaceHasAttribute()
    {
        var source = @"
#define STANDARD

using System;
using Xcsb.Generators;
namespace TestNamespace
{
    [BufferDeclaration]
    public partial interface ITestService
    {
        int DoSomething<T>(int a, int b, T c);
        int DoSomething1<T>(int a, int b, ReadOnlySpan<T> c);
        int DoSomething2<T>(int a, int b, ReadOnlySpan<T> c) where T : struct;
        int DoSomething3<T>(int a, int b, ReadOnlySpan<T> c) where T : struct
#if !STANDARD
    , unmanaged
#endif
;
    }
}";

        var generatedSource = TestHelper.GenerateSource<BufferDeclarationGenerator>(source, AttributeSource, "ITestServiceBuffer.g.cs");

        Assert.Contains("void DoSomething<T>(int a, int b, T c);", generatedSource);
        Assert.Contains("void DoSomething1<T>(int a, int b, global::System.ReadOnlySpan<T> c);", generatedSource);
        Assert.Contains("void DoSomething2<T>(int a, int b, global::System.ReadOnlySpan<T> c) where T : struct;", generatedSource);
        Assert.Contains(
            """
            void DoSomething3<T>(int a, int b, global::System.ReadOnlySpan<T> c) where T : struct
            #if !STANDARD
                , unmanaged
            #endif
            ;
            """
            
            , generatedSource);
        Assert.DoesNotContain("DoSomethingBuffer", generatedSource);
    }
}
