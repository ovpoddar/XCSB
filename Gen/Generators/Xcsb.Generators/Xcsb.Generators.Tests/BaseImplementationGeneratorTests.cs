using Xcsb.Generators.Tests.Utils;
using Xunit;

namespace Xcsb.Generators.Tests;

public class BaseImplementationGeneratorTests
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
    public void Generator_ShouldGenerateClass_WhenClassHasAttribute()
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

    [Theory]
    [InlineData("void")]
    [InlineData("System.Threading.Tasks.Task")]
    [InlineData("System.Threading.Tasks.Task<int>")]
    [InlineData("System.Threading.Tasks.ValueTask")]
    [InlineData("System.Threading.Tasks.ValueTask<int>")]
    public void Generator_ShouldReportDiagnostic_WhenInterfaceMethodReturnsVoidOrTaskLike(string returnType)
    {
        var source = $@"
using Xcsb.Generators;

namespace TestNamespace
{{
    public interface IMyService
    {{
        {returnType} DoBad();
    }}

    
    [Implementation(ImplementationKind.Base, typeof(IMyService))]
    public partial class Service
    {{
    }}
}}";

        TestHelper.AssertDiagnostic<ImplementationGenerator>(source, AttributeSource, "XCSBGEN002");
    }
}
