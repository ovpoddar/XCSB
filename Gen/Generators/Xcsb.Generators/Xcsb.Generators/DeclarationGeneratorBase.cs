using System;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Xcsb.Generators.CodeGen;

namespace Xcsb.Generators;

public abstract class DeclarationGeneratorBase : IIncrementalGenerator
{
    protected abstract string AttributeFullName { get; }
    protected abstract string AttributeSourceCode { get; }
    protected abstract string GeneratedSuffix { get; }

    private string AttributeDisplayName => $"{GeneratedSuffix}Declaration";

    protected abstract string GenerateInterfaceImplementation(INamedTypeSymbol interfaceSymbol);

    private readonly record struct DeclarationResult
    {
        public readonly string HintName;
        public readonly string InterfaceName;
        public readonly string? Source;
        public readonly string? OffendingMethodName;
        public readonly string? OffendingReturnType;
        public readonly Location? OffendingLocation;

        public DeclarationResult(
            string hintName, string interfaceName, string? source,
            string? offendingMethodName, string? offendingReturnType, Location? offendingLocation)
        {
            HintName = hintName;
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
            ctx.AddSource(
                $"{AttributeFullName}.g.cs",
                SourceText.From(AttributeSourceCode, Encoding.UTF8));
        });

        var provider = context.SyntaxProvider.ForAttributeWithMetadataName(
                AttributeFullName,
                predicate: static (node, _) => node is InterfaceDeclarationSyntax,
                transform: static (ctx, _) => (INamedTypeSymbol)ctx.TargetSymbol)
            .WithComparer(SymbolEqualityComparer.Default)
            .Select((interfaceSymbol, _) =>
            {
                var hintName = $"{interfaceSymbol.Name}{GeneratedSuffix}.g.cs";
                var offender = GeneratorDiagnostics.FindFirstOffendingMethod(interfaceSymbol);
                if (offender is null)
                {
                    return new DeclarationResult(
                        hintName, interfaceSymbol.Name, GenerateInterfaceImplementation(interfaceSymbol),
                        null, null, null);
                }

                return new DeclarationResult(
                    hintName, interfaceSymbol.Name, null,
                    offender.Name,
                    offender.ReturnType.ToDisplayString(),
                    offender.Locations.FirstOrDefault() ?? interfaceSymbol.Locations.FirstOrDefault());
            });

        context.RegisterSourceOutput(provider, (ctx, result) =>
        {
            if (result.OffendingMethodName is not null)
            {
                ctx.ReportDiagnostic(Diagnostic.Create(
                    GeneratorDiagnostics.DeclarationInvalidReturnType,
                    result.OffendingLocation ?? Location.None,
                    result.OffendingMethodName, result.InterfaceName, result.OffendingReturnType,
                    AttributeDisplayName));
                return;
            }

            ctx.AddSource(result.HintName, SourceText.From(result.Source!, Encoding.UTF8));
        });
    }
}