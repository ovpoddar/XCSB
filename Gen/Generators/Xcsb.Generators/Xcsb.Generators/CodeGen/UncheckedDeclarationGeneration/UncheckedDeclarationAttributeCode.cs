namespace Xcsb.Generators.CodeGen.UncheckedDeclarationGeneration;

public static class UncheckedDeclarationAttributeCode
{
    public const string AttributeFullName = "Xcsb.Generators.UncheckedDeclarationAttribute";
    public const string UncheckedDeclarationAttribute =
        """
        using System;

        namespace Xcsb.Generators;

        [AttributeUsage(AttributeTargets.Interface)]
        public class UncheckedDeclarationAttribute : Attribute
        {
            
        }
        """;
}