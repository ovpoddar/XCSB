using Microsoft.CodeAnalysis;
using Xcsb.Generators.CodeGen.AsyncDeclarationGeneration;

namespace Xcsb.Generators.SourceGenerator;

[Generator]
public class AsyncDeclarationGenerator : DeclarationGeneratorBase
{
    protected override string AttributeFullName => AsyncDeclarationAttributeCode.AttributeFullName;
    protected override string AttributeSourceCode => AsyncDeclarationAttributeCode.AsyncDeclarationAttribute;
    protected override string GeneratedFileSuffix => "Async";
    protected override string GenerateInterfaceImplementation(INamedTypeSymbol interfaceSymbol)
    {
        return AsyncDeclarationImplCode.GenerateInterfaceImplementation(interfaceSymbol);
    }
}