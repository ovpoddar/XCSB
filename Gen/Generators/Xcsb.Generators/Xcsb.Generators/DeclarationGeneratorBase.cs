using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Xcsb.Generators.CodeGen;
using Xcsb.Generators.CodeGen.InterfaceGeneration;

namespace Xcsb.Generators;

[Generator]
public class DeclarationGeneratorBase : IIncrementalGenerator
{
    private const string DeclarationName = "DeclarationAttribute";

    private readonly record struct DeclarationResult
    {
        public readonly string InterfaceName;
        public readonly string? Source;
        public readonly string? OffendingMethodName;
        public readonly string? OffendingReturnType;
        public readonly Location? OffendingLocation;

        public DeclarationResult(string interfaceName,
            string? source,
            string? offendingMethodName,
            string? offendingReturnType,
            Location? offendingLocation)
        {
            InterfaceName = interfaceName;
            Source = source;
            OffendingMethodName = offendingMethodName;
            OffendingReturnType = offendingReturnType;
            OffendingLocation = offendingLocation;
        }
    }

    public static IEnumerable<AttributeValue>? GetAllAttributeValues(AttributeData? attr)
    {
        if (attr is null)
            yield break;

        var parameters = attr.AttributeConstructor?.Parameters ?? default;

        for (int i = 0; i < attr.ConstructorArguments.Length; i++)
        {
            string name = i < parameters.Length ? parameters[i].Name : $"arg{i}";
            TypedConstant arg = attr.ConstructorArguments[i];

            yield return new AttributeValue(
                name,
                Unwrap(arg),
                AttributeValueSource.Constructor,
                arg.Type?.ToDisplayString());
        }

        foreach (var named in attr.NamedArguments)
        {
            yield return new AttributeValue(
                named.Key,
                Unwrap(named.Value),
                AttributeValueSource.Named,
                named.Value.Type?.ToDisplayString());
        }
    }

    static object? Unwrap(TypedConstant c) => c.Kind switch
    {
        TypedConstantKind.Error => null,
        TypedConstantKind.Array => c.Values.Select(Unwrap).ToArray(),
        TypedConstantKind.Type => (c.Value as ITypeSymbol)?.ToDisplayString(),
        _ => c.Value
    };

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(ctx =>
        {
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

            ctx.AddSource(
                $"Xcsb.Generators.CheckedDeclarationAttribute.g.cs",
                SourceText.From(
                    $$"""
                      using System;

                      namespace Xcsb.Generators;

                      [AttributeUsage(AttributeTargets.Interface)]
                      public class CheckedDeclarationAttribute : Attribute
                      {

                      }
                      """, Encoding.UTF8));

            ctx.AddSource(
                $"Xcsb.Generators.UncheckedDeclarationAttribute.g.cs",
                SourceText.From(
                    $$"""
                      using System;

                      namespace Xcsb.Generators;

                      [AttributeUsage(AttributeTargets.Interface)]
                      public class UncheckedDeclarationAttribute : Attribute
                      {

                      }
                      """, Encoding.UTF8));

            ctx.AddSource(DeclarationAttributeCodeNew.FileName, DeclarationAttributeCodeNew.Context);
        });

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


        var Checked = context.SyntaxProvider.ForAttributeWithMetadataName(
                "Xcsb.Generators.CheckedDeclarationAttribute",
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
                            interfaceSuffix: "Checked",
                            methodSuffix: "Checked",
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

        context.RegisterSourceOutput(Checked, (ctx, result) =>
        {
            if (result.OffendingMethodName is not null)
            {
                ctx.ReportDiagnostic(Diagnostic.Create(
                    GeneratorDiagnostics.DeclarationInvalidReturnType,
                    result.OffendingLocation ?? Location.None,
                    result.OffendingMethodName, result.InterfaceName, result.OffendingReturnType,
                    "CheckedDeclaration"));
                return;
            }

            ctx.AddSource($"{result.InterfaceName}Checked.g.cs", SourceText.From(result.Source!, Encoding.UTF8));
        });

        var Unchecked = context.SyntaxProvider.ForAttributeWithMetadataName(
                "Xcsb.Generators.UncheckedDeclarationAttribute",
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
                            interfaceSuffix: "Unchecked",
                            methodSuffix: "Unchecked",
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

        context.RegisterSourceOutput(Unchecked, (ctx, result) =>
        {
            if (result.OffendingMethodName is not null)
            {
                ctx.ReportDiagnostic(Diagnostic.Create(
                    GeneratorDiagnostics.DeclarationInvalidReturnType,
                    result.OffendingLocation ?? Location.None,
                    result.OffendingMethodName, result.InterfaceName, result.OffendingReturnType,
                    "UncheckedDeclaration"));
                return;
            }

            ctx.AddSource($"{result.InterfaceName}Unchecked.g.cs", SourceText.From(result.Source!, Encoding.UTF8));
        });

        var provider = context.SyntaxProvider.ForAttributeWithMetadataName(
                DeclarationAttributeCodeNew.AttributeName,
                predicate: static (node, _) => node is InterfaceDeclarationSyntax,
                transform: static (ctx, _) =>
                    (attribute: GetAllAttributeValues(ctx.Attributes.FirstOrDefault()),
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

            // var declaration = i switch
            // {
            //     1 => InterfaceCodeGenerator.Generate(result.symbol, interfaceSuffix: "Checked",
            //         methodSuffix: "Checked", returnTypeProvider: _ => "void"),
            //     2 => InterfaceCodeGenerator.Generate(result.symbol, interfaceSuffix: "Unchecked",
            //         methodSuffix: "Unchecked", returnTypeProvider: _ => "void"),
            //     4 => InterfaceCodeGenerator.Generate(result.symbol, interfaceSuffix: "Unchecked",
            //         methodSuffix: "Unchecked", returnTypeProvider: _ => "void"),
            //     _ => throw new Exception()
            // };

            ctx.AddSource("Declaration.g.cs", SourceText.From(InterfaceCodeGeneratorNew.Generate(result.symbol,
                (DeclarationKind)i).ToString(), Encoding.UTF8));
        });
    }
}

public enum AttributeValueSource
{
    Constructor,
    Named
}

public readonly record struct AttributeValue
{
    public readonly string Name;
    public readonly object? Value;
    public readonly AttributeValueSource Source;
    public readonly string? TypeName;

    public AttributeValue(string name, object? value, AttributeValueSource source, string? typeName)
    {
        Name = name;
        Value = value;
        Source = source;
        TypeName = typeName;
    }
}