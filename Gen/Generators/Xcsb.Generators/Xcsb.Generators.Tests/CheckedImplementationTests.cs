using Xcsb.Generators.Tests.Utils;
using Xunit;

namespace Xcsb.Generators.Tests;

public class CheckedImplementationTests
{
    private const string AttributeSource = @"
namespace Xcsb.Generators
{
    [global::System.Flags]
    public enum ImplementationKind
    {
    Checked = 1,
    Unchecked = 2,
    Base = 4,
    }

    [global::System.AttributeUsage(global::System.AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ImplementationAttribute : global::System.Attribute
    {
        public ImplementationAttribute(global::Xcsb.Generators.ImplementationKind kind, global::System.Type target) {}

        private ImplementationAttribute() {}
    }
}
";

    [Fact]
    public void Test()
    {
        var source =
            """
            using Xcsb.Generators;

            namespace TestNamespace
            {
                public interface IMyService
                {
                    int DoStaff();
                }
                
                [Implementation(ImplementationKind.Base, typeof(IMyService))]
                public partial class Service
                {
                    
                }
                
            }
            """;
        var generatedSource = TestHelper.GenerateSource<ImplementationGenerator>(source,
            AttributeSource,
            "Implementation.g.cs");
        Assert.NotEmpty(generatedSource);
    }
}