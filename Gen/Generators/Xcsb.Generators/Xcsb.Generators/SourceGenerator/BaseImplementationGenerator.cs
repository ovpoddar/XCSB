using Microsoft.CodeAnalysis;
using Xcsb.Generators.CodeGen.ClassGeneration;

namespace Xcsb.Generators.SourceGenerator;

[Generator]
public class BaseImplementationGenerator : ImplementationGeneratorBase
{
    protected override string AttributeFullName => DefinitionAttributeCode.Base.FullName;
    protected override string AttributeSourceCode => DefinitionAttributeCode.Base.Source;
    protected override string GeneratedSuffix => DefinitionAttributeCode.Base.SuffixName;

    protected override string ClassGenerator(INamedTypeSymbol classSymbol, INamedTypeSymbol interfaceSymbol) =>
        ClassCodeGenerator.Generate(
            classSymbol, interfaceSymbol, string.Empty, (sb, method) =>
            {
                sb.AppendLine(" =>");
                sb.Append("            this.")
                    .Append(method.Name)
                    .Append("(");
                for (var i = 0; i < method.Parameters.Length; i++)
                {
                    var param = method.Parameters[i];
                    if (i > 0) sb.Append(", ");
                    sb.Append(param.Name);
                }

                sb.AppendLine(");");
            },
            followReturnType: true
        );
}