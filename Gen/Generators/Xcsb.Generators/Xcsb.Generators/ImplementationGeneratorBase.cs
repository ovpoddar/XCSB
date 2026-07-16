using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Xcsb.Generators.CodeGen.ClassGeneration;
using Xcsb.Generators.SourceGenerator;

namespace Xcsb.Generators;

public abstract class ImplementationGeneratorBase : IIncrementalGenerator
{
    private static readonly SymbolDisplayFormat _symbolDisplayFormat =
        new SymbolDisplayFormat(
            typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces);
    protected abstract string AttributeFullName { get; }
    protected abstract string AttributeSourceCode { get; }
    protected abstract string GeneratedSuffix { get; }
    protected abstract string ClassGenerator(INamedTypeSymbol classSymbol, INamedTypeSymbol interfaceSymbol);
    
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(ctx =>
        {
            ctx.AddSource($"{AttributeFullName}.g.cs", SourceText.From(AttributeSourceCode, Encoding.UTF8));
        });

        var provider = context.SyntaxProvider.ForAttributeWithMetadataName(
                AttributeFullName,
                predicate: static (node, _) => node is ClassDeclarationSyntax,
                transform: (ctx, _) =>
                {
                    var classSymbol = (INamedTypeSymbol)ctx.TargetSymbol;
                    var attribute = ctx.Attributes.Single(a =>
                        a.AttributeClass?.ToDisplayString(_symbolDisplayFormat) == AttributeFullName);
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
            var code = ClassGenerator(symbols.classSymbol, symbols.interfaceSymbol!);
            ctx.AddSource($"{symbols.classSymbol.Name}.{GeneratedSuffix}.g.cs",
                SourceText.From(code, Encoding.UTF8));
        });
    }
}