namespace Xcsb.Generators.CodeGen.CheckedDeclarationGeneration;

public static class CheckedDeclarationAttributeCode
{
    public const string AttributeFullName = "Xcsb.Generators.CheckedDeclarationAttribute";
    public const string CheckedDeclarationAttribute =
        """
        using System;

        namespace Xcsb.Generators;

        [AttributeUsage(AttributeTargets.Interface)]
        public class CheckedDeclarationAttribute : Attribute
        {
            
        }
        """;
}