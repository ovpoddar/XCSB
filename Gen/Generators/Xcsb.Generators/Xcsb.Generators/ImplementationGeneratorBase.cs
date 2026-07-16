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
                    var attribute = ctx.Attributes.Single(a =>
                        a.AttributeClass?.ToDisplayString(_symbolDisplayFormat) ==
                        DefinitionAttributeCode.Checked.FullName);
                    INamedTypeSymbol? interfaceSymbol;
                    if (attribute.ConstructorArguments.Length != 0)
                        interfaceSymbol = attribute.ConstructorArguments[0].Value as INamedTypeSymbol;
                    else
                    {
                        var attributeSyntax = attribute.ApplicationSyntaxReference?.GetSyntax() as AttributeSyntax;
                        interfaceSymbol =
                            attributeSyntax?.ArgumentList?.Arguments[0].Expression is TypeOfExpressionSyntax attributeType
                                ? ctx.SemanticModel.GetTypeInfo(attributeType.Type).Type as INamedTypeSymbol
                                : null;
                    }

                    return (classSymbol, interfaceSymbol);
                })
            .Where(static a => a.interfaceSymbol is not null);

        context.RegisterSourceOutput(provider, (ctx, symbols) =>
        {
            var code = ClassCodeGenerator.Generate(symbols.classSymbol, symbols.interfaceSymbol!);
            ctx.AddSource($"{symbols.classSymbol.Name}.{DefinitionAttributeCode.Checked.SuffixName}.g.cs",
                SourceText.From(code, Encoding.UTF8));
        });
    }
}