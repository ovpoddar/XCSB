namespace Xcsb.Generators.CodeGen.InterfaceGeneration;

internal static class DeclarationAttributeCode
{
    public static readonly DeclarationConfiguration Async;
    public static readonly DeclarationConfiguration Checked;
    public static readonly DeclarationConfiguration Unchecked;
    public static readonly DeclarationConfiguration Buffer;

    static DeclarationAttributeCode()
    {
        Async = Create("Async");
        Checked = Create("Checked");
        Unchecked = Create("Unchecked");
        Buffer = Create("Buffer");
    }

    private static DeclarationConfiguration Create(string name) => new(
        fullName: $"Xcsb.Generators.{name}DeclarationAttribute",
        suffixName: name,
        source:
        $$"""
          using System;

          namespace Xcsb.Generators;

          [AttributeUsage(AttributeTargets.Interface)]
          public class {{name}}DeclarationAttribute : Attribute
          {

          }
          """
    );

    internal readonly record struct DeclarationConfiguration
    {
        public readonly string FullName;
        public readonly string SuffixName;
        public readonly string Source;

        public DeclarationConfiguration(string fullName, string suffixName, string source)
        {
            FullName = fullName;
            SuffixName = suffixName;
            Source = source;
        }
    }
}