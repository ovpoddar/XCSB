using Xunit;
using Xcsb.Generators.Tests.Utils;
using System;

namespace Xcsb.Generators.Tests;

public class UncheckedDeclarationGeneratorTests
{

    [Fact]
    public void Generator_ShouldGenerateClass_WhenInterfaceHasAttribute()
    {
        var source = @"
using Xcsb.Generators;
namespace TestNamespace
{
    [Declaration(DeclarationKind.Unchecked)]
    public partial interface ITestService
    {
    }
}";

        var generatedSource = TestHelper.GenerateSource<DeclarationGeneratorBase>(source, string.Empty,
            "Declaration.g.cs",
            validateOutputCompilation: true);

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
    [Declaration(DeclarationKind.Unchecked)]
    public partial interface ITestService
    {
        int DoStaff();
        byte Check { get; set; }
        byte Check12 { get; }
    }
}";

        var generatedSource = TestHelper.GenerateSource<DeclarationGeneratorBase>(source, string.Empty,
            "Declaration.g.cs",
            validateOutputCompilation: true);

        Assert.Contains("void DoStaffUnchecked();", generatedSource);
    }

    private const string attribute =
        """
        namespace Xcsb.Generators
        {
            [global::System.Flags]
            public enum DeclarationKind
            {
                Checked = 1,
                Unchecked = 2,
                Buffer = 4,
            }
            
            [global::System.AttributeUsage(global::System.AttributeTargets.Interface, AllowMultiple = false, Inherited = false)]
            public class DeclarationAttribute : global::System.Attribute
            {
                public DeclarationAttribute(global::Xcsb.Generators.DeclarationKind kind)
                {
                }
            
                private DeclarationAttribute()
                {
                }
            }
        }
        
        """;
    
    [Fact]
    public void Generator_ShouldGenerateClass_ContainsConstrainMethods_WhenInterfaceHasAttribute()
    {
        var source = @"
#define STANDARD
using Xcsb.Generators;
namespace TestNamespace
{
    [Declaration(DeclarationKind.Unchecked | DeclarationKind.Checked)]
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

        var generatedSource = TestHelper.GenerateSource<DeclarationGeneratorBase>(source, attribute,
            "Declaration.g.cs");

        Assert.Contains("void DoSomethingUnchecked<T>(int a, int b, T c);", generatedSource);
        Assert.Contains("void DoSomething1Unchecked<T>(int a, int b, global::System.ReadOnlySpan<T> c);",
            generatedSource);
        Assert.Contains(
            "void DoSomething2Unchecked<T>(int a, int b, global::System.ReadOnlySpan<T> c) where T : struct;",
            generatedSource);
        Assert.Contains(
            """
            void DoSomething3Unchecked<T>(int a, int b, global::System.ReadOnlySpan<T> c) where T : struct
            #if !STANDARD
                , unmanaged
            #endif
            ;
            """,
            generatedSource);
    }

    [Theory]
    [InlineData("void")]
    [InlineData("System.Threading.Tasks.Task")]
    [InlineData("System.Threading.Tasks.Task<int>")]
    [InlineData("System.Threading.Tasks.ValueTask")]
    [InlineData("System.Threading.Tasks.ValueTask<int>")]
    public void Generator_ShouldReportDiagnostic_WhenMethodReturnsVoidOrTaskLike(string returnType)
    {
        var source = $@"
using Xcsb.Generators;
namespace TestNamespace
{{
    [Declaration(DeclarationKind.Unchecked)]
    public partial interface ITestService
    {{
        {returnType} DoBad();
    }}
}}";

        TestHelper.AssertDiagnostic<DeclarationGeneratorBase>(source, string.Empty, "XCSBGEN001");
    }
}