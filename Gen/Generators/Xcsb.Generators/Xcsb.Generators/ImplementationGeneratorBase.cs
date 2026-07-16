using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Xcsb.Generators.CodeGen.ClassGeneration;
using Xcsb.Generators.SourceGenerator;

namespace Xcsb.Generators;

[Generator]
public sealed class ImplementationGeneratorBase : IIncrementalGenerator
{
    private static readonly SymbolDisplayFormat _symbolDisplayFormat =
        new SymbolDisplayFormat(
            typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces);

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(ctx =>
        {
            ctx.AddSource(
                $"{DefinitionAttributeCode.Checked.FullName}.g.cs",
                SourceText.From(DefinitionAttributeCode.Checked.Source, Encoding.UTF8));
        });

        var provider = context.SyntaxProvider.ForAttributeWithMetadataName(
            DefinitionAttributeCode.Checked.FullName,
            predicate: static (node, _) => node is ClassDeclarationSyntax,
            transform: static (ctx, _) =>
            {
                var classSymbol = (INamedTypeSymbol)ctx.TargetSymbol;
                var interfaceSymbol = ctx.Attributes.Single(a =>
                    a.AttributeClass?.ToDisplayString(_symbolDisplayFormat) == DefinitionAttributeCode.Checked.FullName
                    && a.ConstructorArguments.Length == 1)
                    .ConstructorArguments[0].Value as INamedTypeSymbol;
                return (classSymbol, interfaceSymbol);
            });

        context.RegisterSourceOutput(provider, (ctx, symbols) =>
        {
            var code = ClassCodeGenerator.Generate(symbols.classSymbol, symbols.interfaceSymbol!);
            ctx.AddSource($"{symbols.classSymbol.Name}.{DefinitionAttributeCode.Checked.SuffixName}.g.cs",
                SourceText.From(code, Encoding.UTF8));
        });
    }
}