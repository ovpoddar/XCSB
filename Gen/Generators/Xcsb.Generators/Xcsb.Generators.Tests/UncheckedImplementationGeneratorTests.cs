using Xcsb.Generators.Tests.Utils;
using Xunit;

namespace Xcsb.Generators.Tests;

public class UncheckedImplementationGeneratorTests
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
                    int Do();
                }

                [Implementation(ImplementationKind.Unchecked, typeof(IMyService))]
                public partial class Service
                {
                    private Cookie DoBase()
                    {
                        return new Cookie();
                    }

                    public class Cookie
                    {
                        public int Sequence { get; set; }
                    }

                    private readonly TestService _socketAccessor = new TestService();

                    public class TestService
                    {
                        public void SkipErrorForSequence(int sequence, bool isError)
                        {
                        }
                    }
                }
            }
            """;
        var generatedSource = TestHelper.GenerateSource<ImplementationGeneratorBaseNew>(source,
            AttributeSource,
            "Implementation.g.cs");
        Assert.Contains("void DoUnchecked()", generatedSource);
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

    [Implementation(ImplementationKind.Unchecked, typeof(IMyService))]
    public partial class Service
    {{
    }}
}}";

        TestHelper.AssertDiagnostic<ImplementationGeneratorBaseNew>(source, AttributeSource, "XCSBGEN002");
    }
}
