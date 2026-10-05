using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Xcsb.Generators;

[Generator]
public class Declaration : IIncrementalGenerator
{
    private const string DeclarationName = "DeclarationAttribute";
    private const string DeclarationSource =
$$"""
using System;

namespace Xcsb.Generators
{
    [AttributeUsage(validOn: AttributeTargets.Interface, AllowMultiple = false, Inherited = false)]
    public sealed class {{DeclarationName}} : Attribute
    {
        public DeclarationKind Kind { get; }
        public {{DeclarationName}} (DeclarationKind kind)
        {
            Kind = kind;
        }
        
        private {{DeclarationName}} () {}
    }
}

""";

    private const string DeclarationKindSource =
"""
using System;

namespace Xcsb.Generators
{
    [Flags]
    public enum DeclarationKind
    {
        Checked = 0,
        Unchecked = 1,
        Buffer = 2
    }

}

""";

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        
        context.RegisterPostInitializationOutput(ctx =>
        {
            ctx.AddSource(
                "Declaration.g.cs",
                SourceText.From(DeclarationSource, Encoding.UTF8));
            ctx.AddSource(
                "DeclarationKind.g.cs",
                SourceText.From(DeclarationKindSource, Encoding.UTF8)
                );
        });

        var provider = context.SyntaxProvider.ForAttributeWithMetadataName(
            DeclarationName,
            predicate: static (node, _) => node is InterfaceDeclarationSyntax,
            // transform: (ctx, _) => (attributeData: (AttributeData?)ctx.Attributes.FirstOrDefault(), targetSymbol: ctx.TargetSymbol.Locations.FirstOrDefault()))
            // .Where(a => a.attributeData is not null);
            transform: (ctx, _) => ctx.Attributes.FirstOrDefault());
        
        
        context.RegisterSourceOutput(provider, (ctx, result) =>
        {
            
            // var val = string.Join(", ",
            //     result.attributeData!.NamedArguments.Select(a => $"{a.Key} = {a.Value}"));
            var val = string.Join(", ",
                result!.NamedArguments.Select(a => $"{a.Key} = {a.Value}"));
            ctx.AddSource(
                "Declaration.OfWar.g.cs",
                SourceText.From($@"
public class Foo 
{{
    // {val}
    
}}
", Encoding.UTF8));
        });
    }
}