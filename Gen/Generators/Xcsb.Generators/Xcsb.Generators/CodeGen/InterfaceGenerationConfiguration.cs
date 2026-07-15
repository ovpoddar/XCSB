namespace Xcsb.Generators.CodeGen;

internal static class DeclarationAttributeCode
{
    public static readonly AttributeInfo Async;
    public static readonly AttributeInfo Checked;
    public static readonly AttributeInfo Unchecked;

    static DeclarationAttributeCode()
    {
        Async = Create("Async");
        Checked = Create("Checked");
        Unchecked = Create("Unchecked");
    }

    private static AttributeInfo Create(string name) => new(
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

    internal readonly record struct AttributeInfo
    {
        public readonly string FullName { get; }
        public readonly string SuffixName { get; }
        public readonly string Source { get; }

        public AttributeInfo(string fullName, string suffixName, string source)
        {
            FullName = fullName;
            SuffixName = suffixName;
            Source = source;
        }
    }
}