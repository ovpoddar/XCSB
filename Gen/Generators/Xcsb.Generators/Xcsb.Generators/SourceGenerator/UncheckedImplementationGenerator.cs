using Microsoft.CodeAnalysis;
using Xcsb.Generators.CodeGen.ClassGeneration;

namespace Xcsb.Generators.SourceGenerator;

[Generator]
public sealed class UncheckedImplementationGenerator : ImplementationGeneratorBase
{
    protected override string AttributeFullName => DefinitionAttributeCode.Unchecked.FullName;
    protected override string AttributeSourceCode => DefinitionAttributeCode.Unchecked.Source;
    protected override string GeneratedSuffix => DefinitionAttributeCode.Unchecked.SuffixName;
    protected override string ClassGenerator(INamedTypeSymbol classSymbol, INamedTypeSymbol interfaceSymbol) =>
        ClassCodeGenerator.Generate(classSymbol, interfaceSymbol, GeneratedSuffix, (sb, method) =>
        {
            sb.AppendLine();
            sb.AppendLine("        {");
            sb.Append("            var cookie = this.")
                .Append(method.Name)
                .Append("Base(");

            for (var i = 0; i < method.Parameters.Length; i++)
            {
                var param = method.Parameters[i];
                if (i > 0) sb.Append(", ");
                sb.Append(param.Name);
            }

            sb.AppendLine(");");
            sb.AppendLine("            _socketAccessor.SkipErrorForSequence(cookie.Sequence, false);");
            sb.AppendLine("        }");
        });

}