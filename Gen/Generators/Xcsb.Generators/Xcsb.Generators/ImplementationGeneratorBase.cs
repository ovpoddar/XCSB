using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Xcsb.Generators.CodeGen;
using Xcsb.Generators.CodeGen.ClassGeneration;
using Xcsb.Generators.SourceGenerator;

namespace Xcsb.Generators;

public abstract class ImplementationGeneratorBase : IIncrementalGenerator
{
    protected abstract string AttributeFullName { get; }
    protected abstract string AttributeSourceCode { get; }
    protected abstract string GeneratedSuffix { get; }
    protected abstract string ClassGenerator(INamedTypeSymbol classSymbol, INamedTypeSymbol interfaceSymbol);

    private string AttributeDisplayName => $"{GeneratedSuffix}Implementation";

    private sealed class AttributeTargetComparer
        : IEqualityComparer<(INamedTypeSymbol classSymbol, INamedTypeSymbol? interfaceSymbol, Location? attributeLocation)>
    {
        public static readonly AttributeTargetComparer Instance = new();

        public bool Equals(
            (INamedTypeSymbol classSymbol, INamedTypeSymbol? interfaceSymbol, Location? attributeLocation) x,
            (INamedTypeSymbol classSymbol, INamedTypeSymbol? interfaceSymbol, Location? attributeLocation) y) =>
            SymbolEqualityComparer.Default.Equals(x.classSymbol, y.classSymbol) &&
            SymbolEqualityComparer.Default.Equals(x.interfaceSymbol, y.interfaceSymbol) &&
            Equals(x.attributeLocation, y.attributeLocation);

        public int GetHashCode(
            (INamedTypeSymbol classSymbol, INamedTypeSymbol? interfaceSymbol, Location? attributeLocation) obj)
        {
            unchecked
            {
                var hash = SymbolEqualityComparer.Default.GetHashCode(obj.classSymbol);
                hash = (hash * 397) ^ (obj.interfaceSymbol is null
                    ? 0
                    : SymbolEqualityComparer.Default.GetHashCode(obj.interfaceSymbol));
                hash = (hash * 397) ^ (obj.attributeLocation?.GetHashCode() ?? 0);
                return hash;
            }
        }
    }

    private readonly record struct ImplementationResult
    {
        public readonly string HintName;
        public readonly string ClassName;
        public readonly string InterfaceName;
        public readonly string? Source;
        public readonly string? OffendingMethodName;
        public readonly string? OffendingReturnType;
        public readonly Location? OffendingLocation;

        public ImplementationResult(
            string hintName, string className, string interfaceName, string? source,
            string? offendingMethodName, string? offendingReturnType, Location? offendingLocation)
        {
            HintName = hintName;
            ClassName = className;
            InterfaceName = interfaceName;
            Source = source;
            OffendingMethodName = offendingMethodName;
            OffendingReturnType = offendingReturnType;
            OffendingLocation = offendingLocation;
        }
    }

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
                    var attribute = ctx.Attributes.Single();
                    var attributeSyntax = attribute.ApplicationSyntaxReference?.GetSyntax() as AttributeSyntax;
                    INamedTypeSymbol? interfaceSymbol;
                    if (attribute.ConstructorArguments.Length != 0)
                        interfaceSymbol = attribute.ConstructorArguments[0].Value as INamedTypeSymbol;
                    else
                    {
                        interfaceSymbol =
                            attributeSyntax?.ArgumentList?.Arguments[0].Expression is TypeOfExpressionSyntax attributeType
                                ? ctx.SemanticModel.GetTypeInfo(attributeType.Type).Type as INamedTypeSymbol
                                : null;
                    }

                    return (classSymbol, interfaceSymbol, attributeLocation: attributeSyntax?.GetLocation());
                })
            .WithComparer(AttributeTargetComparer.Instance)
            .Where(static a => a.interfaceSymbol is not null)
            .Select((a, _) =>
            {
                var hintName = $"{a.classSymbol.Name}.{GeneratedSuffix}.g.cs";
                var offender = GeneratorDiagnostics.FindFirstOffendingMethod(a.interfaceSymbol!);
                if (offender is null)
                {
                    return new ImplementationResult(
                        hintName, a.classSymbol.Name, a.interfaceSymbol!.Name,
                        ClassGenerator(a.classSymbol, a.interfaceSymbol!),
                        null, null, null);
                }

                return new ImplementationResult(
                    hintName, a.classSymbol.Name, a.interfaceSymbol!.Name, null,
                    offender.Name,
                    offender.ReturnType.ToDisplayString(),
                    a.attributeLocation ?? a.classSymbol.Locations.FirstOrDefault());
            });

        context.RegisterSourceOutput(provider, (ctx, result) =>
        {
            if (result.OffendingMethodName is not null)
            {
                ctx.ReportDiagnostic(Diagnostic.Create(
                    GeneratorDiagnostics.ImplementationInvalidReturnType,
                    result.OffendingLocation ?? Location.None,
                    result.OffendingMethodName, result.InterfaceName, result.OffendingReturnType,
                    AttributeDisplayName, result.ClassName));
                return;
            }

            ctx.AddSource(result.HintName, SourceText.From(result.Source!, Encoding.UTF8));
        });
    }
}