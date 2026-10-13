using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Xcsb.Generators.Helper;

internal static class AttributeTransformer
{
    public static IEnumerable<IAttributeValue> GetAllAttributeValues(AttributeData? attr)
    {
        if (attr is null)
            yield break;

        var hasValues = false;
        foreach (var typedConstant in attr.ConstructorArguments)
        {
            hasValues = true;
            yield return new SemanticValue(typedConstant);
        }

        foreach (var named in attr.NamedArguments)
        {
            hasValues = true;
            yield return new SemanticValue(named.Value);
        }

        if (hasValues || attr.ApplicationSyntaxReference?.GetSyntax() is not AttributeSyntax { ArgumentList: { } list })
            yield break;

        foreach (var arg in list.Arguments)
            yield return new SyntaxValue(arg.Expression);
    }

    public static TEnum? GetKind<TEnum>(IAttributeValue? symbol) where TEnum : struct, Enum =>
        symbol switch
        {
            SemanticValue semanticValue when semanticValue.Constant.Value is { } value =>
                (TEnum)Enum.ToObject(typeof(TEnum), value),
            SyntaxValue syntaxValue => ParseEnum<TEnum>(syntaxValue.Expression),
            _ => null
        };

    public static INamedTypeSymbol? GetInterfaceSymbol(IAttributeValue? symbol, GeneratorAttributeSyntaxContext ctx) =>
        symbol switch
        {
            SemanticValue semanticValue => semanticValue.Constant.Value as INamedTypeSymbol,
            SyntaxValue syntaxValue => GetInterfaceSymbol(syntaxValue.Expression, ctx),
            _ => null
        };

    private static INamedTypeSymbol? GetInterfaceSymbol(ExpressionSyntax expr, GeneratorAttributeSyntaxContext ctx) =>
        expr is TypeOfExpressionSyntax attributeType
            ? ctx.SemanticModel.GetTypeInfo(attributeType.Type).Type as INamedTypeSymbol
            : null;

    private static TEnum? ParseEnum<TEnum>(ExpressionSyntax expr) where TEnum : struct, Enum => expr switch
    {
        MemberAccessExpressionSyntax m => TryParseName<TEnum>(m.Name.Identifier.Text),
        IdentifierNameSyntax id => TryParseName<TEnum>(id.Identifier.Text),
        BinaryExpressionSyntax b when b.IsKind(SyntaxKind.BitwiseOrExpression) =>
            ParseEnum<TEnum>(b.Left) is { } l && ParseEnum<TEnum>(b.Right) is { } r
                ? (TEnum)Enum.ToObject(typeof(TEnum), Convert.ToInt64(l) | Convert.ToInt64(r))
                : null,
        ParenthesizedExpressionSyntax p => ParseEnum<TEnum>(p.Expression),
        _ => null
    };

    private static TEnum? TryParseName<TEnum>(string name) where TEnum : struct, Enum =>
        Enum.TryParse<TEnum>(name, out var value) && Enum.IsDefined(typeof(TEnum), value)
            ? value
            : null;
}

internal interface IAttributeValue
{
}

internal sealed record SemanticValue(TypedConstant Constant) : IAttributeValue
{
    public TypedConstant Constant { get; } = Constant;
}

internal sealed record SyntaxValue(ExpressionSyntax Expression) : IAttributeValue
{
    public ExpressionSyntax Expression { get; } = Expression;
}