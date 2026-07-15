namespace Xcsb.Generators.CodeGen.ClassGeneration;

internal static class DefinitionAttributeCode
{
    // public static readonly DeclarationConfiguration Async;
    public static readonly DeclarationConfiguration Checked;
    // public static readonly DeclarationConfiguration Unchecked;
    // public static readonly DeclarationConfiguration Buffer;
    
    
    static DefinitionAttributeCode()
    {
        Checked = Create("Checked");
    }
    
    private static DeclarationConfiguration Create(string name) => new(
        fullName: $"Xcsb.Generators.{name}ImplementationAttribute",
        source:
        $$"""
          using System;

          namespace Xcsb.Generators;

          [AttributeUsage(AttributeTargets.Class)]
          public class {{name}}ImplementationAttribute : Attribute
          {
                public Type Name { get; }
                public {{name}}ImplementationAttribute(Type name) 
                {
                    Name = name;
                }
          }
          """
    );

    internal readonly record struct DeclarationConfiguration
    {
        public readonly string FullName;
        public readonly string Source;

        public DeclarationConfiguration(string fullName, string source)
        {
            FullName = fullName;
            Source = source;
        }
    }
}