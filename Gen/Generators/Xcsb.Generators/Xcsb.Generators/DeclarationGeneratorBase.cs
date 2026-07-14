using System;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Xcsb.Generators;

public abstract class DeclarationGeneratorBase : IIncrementalGenerator
{
    protected abstract string AttributeFullName { get; }
    protected abstract string AttributeSourceCode { get; }
    protected abstract string GeneratedFileSuffix { get; }

    protected abstract string GenerateInterfaceImplementation(INamedTypeSymbol interfaceSymbol);

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(ctx =>
        {
            ctx.AddSource(
                $"{AttributeFullName}.g.cs",
                SourceText.From(AttributeSourceCode, Encoding.UTF8));
        });

        var provider = context.SyntaxProvider.ForAttributeWithMetadataName(
            AttributeFullName,
            predicate: static (node, _) => node is InterfaceDeclarationSyntax,
            transform: static (ctx, _) => (INamedTypeSymbol)ctx.TargetSymbol)
            .WithComparer(SymbolEqualityComparer.Default);

        context.RegisterSourceOutput(provider, (ctx, interfaceSymbol) =>
        {
            var code = GenerateInterfaceImplementation(interfaceSymbol);
            ctx.AddSource($"{interfaceSymbol.Name}{GeneratedFileSuffix}.g.cs", SourceText.From(code, Encoding.UTF8));
        });
    }
}
