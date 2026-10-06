using Xcsb.Generators.Tests.Utils;
using Xunit;

namespace Xcsb.Generators.Tests;

public class DeclarationGeneratorTests
{
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
            TestHelper.GenerateSource<DeclarationGeneratorBase>(
                source, string.Empty, "Declaration.OfWar.g.cs", validateOutputCompilation: true);

        Assert.Contains("public class Foo ", generatedSource);
    }
}