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
    
    internal sealed record SemanticValue(TypedConstant Constant) : IAttributeValue
    {
        public TypedConstant Constant { get; } = Constant;

        public AttributeValueSource Source { get; } = AttributeValueSource.Semantic;
    }

    internal sealed record SyntaxValue(ExpressionSyntax Expression) : IAttributeValue
    {
        public ExpressionSyntax Expression { get; } = Expression;
        public AttributeValueSource Source => AttributeValueSource.Syntax;
    }
}

internal interface IAttributeValue
{
    AttributeValueSource Source { get; }
}

internal enum AttributeValueSource
{
    Semantic,
    Syntax
}