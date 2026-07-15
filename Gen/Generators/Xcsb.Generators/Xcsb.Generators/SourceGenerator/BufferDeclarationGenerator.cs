using Microsoft.CodeAnalysis;
using Xcsb.Generators.CodeGen;

namespace Xcsb.Generators.SourceGenerator;

[Generator]
public class BufferDeclarationGenerator : DeclarationGeneratorBase
{
    protected override string AttributeFullName => DeclarationAttributeCode.Buffer.FullName;
    protected override string AttributeSourceCode => DeclarationAttributeCode.Buffer.Source;
    protected override string GeneratedSuffix => DeclarationAttributeCode.Buffer.SuffixName;

    protected override string GenerateInterfaceImplementation(INamedTypeSymbol interfaceSymbol)
    {
        return InterfaceCodeGenerator.Generate(
            interfaceSymbol,
            interfaceSuffix: DeclarationAttributeCode.Buffer.SuffixName,
            methodSuffix: string.Empty,
            returnTypeProvider: _ => "void"
        );
    }
}