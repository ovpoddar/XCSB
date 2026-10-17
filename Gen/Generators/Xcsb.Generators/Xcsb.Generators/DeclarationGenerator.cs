using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Xcsb.Generators.CodeGen;
using Xcsb.Generators.CodeGen.InterfaceGeneration;
using Xcsb.Generators.Helper;

namespace Xcsb.Generators;

[Generator]
public class DeclarationGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(ctx =>
        {
            ctx.AddSource(DeclarationAttributeCode.FileName, DeclarationAttributeCode.Context);
        });

        var provider = context.SyntaxProvider.ForAttributeWithMetadataName(
                DeclarationAttributeCode.AttributeName,
                predicate: static (node, _) => node is InterfaceDeclarationSyntax,
                transform: static (ctx, _) =>
                    (kindInfo: AttributeTransformer.GetKind<DeclarationKind>(
                            AttributeTransformer.GetAllAttributeValues(ctx.Attributes).FirstOrDefault()),
                        symbol: ctx.TargetSymbol as INamedTypeSymbol))
            .Where(a => a.symbol is not null && a.kindInfo.HasValue)
            .Select((a, _) => (symbol: a.symbol!, info: a.kindInfo!.Value));

        context.RegisterSourceOutput(provider, (ctx, result) =>
        {
            var diagnosis = GeneratorDiagnostics.FindFirstOffendingMethod(result.symbol);
            if (diagnosis is not null)
            {
                ctx.ReportDiagnostic(Diagnostic.Create(
                    GeneratorDiagnostics.DeclarationInvalidReturnType,
                    diagnosis.Locations.FirstOrDefault() ?? result.symbol.Locations.FirstOrDefault() ?? Location.None,
                    diagnosis.Name,
                    diagnosis.Name,
                    diagnosis.ReturnType.ToDisplayString(),
                    DeclarationAttributeCode.Type));
                return;
            }

            ctx.AddSource($"{result.symbol.Name}.Declaration.g.cs", SourceText.From(
                InterfaceCodeGenerator.Generate(result.symbol, result.info).ToString(),
                Encoding.UTF8));
        });
    }
}