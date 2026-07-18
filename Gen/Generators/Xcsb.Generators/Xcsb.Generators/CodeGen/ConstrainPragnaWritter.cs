using System;
using System.Collections.Generic;
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

        var methodText = node.SyntaxTree.GetText().ToString(node.FullSpan);
        var fullText = methodText.AsSpan();
        while (true)
        {
            var startIndex = fullText.IndexOf(startSequence.AsSpan());
            if (startIndex == -1) break;

            fullText = fullText.Slice(startIndex);
            var endIndex = fullText.IndexOf(endSequence.AsSpan());

            if (endIndex == -1) break;
            builder.AppendLine();
            builder.AppendLine(fullText.Slice(0, endIndex + endSequence.Length).ToString());
            fullText = fullText.Slice(endIndex + endSequence.Length);
            
            startIndex = fullText.IndexOf(startSequence.AsSpan());
            if (startIndex == -1) break;
        }
        
        if (appendTrailingSemicolon) builder.AppendLine(";");
    }

    internal static bool Contain(IMethodSymbol method, string type)
    {
        var syntaxRef = method.DeclaringSyntaxReferences.FirstOrDefault();
        if (syntaxRef?.GetSyntax() is not MethodDeclarationSyntax node || node.ConstraintClauses.Count == 0)
        {
            return false;
        }

        var methodText = node.SyntaxTree.GetText().ToString(node.FullSpan);
        var fullText = methodText.AsSpan();
        while (true)
        {
            var startIndex = fullText.IndexOf(startSequence.AsSpan());
            if (startIndex == -1)
            {
                break;
            }

            fullText = fullText.Slice(startIndex);
            var endIndex = fullText.IndexOf(endSequence.AsSpan());

            if (endIndex == -1) break;
            if (fullText.Slice(startSequence.Length, endIndex).Contains(type.AsSpan(), StringComparison.InvariantCultureIgnoreCase))
                return true;
            fullText = fullText.Slice(endIndex + endSequence.Length);
            
            startIndex = fullText.IndexOf(startSequence.AsSpan());
            if (startIndex == -1) break;
        }
        
        return false;
    }
}