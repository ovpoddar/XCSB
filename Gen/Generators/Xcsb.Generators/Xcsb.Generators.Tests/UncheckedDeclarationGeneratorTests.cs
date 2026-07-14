using Xunit;
using Xcsb.Generators.SourceGenerator;
using Xcsb.Generators.Tests.Utils;

namespace Xcsb.Generators.Tests;

public class UncheckedDeclarationGeneratorTests
{
    private const string AttributeSource = @"
using System;
namespace Xcsb.Generators
{
    [AttributeUsage(AttributeTargets.Interface)]
    public class UncheckedDeclarationAttribute : Attribute {}
}";

    [Fact]
    public void Generator_ShouldGenerateClass_WhenInterfaceHasAttribute()
    {
        var source = @"
using Xcsb.Generators;
namespace TestNamespace
{
    [UncheckedDeclaration]
    public partial interface ITestService
    {
    }
}";

        var generatedSource = TestHelper.GenerateSource<UncheckedDeclarationGenerator>(source, AttributeSource, "ITestServiceUnchecked.g.cs");
        
        Assert.Contains("public interface ITestServiceUnchecked", generatedSource);
        Assert.Contains("namespace TestNamespace", generatedSource);
    }

    [Fact]
    public void Generator_ShouldGenerateClass_ContainsMethods_WhenInterfaceHasAttribute()
    {
        var source = @"
using Xcsb.Generators;
namespace TestNamespace
{
    [UncheckedDeclaration]
    public partial interface ITestService
    {
        int DoStaff();
        byte Check { get; set; }
        byte Check12 { get; }
    }
}";

        var generatedSource = TestHelper.GenerateSource<UncheckedDeclarationGenerator>(source, AttributeSource, "ITestServiceUnchecked.g.cs");
        
        Assert.Contains("void DoStaffUnchecked();", generatedSource);
    }

    [Fact]
    public void Generator_ShouldGenerateClass_ContainsConstrainMethods_WhenInterfaceHasAttribute()
    {
        var source = @"
using Xcsb.Generators;
namespace TestNamespace
{
    [UncheckedDeclaration]
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

        var generatedSource = TestHelper.GenerateSource<UncheckedDeclarationGenerator>(source, AttributeSource, "ITestServiceUnchecked.g.cs");
        
        Assert.Contains("void DoSomethingUnchecked<T>(int a, int b, T c);", generatedSource);
        Assert.Contains("void DoSomething1Unchecked<T>(int a, int b, ReadonlySpan<T> c);", generatedSource);
        Assert.Contains("void DoSomething2Unchecked<T>(int a, int b, ReadonlySpan<T> c) where T : struct;", generatedSource);
        Assert.Contains("void DoSomething3Unchecked<T>(int a, int b, ReadonlySpan<T> c) where T : struct\n#if !NETSTANDARD\n    , unmanaged\n#endif\n;", generatedSource);
    }
}
