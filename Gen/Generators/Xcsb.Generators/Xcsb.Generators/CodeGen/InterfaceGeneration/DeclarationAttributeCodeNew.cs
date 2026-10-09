using System.Text;
using Microsoft.CodeAnalysis.Text;

namespace Xcsb.Generators.CodeGen.InterfaceGeneration;

internal static class DeclarationAttributeCodeNew
{
    private const string Type = "Declaration";
    public const string AttributeName = $"Xcsb.Generators.{Type}Attribute";
    public const string FileName = $"Xcsb.Generators.{Type}.cs";

    public static readonly SourceText Context = SourceText.From(
        $$"""
          namespace Xcsb.Generators
          {
              [global::System.Flags]
              public enum {{Type}}Kind
              {
                  Checked = 1,
                  Unchecked = 2,
                  Buffer = 4
              }
              
              [global::System.AttributeUsage(validOn: global::System.AttributeTargets.Interface, AllowMultiple = false, Inherited = false)]
              public sealed class {{Type}}Attribute : global::System.Attribute
              {
                  public {{Type}}Attribute (DeclarationKind kind) { }
                  private {{Type}}Attribute () {}
              }
          }
          """, Encoding.UTF8);
}