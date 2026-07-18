using Microsoft.CodeAnalysis;
using Xcsb.Generators.CodeGen.ClassGeneration;

namespace Xcsb.Generators.SourceGenerator;

[Generator]
public sealed class CheckedImplementationGenerator : ImplementationGeneratorBase
{
    protected override string AttributeFullName => DefinitionAttributeCode.Checked.FullName;
    protected override string AttributeSourceCode => DefinitionAttributeCode.Checked.Source;
    protected override string GeneratedSuffix => DefinitionAttributeCode.Checked.SuffixName;
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
            sb.AppendLine("            _socketAccessor.SkipErrorForSequence(cookie.Id, true);");
            sb.AppendLine("        }");
        });

}