namespace Xcsb.Generators.CodeGen.AsyncDeclarationGeneration;

public static class AsyncDeclarationAttributeCode
{
    public const string AttributeFullName = "Xcsb.Generators.AsyncDeclarationAttribute";
    public const string AsyncDeclarationAttribute =
        """
        using System;

        namespace Xcsb.Generators;

        [AttributeUsage(AttributeTargets.Interface)]
        public class AsyncDeclarationAttribute : Attribute
        {
            
        }
        """;
}