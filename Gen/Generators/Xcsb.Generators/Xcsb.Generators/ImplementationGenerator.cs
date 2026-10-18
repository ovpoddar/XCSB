using System.CodeDom.Compiler;
using System.Collections.Generic;
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
                    var attributeValues = AttributeTransformer.GetAllAttributeValues(ctx.Attributes)
                        .ToArray();
                    if (attributeValues.Length % 2 != 0)
                        return ([], null);

                    var collection = new List<(ImplementationKind, INamedTypeSymbol)>();
                    for (var i = 0; i < attributeValues.Length; i += 2)
                    {
                        var interfaceSymbol = AttributeTransformer.GetInterfaceSymbol(attributeValues[i + 1], ctx);
                        var kindInfo = AttributeTransformer.GetKind<ImplementationKind>(attributeValues[i]);
                        if (interfaceSymbol == null || !kindInfo.HasValue)
                            continue;

                        var index = collection.FindIndex(a => a.Item2.Name == interfaceSymbol.Name);
                        if (index < 0)
                            collection.Add((kindInfo.Value, interfaceSymbol));
                        else
                        {
                            var existing = collection[index];
                            collection[index] = (existing.Item1 | kindInfo.Value, existing.Item2);
                        }
                    }

                    return (collection, classSymbol: ctx.TargetSymbol as INamedTypeSymbol);
                })
            .Where(a =>
                a is { classSymbol: not null })
            .Select((b, _) =>
                (b.collection, classSymbol: b.classSymbol!));

        context.RegisterSourceOutput(provider, (ctx, result) =>
        {
            foreach (var interfaceMember in result.collection)
            {
                var diagnosis = GeneratorDiagnostics.FindFirstOffendingMethod(interfaceMember.Item2);
                if (diagnosis == null)
                    continue;

                ctx.ReportDiagnostic(Diagnostic.Create(
                    GeneratorDiagnostics.ImplementationInvalidReturnType,
                    diagnosis.Locations.FirstOrDefault() ??
                    result.classSymbol.Locations.FirstOrDefault() ?? Location.None,
                    result.classSymbol.Name,
                    interfaceMember.Item2.Name,
                    diagnosis.ReturnType.ToDisplayString()));
                return;
            }

            ctx.AddSource($"{result.classSymbol.Name}.Implementation.g.cs", SourceText.From(
                ClassCodeGenerator.Generate(result.classSymbol, result.collection).ToString(),
                Encoding.UTF8));
        });
    }
}