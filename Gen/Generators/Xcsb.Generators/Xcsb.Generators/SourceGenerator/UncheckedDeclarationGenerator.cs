using Microsoft.CodeAnalysis;
using Xcsb.Generators.CodeGen.UncheckedDeclarationGeneration;

namespace Xcsb.Generators.SourceGenerator;

[Generator]
public class UncheckedDeclarationGenerator : DeclarationGeneratorBase
{
    protected override string AttributeFullName => UncheckedDeclarationAttributeCode.AttributeFullName;
    protected override string AttributeSourceCode => UncheckedDeclarationAttributeCode.UncheckedDeclarationAttribute;
    protected override string GeneratedFileSuffix => "Unchecked";

    protected override string GenerateInterfaceImplementation(INamedTypeSymbol interfaceSymbol)
    {
        return UncheckedDeclarationImplCode.GenerateInterfaceImplementation(interfaceSymbol);
    }
}