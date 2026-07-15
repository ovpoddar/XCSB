using Microsoft.CodeAnalysis;
using Xcsb.Generators.CodeGen;
using Xcsb.Generators.CodeGen.InterfaceGeneration;

namespace Xcsb.Generators.SourceGenerator;

[Generator]
public sealed class AsyncDeclarationGenerator : DeclarationGeneratorBase
{
    protected override string AttributeFullName => DeclarationAttributeCode.Async.FullName;
    protected override string AttributeSourceCode => DeclarationAttributeCode.Async.Source;
    protected override string GeneratedSuffix => DeclarationAttributeCode.Async.SuffixName;

    protected override string GenerateInterfaceImplementation(INamedTypeSymbol interfaceSymbol)
    {
        return InterfaceCodeGenerator.Generate(
            interfaceSymbol,
            interfaceSuffix: DeclarationAttributeCode.Async.SuffixName,
            methodSuffix: DeclarationAttributeCode.Async.SuffixName,
            returnTypeProvider: method => $"System.Threading.Tasks.Task<{method.ReturnType}>",
            addCancellationToken: true
        );
    }
}