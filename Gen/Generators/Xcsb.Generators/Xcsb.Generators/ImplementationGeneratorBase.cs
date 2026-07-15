using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Xcsb.Generators.CodeGen.ClassGeneration;
using Xcsb.Generators.SourceGenerator;

namespace Xcsb.Generators;

[Generator]
public sealed class ImplementationGeneratorBase : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(ctx =>
        {
            ctx.AddSource(
                $"{DefinitionAttributeCode.Checked.FullName}.g.cs",
                SourceText.From(DefinitionAttributeCode.Checked.Source, Encoding.UTF8));
        });
        
        var provider = context.SyntaxProvider.ForAttributeWithMetadataName(
                "Xcsb.Generators.CheckedImplementationAttribute",
                predicate: static (node, _) => node is ClassDeclarationSyntax,
                transform: static (ctx, _) => (INamedTypeSymbol)ctx.TargetSymbol)
            .WithComparer(SymbolEqualityComparer.Default);

        context.RegisterSourceOutput(provider, (ctx, interfaceSymbol) =>
        {
            var code =
            $$"""
            internal partial class {{interfaceSymbol.Name}}Implementation
            {
            }
            """;
            ctx.AddSource($"{interfaceSymbol.Name}.g.cs", SourceText.From(code, Encoding.UTF8));
        });
    }
}