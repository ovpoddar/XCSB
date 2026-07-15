using Microsoft.CodeAnalysis;
using Xcsb.Generators.CodeGen;
using Xcsb.Generators.CodeGen.InterfaceGeneration;

namespace Xcsb.Generators.SourceGenerator;

[Generator]
public sealed class CheckedDeclarationGenerator : DeclarationGeneratorBase
{
    protected override string AttributeFullName => DeclarationAttributeCode.Checked.FullName;
    protected override string AttributeSourceCode => DeclarationAttributeCode.Checked.Source;
    protected override string GeneratedSuffix => DeclarationAttributeCode.Checked.SuffixName;

    protected override string GenerateInterfaceImplementation(INamedTypeSymbol interfaceSymbol)
    {
        return InterfaceCodeGenerator.Generate(
            interfaceSymbol,
            interfaceSuffix: DeclarationAttributeCode.Checked.SuffixName,
            methodSuffix: DeclarationAttributeCode.Checked.SuffixName,
            returnTypeProvider: _ => "void"
        );
    }
}