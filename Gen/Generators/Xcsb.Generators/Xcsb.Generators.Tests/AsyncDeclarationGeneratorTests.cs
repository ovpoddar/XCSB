using Xunit;
using Xcsb.Generators.SourceGenerator;
using Xcsb.Generators.Tests.Utils;

namespace Xcsb.Generators.Tests;

public class AsyncDeclarationGeneratorTests
{
    private const string AttributeSource = @"
using System;
namespace Xcsb.Generators
{
    [AttributeUsage(AttributeTargets.Interface)]
    public class AsyncDeclarationAttribute : Attribute {}
}";

    [Fact]
    public void Generator_ShouldGenerateClass_WhenInterfaceHasAttribute()
    {
        var source = @"
using Xcsb.Generators;
namespace TestNamespace
{
    [AsyncDeclaration]
    public partial interface ITestService
    {
    }
}";

        var generatedSource = TestHelper.GenerateSource<AsyncDeclarationGenerator>(source, AttributeSource, "ITestServiceAsync.g.cs");
        
        Assert.Contains("public interface ITestServiceAsync", generatedSource);
        Assert.Contains("namespace TestNamespace", generatedSource);
    }

    [Fact]
    public void Generator_ShouldGenerateClass_ContainsMethods_WhenInterfaceHasAttribute()
    {
        var source = @"
using Xcsb.Generators;
namespace TestNamespace
{
    [AsyncDeclaration]
    public partial interface ITestService
    {
        int DoStaff();
        byte Check { get; set; }
        byte Check12 { get; }
    }
}";

        var generatedSource = TestHelper.GenerateSource<AsyncDeclarationGenerator>(source, AttributeSource, "ITestServiceAsync.g.cs");
        
        Assert.Contains("System.Threading.Tasks.Task<int> DoStaffAsync();", generatedSource);
    }

    [Fact]
    public void Generator_ShouldGenerateClass_ContainsConstrainMethods_WhenInterfaceHasAttribute()
    {
        var source = @"
using Xcsb.Generators;
namespace TestNamespace
{
    [AsyncDeclaration]
    public partial interface ITestService
    {
        int DoSomething<T>(int a, int b, T c);
        int DoSomething1<T>(int a, int b, ReadonlySpan<T> c);
        int DoSomething2<T>(int a, int b, ReadonlySpan<T> c) where T : struct;
        int DoSomething3<T>(int a, int b, ReadonlySpan<T> c) where T : struct
#if !NETSTANDARD
    , unmanaged
#endif
;
    }
}";

        var generatedSource = TestHelper.GenerateSource<AsyncDeclarationGenerator>(source, AttributeSource, "ITestServiceAsync.g.cs");
        
        Assert.Contains("System.Threading.Tasks.Task<int> DoSomethingAsync<T>(int a, int b, T c);", generatedSource);
        Assert.Contains("System.Threading.Tasks.Task<int> DoSomething1Async<T>(int a, int b, ReadonlySpan<T> c);", generatedSource);
        Assert.Contains("System.Threading.Tasks.Task<int> DoSomething2Async<T>(int a, int b, ReadonlySpan<T> c) where T : struct;", generatedSource);
        Assert.Contains("System.Threading.Tasks.Task<int> DoSomething3Async<T>(int a, int b, ReadonlySpan<T> c) where T : struct\n#if !NETSTANDARD\n    , unmanaged\n#endif\n;", generatedSource);
    }
}
