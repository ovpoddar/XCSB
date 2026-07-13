using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Linq;
using Xunit;
using Xcsb.Generators;
using System.Reflection;

namespace Xcsb.Generators.Tests;

public class CheckedDeclarationGeneratorTests
{
    [Fact]
    public void Generator_ShouldGenerateClass_WhenInterfaceHasAttribute()
    {
        // Arrange
        var source = @"
using Xcsb.Generators;
namespace TestNamespace
{
    [CheckedDeclaration]
    public partial interface ITestService
    {
    }
}";

        var attributeSource = @"
using System;
namespace Xcsb.Generators
{
    [AttributeUsage(AttributeTargets.Interface)]
    public class CheckedDeclarationAttribute : Attribute {}
}";
        var compilation = CreateCompilation(source, attributeSource);
        var generator = new CheckedDeclarationGenerator();
        GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);

        // Act
        driver = driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out var diagnostics);

        // Assert
        Assert.Empty(diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error));

        var runResult = driver.GetRunResult();
        Assert.Equal(2, runResult.GeneratedTrees.Length);
        
        var generatedSource = runResult.GeneratedTrees.First(t => t.FilePath.Contains("ITestServiceChecked.g.cs")).GetText().ToString();
        Assert.Contains("public interface ITestServiceChecked", generatedSource);
        Assert.Contains("namespace TestNamespace", generatedSource);
    }

    [Fact]
    public void Generator_ShouldGenerateClass_ContainsMethods_WhenInterfaceHasAttribute()
    {
        // Arrange
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

        var attributeSource = @"
using System;
namespace Xcsb.Generators
{
    [AttributeUsage(AttributeTargets.Interface)]
    public class CheckedDeclarationAttribute : Attribute {}
}";
        var compilation = CreateCompilation(source, attributeSource);
        var generator = new CheckedDeclarationGenerator();
        GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);

        // Act
        driver = driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out var diagnostics);

        // Assert
        Assert.Empty(diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error));

        var runResult = driver.GetRunResult();
        Assert.Equal(2, runResult.GeneratedTrees.Length);
        
        var generatedSource = runResult.GeneratedTrees.First(t => t.FilePath.Contains("ITestServiceChecked.g.cs")).GetText().ToString();
        Assert.Contains("void DoStaffChecked();", generatedSource);
    }

    
    [Fact]
    public void Generator_ShouldGenerateClass_ContainsConstrainMethods_WhenInterfaceHasAttribute()
    {
        // Arrange
        var source = @"
using Xcsb.Generators;
namespace TestNamespace
{
    [CheckedDeclaration]
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

        var attributeSource = @"
using System;
namespace Xcsb.Generators
{
    [AttributeUsage(AttributeTargets.Interface)]
    public class CheckedDeclarationAttribute : Attribute {}
}";
        var compilation = CreateCompilation(source, attributeSource);
        var generator = new CheckedDeclarationGenerator();
        GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);

        // Act
        driver = driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out var diagnostics);

        // Assert
        Assert.Empty(diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error));

        var runResult = driver.GetRunResult();
        Assert.Equal(2, runResult.GeneratedTrees.Length);
        
        var generatedSource = runResult.GeneratedTrees.First(t => t.FilePath.Contains("ITestServiceChecked.g.cs")).GetText().ToString();
        Assert.Contains("void DoSomethingChecked<T>(int a, int b, T c);", generatedSource);
        Assert.Contains("void DoSomething1Checked<T>(int a, int b, ReadonlySpan<T> c);", generatedSource);
        Assert.Contains("void DoSomething2Checked<T>(int a, int b, ReadonlySpan<T> c) where T : struct;", generatedSource);
        Assert.Contains("void DoSomething3Checked<T>(int a, int b, ReadonlySpan<T> c) where T : struct\n#if !NETSTANDARD\n    , unmanaged\n#endif\n;", generatedSource);
    }

    
    private static Compilation CreateCompilation(params string[] sources)
    {
        var syntaxTrees = sources.Select(s => CSharpSyntaxTree.ParseText(s));
        
        var references = new[]
        {
            MetadataReference.CreateFromFile(typeof(object).GetTypeInfo().Assembly.Location),
            MetadataReference.CreateFromFile(typeof(Enumerable).GetTypeInfo().Assembly.Location),
            MetadataReference.CreateFromFile(typeof(System.Runtime.AssemblyTargetedPatchBandAttribute).GetTypeInfo().Assembly.Location)
        };

        return CSharpCompilation.Create("TestCompilation",
            syntaxTrees,
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
    }
}
