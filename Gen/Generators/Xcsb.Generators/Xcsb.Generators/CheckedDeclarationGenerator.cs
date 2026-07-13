using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Xcsb.Generators.CodeGen.CheckedDeclarationGeneration;

namespace Xcsb.Generators;

[Generator]
public class CheckedDeclarationGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(static ctx =>
        {
            ctx.AddSource(
                $"{nameof(CheckedDeclarationAttributeCode.CheckedDeclarationAttribute)}.g.cs",
                SourceText.From(CheckedDeclarationAttributeCode.CheckedDeclarationAttribute, Encoding.UTF8));
        });

        var provider = context.SyntaxProvider.ForAttributeWithMetadataName(
            CheckedDeclarationAttributeCode.AttributeFullName,
            static (node, _) => node is InterfaceDeclarationSyntax,
            static (ctx, _) => (INamedTypeSymbol)ctx.TargetSymbol);

        context.RegisterSourceOutput(context.CompilationProvider.Combine(provider.Collect()),
            (ctx, t) => GenerateCode(ctx, t.Left, t.Right));
    }

    private static void GenerateCode(SourceProductionContext context, Compilation compilation,
        ImmutableArray<INamedTypeSymbol> interfaceSymbols)
    {
        foreach (var interfaceSymbol in interfaceSymbols)
        {
            var code = CheckedDeclarationImplCode.GenerateInterfaceImplementation(interfaceSymbol);
            context.AddSource($"{interfaceSymbol.Name}Checked.g.cs", SourceText.From(code, Encoding.UTF8));
        }
    }
}