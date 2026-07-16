using Xunit;
using Xcsb.Generators.SourceGenerator;
using Xcsb.Generators.Tests.Utils;

namespace Xcsb.Generators.Tests;

public class CheckedDeclarationGeneratorTests
{
    private const string AttributeSource = @"
using System;
namespace Xcsb.Generators
{
    [AttributeUsage(AttributeTargets.Interface)]
    public class CheckedDeclarationAttribute : Attribute {}
}";

    [Fact]
    public void Generator_ShouldGenerateClass_WhenInterfaceHasAttribute()
    {
        var source = @"
using Xcsb.Generators;
namespace TestNamespace
{
    [CheckedDeclaration]
    public partial interface ITestService
    {
    }
}";

        var generatedSource =
            TestHelper.GenerateSource<CheckedDeclarationGenerator>(source, AttributeSource, "ITestServiceChecked.g.cs");

        Assert.Contains("public interface ITestServiceChecked", generatedSource);
        Assert.Contains("namespace TestNamespace", generatedSource);
    }

    [Fact]
    public void Generator_ShouldGenerateClass_ContainsMethods_WhenInterfaceHasAttribute()
    {
        var source = @"
using Xcsb.Generators;
namespace TestNamespace
{
    [CheckedDeclaration]
    public partial interface ITestService
    {
        int DoStaff();
        byte Check { get; set; }
        byte Check12 { get; }
    }
}";

        var generatedSource =
            TestHelper.GenerateSource<CheckedDeclarationGenerator>(source, AttributeSource, "ITestServiceChecked.g.cs");

        Assert.Contains("void DoStaffChecked();", generatedSource);
    }

    [Fact]
    public void Generator_ShouldGenerateClass_ContainsConstrainMethods_WhenInterfaceHasAttribute()
    {
        var source = @"
#define STANDARD
using Xcsb.Generators;
namespace TestNamespace
{
    [CheckedDeclaration]
    public partial interface ITestService
    {
        int DoSomething<T>(int a, int b, T c);
        int DoSomething1<T>(int a, int b, System.ReadOnlySpan<T> c);
        int DoSomething2<T>(int a, int b, System.ReadOnlySpan<T> c) where T : struct;
        int DoSomething3<T>(int a, int b, System.ReadOnlySpan<T> c) where T : struct
#if !STANDARD
    , unmanaged
#endif
;
    }
}";

        var generatedSource =
            TestHelper.GenerateSource<CheckedDeclarationGenerator>(source, AttributeSource, "ITestServiceChecked.g.cs");

        Assert.Contains("void DoSomethingChecked<T>(int a, int b, T c);", generatedSource);
        Assert.Contains("void DoSomething1Checked<T>(int a, int b, global::System.ReadOnlySpan<T> c);", generatedSource);
        Assert.Contains("void DoSomething2Checked<T>(int a, int b, global::System.ReadOnlySpan<T> c) where T : struct;",
            generatedSource);
        Assert.Contains(
            """
            void DoSomething3Checked<T>(int a, int b, global::System.ReadOnlySpan<T> c) where T : struct
            #if !STANDARD
                , unmanaged
            #endif
            ;
            """,
            generatedSource);
    }
}