using Microsoft.CodeAnalysis;
using Xcsb.Generators.CodeGen.CheckedDeclarationGeneration;

namespace Xcsb.Generators.SourceGenerator;

[Generator]
public class CheckedDeclarationGenerator : DeclarationGeneratorBase
{
    protected override string AttributeFullName => CheckedDeclarationAttributeCode.AttributeFullName;
    protected override string AttributeSourceCode => CheckedDeclarationAttributeCode.CheckedDeclarationAttribute;
    protected override string GeneratedFileSuffix => "Checked";

    protected override string GenerateInterfaceImplementation(INamedTypeSymbol interfaceSymbol)
    {
        return CheckedDeclarationImplCode.GenerateInterfaceImplementation(interfaceSymbol);
    }
}