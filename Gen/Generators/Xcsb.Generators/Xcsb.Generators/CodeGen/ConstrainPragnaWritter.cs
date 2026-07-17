using System;
using System.Text;
using Microsoft.CodeAnalysis;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Xcsb.Generators.CodeGen;

internal static class ConstrainPragmaWriter
{
    private const string startSequence = "#if";
    private const string endSequence = "#endif";

    internal static void Write(StringBuilder builder, IMethodSymbol method, bool appendTrailingSemicolon)
    {
        var syntaxRef = method.DeclaringSyntaxReferences.FirstOrDefault();
        if (syntaxRef?.GetSyntax() is not MethodDeclarationSyntax node || node.ConstraintClauses.Count == 0)
        {
            if (appendTrailingSemicolon) builder.AppendLine(";");
            return;
        }

        var methodText = node.SyntaxTree.GetText().ToString(node.FullSpan).AsSpan();
        var startIndex = methodText.IndexOf(startSequence.AsSpan());
        if (startIndex == -1)
        {
            if (appendTrailingSemicolon) builder.AppendLine(";");
            return;
        }

        var remaining = methodText.Slice(startIndex);
        var endIndex = remaining.IndexOf(endSequence.AsSpan());

        if (endIndex != -1)
        {
            builder.AppendLine();
            builder.AppendLine(remaining.Slice(0, endIndex + endSequence.Length).ToString());
        }
        
        if (appendTrailingSemicolon) builder.AppendLine(";");
    }
}