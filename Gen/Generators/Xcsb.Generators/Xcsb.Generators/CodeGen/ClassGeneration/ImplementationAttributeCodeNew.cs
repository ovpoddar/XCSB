using System.Text;
using Microsoft.CodeAnalysis.Text;
using Xcsb.Generators;
using Xcsb.Generators.Helper;

internal static class ImplementationAttributeCodeNew
{
    public const string Type = "Implementation";
    public const string AttributeName = $"Xcsb.Generators.{Type}Attribute";
    public const string FileName = $"Xcsb.Generators.{Type}.cs";
    
    public static readonly SourceText Context =
        SourceText.From(SourceToString.GetSource(typeof(ImplementationKind), typeof(ImplementationAttribute)), Encoding.UTF8);
}