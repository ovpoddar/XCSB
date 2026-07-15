using Microsoft.CodeAnalysis;
using Xcsb.Generators.CodeGen;

namespace Xcsb.Generators.SourceGenerator;

[Generator]
public class UncheckedDeclarationGenerator : DeclarationGeneratorBase
{
    protected override string AttributeFullName => DeclarationAttributeCode.Unchecked.FullName;
    protected override string AttributeSourceCode => DeclarationAttributeCode.Unchecked.Source;
    protected override string GeneratedSuffix => DeclarationAttributeCode.Unchecked.SuffixName;

    protected override string GenerateInterfaceImplementation(INamedTypeSymbol interfaceSymbol)
    {
        return InterfaceCodeGenerator.Generate(
            interfaceSymbol,
            interfaceSuffix: DeclarationAttributeCode.Unchecked.SuffixName,
            methodSuffix: DeclarationAttributeCode.Unchecked.SuffixName,
            returnTypeProvider: _ => "void"
        );
    }
}