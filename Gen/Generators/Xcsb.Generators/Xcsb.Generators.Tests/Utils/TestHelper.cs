using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Linq;
using Xunit;
using System.Reflection;

namespace Xcsb.Generators.Tests.Utils;

public static class TestHelper
{
    public static string GenerateSource<TGenerator>(string source, string attributeSource, string expectedFileSubstring)
        where TGenerator : IIncrementalGenerator, new()
    {
        var compilation = CreateCompilation(source, attributeSource);
        var generator = new TGenerator();
        GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);

        driver = driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out var diagnostics);

        var errors = compilation.GetDiagnostics().ToList();
        errors.AddRange(diagnostics);
        Assert.Empty(errors.Where(d => d.Severity == DiagnosticSeverity.Error));
        
        var runResult = driver.GetRunResult();
        Assert.Equal(2, runResult.GeneratedTrees.Length);
        
        return runResult.GeneratedTrees.First(t => t.FilePath.Contains(expectedFileSubstring)).GetText().ToString();
    }

    private static Compilation CreateCompilation(params string[] sources)
    {
        var syntaxTrees = sources
            .Where(s => !string.IsNullOrEmpty(s))
            .Select(s => CSharpSyntaxTree.ParseText(s));
        
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
