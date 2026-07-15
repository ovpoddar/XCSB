namespace Xcsb.Generators.CodeGen;

internal static class DeclarationAttributeCode
{
    public static readonly AttributeInfo Async;
    public static readonly AttributeInfo Checked;
    public static readonly AttributeInfo Unchecked;
    public static readonly AttributeInfo Buffer;

    static DeclarationAttributeCode()
    {
        Async = Create("Async");
        Checked = Create("Checked");
        Unchecked = Create("Unchecked");
        Buffer = Create("Buffer");
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
        public readonly string FullName;
        public readonly string SuffixName;
        public readonly string Source;

        public AttributeInfo(string fullName, string suffixName, string source)
        {
            FullName = fullName;
            SuffixName = suffixName;
            Source = source;
        }
    }
}