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
    
    
    public static DeclarationKind? GetDeclarationKind(IAttributeValue? symbol) =>
        symbol switch
        {
            SemanticValue semanticValue => (DeclarationKind?)(semanticValue.Constant.Value),
            SyntaxValue syntaxValue => ParseDeclarationKind(syntaxValue.Expression),
            _ => null
        };

    private static DeclarationKind? ParseDeclarationKind(ExpressionSyntax expr) => expr switch
    {
        MemberAccessExpressionSyntax m => TryParseKind(m.Name.Identifier.Text),
        IdentifierNameSyntax id => TryParseKind(id.Identifier.Text),
        BinaryExpressionSyntax b when b.IsKind(SyntaxKind.BitwiseOrExpression) =>
            ParseDeclarationKind(b.Left) is { } l && ParseDeclarationKind(b.Right) is { } r
                ? l | r
                : null,
        ParenthesizedExpressionSyntax p => ParseDeclarationKind(p.Expression),
        _ => null
    };

    private static DeclarationKind? TryParseKind(string name) =>
        Enum.TryParse<DeclarationKind>(name, out var kind) ? kind : null;
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