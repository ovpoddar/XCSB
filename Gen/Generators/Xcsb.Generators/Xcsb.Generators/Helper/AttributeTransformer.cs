using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Xcsb.Generators.Helper;

internal static class AttributeTransformer
{
    public static IEnumerable<object?> GetAllAttributeValues(AttributeData? attr)
    {
        if (attr is null)
            yield break;

        var parameters = attr.AttributeConstructor?.Parameters ?? default;

        for (var i = 0; i < attr.ConstructorArguments.Length; i++)
        {
            var name = i < parameters.Length ? parameters[i].Name : $"arg{i}";
            var arg = attr.ConstructorArguments[i];

            yield return Unwrap(arg);
        }

        foreach (var named in attr.NamedArguments)
        {
            yield return Unwrap(named.Value);
        }

        var syntax = attr.ApplicationSyntaxReference?.GetSyntax() as AttributeSyntax;
        var args = syntax?.ArgumentList?.Arguments;
        if (args is null)
            yield break;

        var position = 0;
        foreach (var arg in args.Value)
        {
            var name =
                arg.NameEquals?.Name.Identifier.Text ??
                arg.NameColon?.Name.Identifier.Text ??
                $"arg{position++}";

            yield return ParseExpression(arg.Expression);
        }
    }


    private static object? ParseExpression(ExpressionSyntax expr)
    {
        return expr switch
        {
            LiteralExpressionSyntax lit => lit.Token.Value,
            ParenthesizedExpressionSyntax p => ParseExpression(p.Expression),
            PrefixUnaryExpressionSyntax u when u.IsKind(SyntaxKind.UnaryMinusExpression) =>
                ParseExpression(u.Operand) is int n ? -n : null,
            BinaryExpressionSyntax b when b.IsKind(SyntaxKind.BitwiseOrExpression) =>
                ParseExpression(b.Left) is int l && ParseExpression(b.Right) is int r ? l | r : null,
            CastExpressionSyntax c => ParseExpression(c.Expression),
            TypeOfExpressionSyntax t => t.Type.ToString(),
            MemberAccessExpressionSyntax m => ParseEnumName(m.Name.Identifier.Text),
            IdentifierNameSyntax id => ParseEnumName(id.Identifier.Text),
            _ => expr.ToString()
        };
    }

    private static object ParseEnumName(string name) => name switch
    {
        "Checked"   => (int)DeclarationKind.Checked,
        "Unchecked" => (int)DeclarationKind.Unchecked,
        "Buffer"    => (int)DeclarationKind.Buffer,
        _ when int.TryParse(name, out var v) => v,
        _ => name
    };


    private static object? Unwrap(TypedConstant c) => c.Kind switch
    {
        TypedConstantKind.Error => null,
        TypedConstantKind.Array => c.Values.Select(Unwrap).ToArray(),
        TypedConstantKind.Type => (c.Value as ITypeSymbol)?.ToDisplayString(),
        _ => c.Value
    };
}
