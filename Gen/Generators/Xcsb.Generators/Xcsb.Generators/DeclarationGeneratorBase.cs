using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Xcsb.Generators.CodeGen;
using Xcsb.Generators.CodeGen.InterfaceGeneration;
using Xcsb.Generators.Helper;

namespace Xcsb.Generators;

[Generator]
public class DeclarationGeneratorBase : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(ctx =>
        {
/*
            ctx.AddSource(
                $"Xcsb.Generators.BufferDeclarationAttribute.g.cs",
                SourceText.From(
                    $$"""
                      using System;

                      namespace Xcsb.Generators;

                      [AttributeUsage(AttributeTargets.Interface)]
                      public class BufferDeclarationAttribute : Attribute
                      {

                      }
                      """, Encoding.UTF8));
*/
            ctx.AddSource(DeclarationAttributeCodeNew.FileName, DeclarationAttributeCodeNew.Context);
        });
/*
        var buffer = context.SyntaxProvider.ForAttributeWithMetadataName(
                "Xcsb.Generators.BufferDeclarationAttribute",
                predicate: static (node, _) => node is InterfaceDeclarationSyntax,
                transform: static (ctx, _) => (INamedTypeSymbol)ctx.TargetSymbol)
            .WithComparer(SymbolEqualityComparer.Default)
            .Select((interfaceSymbol, _) =>
            {
                var offender = GeneratorDiagnostics.FindFirstOffendingMethod(interfaceSymbol);
                if (offender is null)
                {
                    return new DeclarationResult(
                        interfaceSymbol.Name,
                        InterfaceCodeGenerator.Generate(
                            interfaceSymbol,
                            interfaceSuffix: "Buffer",
                            methodSuffix: string.Empty,
                            returnTypeProvider: _ => "void"
                        ),
                        null, null, null);
                }

                return new DeclarationResult(
                    interfaceSymbol.Name, null,
                    offender.Name,
                    offender.ReturnType.ToDisplayString(),
                    offender.Locations.FirstOrDefault() ?? interfaceSymbol.Locations.FirstOrDefault());
            });

        context.RegisterSourceOutput(buffer, (ctx, result) =>
        {
            if (result.OffendingMethodName is not null)
            {
                ctx.ReportDiagnostic(Diagnostic.Create(
                    GeneratorDiagnostics.DeclarationInvalidReturnType,
                    result.OffendingLocation ?? Location.None,
                    result.OffendingMethodName, result.InterfaceName, result.OffendingReturnType,
                    "BufferDeclaration"));
                return;
            }

            ctx.AddSource($"{result.InterfaceName}Buffer.g.cs", SourceText.From(result.Source!, Encoding.UTF8));
        });

*/
        var provider = context.SyntaxProvider.ForAttributeWithMetadataName(
                DeclarationAttributeCodeNew.AttributeName,
                predicate: static (node, _) => node is InterfaceDeclarationSyntax,
                transform: static (ctx, _) =>
                    (attribute: AttributeTransformer.GetAllAttributeValues(ctx.Attributes.FirstOrDefault()),
                        symbol: ctx.TargetSymbol as INamedTypeSymbol))
            .Where(a => a.attribute is not null && a.symbol is not null)
            .Select((a, _) =>
                (symbol: a.symbol!,
                    info: a.attribute!.FirstOrDefault(b => b is
                        { Name: "kind", TypeName: "Xcsb.Generators.DeclarationKind" })));

        context.RegisterSourceOutput(provider, (ctx, result) =>
        {
            if (result.info.Value is not int i)
                return;
            var diagnosis = GeneratorDiagnostics.FindFirstOffendingMethod(result.symbol!);
            if (diagnosis is not null)
            {
                ctx.ReportDiagnostic(Diagnostic.Create(
                    GeneratorDiagnostics.DeclarationInvalidReturnType,
                    diagnosis.Locations.FirstOrDefault() ?? result.symbol.Locations.FirstOrDefault() ?? Location.None,
                    diagnosis.Name,
                    diagnosis.Name,
                    diagnosis.ReturnType.ToDisplayString(),
                    DeclarationAttributeCodeNew.Type));
                return;
            }
            
            ctx.AddSource("Declaration.g.cs", SourceText.From(InterfaceCodeGeneratorNew.Generate(result.symbol,
                (DeclarationKind)i).ToString(), Encoding.UTF8));
        });
    }
}
