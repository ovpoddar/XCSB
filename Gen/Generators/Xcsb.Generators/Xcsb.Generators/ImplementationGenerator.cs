using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Xcsb.Generators;
using Xcsb.Generators.CodeGen;
using Xcsb.Generators.CodeGen.ClassGeneration;
using Xcsb.Generators.Helper;

namespace Xcsb.Generators;

[Generator]
public class ImplementationGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(ctx =>
        {
            ctx.AddSource(ImplementationAttributeCode.FileName, ImplementationAttributeCode.Context);
        });

        var provider = context.SyntaxProvider.ForAttributeWithMetadataName(
                ImplementationAttributeCode.AttributeName,
                predicate: static (node, _) => node is ClassDeclarationSyntax,
                transform: static (ctx, _) =>
                {
                    var attributeValues = AttributeTransformer.GetAllAttributeValues(ctx.Attributes.FirstOrDefault())
                        .ToArray();
                    if (attributeValues.Length != 2)
                        return (null, null, null);

                    return (kindInfo: AttributeTransformer.GetKind<ImplementationKind>(attributeValues[0]),
                        interfaceSymbol: AttributeTransformer.GetInterfaceSymbol(attributeValues[1], ctx),
                        classSymbol: ctx.TargetSymbol as INamedTypeSymbol);
                })
            .Where(a =>
                a is { kindInfo: not null, interfaceSymbol: not null, classSymbol: not null })
            .Select((b, _) =>
                (kindInfo: b.kindInfo!.Value, interfaceSymbol: b.interfaceSymbol!, classSymbol: b.classSymbol!));

        context.RegisterSourceOutput(provider, (ctx, result) =>
        {
            var diagnosis = GeneratorDiagnostics.FindFirstOffendingMethod(result.interfaceSymbol);
            if (diagnosis != null)
            {
                ctx.ReportDiagnostic(Diagnostic.Create(
                    GeneratorDiagnostics.ImplementationInvalidReturnType,
                    diagnosis.Locations.FirstOrDefault() ??
                    result.classSymbol.Locations.FirstOrDefault() ?? Location.None,
                    result.classSymbol.Name,
                    result.interfaceSymbol.Name,
                    diagnosis.ReturnType.ToDisplayString()));
                return;
            }

            ctx.AddSource($"{result.classSymbol.Name}.Implementation.g.cs", SourceText.From(
                ClassCodeGenerator.Generate1(result.classSymbol, result.interfaceSymbol, result.kindInfo).ToString(),
                Encoding.UTF8));
        });
    }
}