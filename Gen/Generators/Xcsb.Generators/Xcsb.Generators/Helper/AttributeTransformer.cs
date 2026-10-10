using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Xcsb.Generators.Helper;

internal static class AttributeTransformer
{
    public static IEnumerable<AttributeValue>? GetAllAttributeValues(AttributeData? attr)
    {
        if (attr is null)
            yield break;

        var parameters = attr.AttributeConstructor?.Parameters ?? default;

        for (int i = 0; i < attr.ConstructorArguments.Length; i++)
        {
            string name = i < parameters.Length ? parameters[i].Name : $"arg{i}";
            TypedConstant arg = attr.ConstructorArguments[i];

            yield return new AttributeValue(name, Unwrap(arg), arg.Type?.ToDisplayString());
        }

        foreach (var named in attr.NamedArguments)
        {
            yield return new AttributeValue(named.Key, Unwrap(named.Value), named.Value.Type?.ToDisplayString());
        }
    }

    private static object? Unwrap(TypedConstant c) => c.Kind switch
    {
        TypedConstantKind.Error => null,
        TypedConstantKind.Array => c.Values.Select(Unwrap).ToArray(),
        TypedConstantKind.Type => (c.Value as ITypeSymbol)?.ToDisplayString(),
        _ => c.Value
    };
}

public readonly record struct AttributeValue
{
    public readonly string Name;
    public readonly object? Value;
    public readonly string? TypeName;

    public AttributeValue(string name, object? value, string? typeName)
    {
        Name = name;
        Value = value;
        TypeName = typeName;
    }
}