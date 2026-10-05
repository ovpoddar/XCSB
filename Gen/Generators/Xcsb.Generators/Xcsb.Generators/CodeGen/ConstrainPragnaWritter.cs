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
        var newline = methodText.Contains("\r\n", StringComparison.Ordinal) ? "\r\n" : "\n";
        var fullText = methodText.AsSpan();
        var wrotePragma = false;
        while (true)
        {
            var startIndex = fullText.IndexOf(startSequence.AsSpan());
            if (startIndex == -1) break;

            fullText = fullText.Slice(startIndex);
            var endIndex = fullText.IndexOf(endSequence.AsSpan());

            if (endIndex == -1) break;

            var pragmaBlock = NormalizeBlock(fullText.Slice(0, endIndex + endSequence.Length).ToString(), newline);
            builder.Append(newline);
            builder.Append(pragmaBlock);
            wrotePragma = true;
            fullText = fullText.Slice(endIndex + endSequence.Length);

            startIndex = fullText.IndexOf(startSequence.AsSpan());
            if (startIndex == -1) break;
        }

        if (appendTrailingSemicolon)
        {
            if (wrotePragma)
                builder.Append(newline);
            builder.Append(';').Append(newline);
        }
    }

    internal static bool Contain(IMethodSymbol method, string type)
    {
        var syntaxRef = method.DeclaringSyntaxReferences.FirstOrDefault();
        if (syntaxRef?.GetSyntax() is not MethodDeclarationSyntax node || node.ConstraintClauses.Count == 0)
        {
            return false;
        }

        var methodText = node.SyntaxTree.GetText().ToString(node.FullSpan);
        var newline = methodText.Contains("\r\n", StringComparison.Ordinal) ? "\r\n" : "\n";
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
            var pragmaBody = NormalizeBlock(fullText.Slice(startSequence.Length, endIndex - startSequence.Length).ToString(), newline);
            if (pragmaBody.Contains(type, StringComparison.InvariantCultureIgnoreCase))
                return true;
            fullText = fullText.Slice(endIndex + endSequence.Length);

            startIndex = fullText.IndexOf(startSequence.AsSpan());
            if (startIndex == -1) break;
        }

        return false;
    }

    private static string NormalizeBlock(string pragmaBlock, string newline)
    {
        var lines = pragmaBlock.Replace("\r\n", "\n").Replace('\r', '\n').Split('\n');
        var significantLines = lines.Where(static line => !string.IsNullOrWhiteSpace(line)).ToArray();
        if (significantLines.Length == 0) return string.Empty;

        var commonIndent = significantLines
            .Select(static line => line.TakeWhile(static ch => ch == ' ' || ch == '\t').Count())
            .Min();

        return string.Join(newline, lines.Select(line =>
        {
            if (string.IsNullOrWhiteSpace(line)) return string.Empty;
            return line.Substring(Math.Min(commonIndent, line.Length));
        }));
    }
}