using System.Text;
using Microsoft.CodeAnalysis.Text;
using Xcsb.Generators.Helper;

namespace Xcsb.Generators.CodeGen.InterfaceGeneration;

internal static class DeclarationAttributeCodeNew
{
    public const string Type = "Declaration";
    public const string AttributeName = $"Xcsb.Generators.{Type}Attribute";
    public const string FileName = $"Xcsb.Generators.{Type}.cs";

    public static readonly SourceText Context =
        SourceText.From(SourceToString.GetSource(typeof(DeclarationKind), typeof(DeclarationAttribute)), Encoding.UTF8);
}