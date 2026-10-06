using Xcsb.Generators.Tests.Utils;
using Xunit;

namespace Xcsb.Generators.Tests;

public class DeclarationGeneratorTests
{
    private const string AttributeSource = @"
using System;

namespace Xcsb.Generators
{
    [AttributeUsage(validOn: AttributeTargets.Interface, AllowMultiple = false, Inherited = false)]
    public sealed class DeclarationAttribute : Attribute
    {
        public DeclarationKind Kind { get; }
        public DeclarationAttribute (DeclarationKind kind)
        {
            Kind = kind;
        }
        
        private DeclarationAttribute () {}
    }
}

namespace Xcsb.Generators
{
    [Flags]
    public enum DeclarationKind
    {
        Checked = 1,
        Unchecked = 2,
        Buffer = 4
    }

}

";

    [Fact]
    public void Text()
    {
        var source = """
                     using Xcsb.Generators;
                     namespace TestNamespace
                     {
                         [Declaration(DeclarationKind.Unchecked)]
                         public partial interface ITestService
                         {
                         }
                     }
                     """;
        var generatedSource =
            TestHelper.GenerateSource<DeclarationGeneratorBase>(source, AttributeSource, "Declaration.OfWar.g.cs");

        Assert.Contains("public class Foo ", generatedSource);
    }
}