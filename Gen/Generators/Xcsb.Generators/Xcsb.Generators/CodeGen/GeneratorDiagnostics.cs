using System.Linq;
using Microsoft.CodeAnalysis;

namespace Xcsb.Generators.CodeGen;

internal static class GeneratorDiagnostics
{
    public static readonly DiagnosticDescriptor DeclarationInvalidReturnType = new(
        id: "XCSBGEN001",
        title: "Unsupported return type for source-generated declaration",
        messageFormat: "Method '{0}' on interface '{1}' returns '{2}', which is not supported by '[{3}]'; " +
                        "ordinary methods must return a non-void, non-Task, non-ValueTask value",
        category: "Xcsb.Generators.Usage",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor ImplementationInvalidReturnType = new(
        id: "XCSBGEN002",
        title: "Unsupported return type on interface referenced by implementation attribute",
        messageFormat: "Method '{0}' on interface '{1}' returns '{2}', which is not supported by " +
                        "'[{3}(typeof({1}))]' on class '{4}'; ordinary methods must return a non-void, non-Task, non-ValueTask value",
        category: "Xcsb.Generators.Usage",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    internal static IMethodSymbol? FindFirstOffendingMethod(INamedTypeSymbol typeSymbol)
    {
        foreach (var method in typeSymbol.GetMembers().OfType<IMethodSymbol>())
        {
            if (method.MethodKind != MethodKind.Ordinary) continue;
            if (IsUnsupportedReturnType(method.ReturnType)) return method;
        }

        return null;
    }

    private static bool IsUnsupportedReturnType(ITypeSymbol returnType)
    {
        if (returnType.SpecialType == SpecialType.System_Void) return true;

        // Matches Task/Task<T> and ValueTask/ValueTask<T> — arity doesn't affect ITypeSymbol.Name,
        // so this deliberately does not distinguish the open vs. constructed generic form.
        var ns = returnType.ContainingNamespace?.ToDisplayString();
        if (ns != "System.Threading.Tasks") return false;

        return returnType.Name is "Task" or "ValueTask";
    }
}
