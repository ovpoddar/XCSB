using Xcsb.Generators.SourceGenerator;
using Xcsb.Generators.Tests.Utils;
using Xunit;

namespace Xcsb.Generators.Tests;

public class CheckedImplementationGeneratorTests
{
    private const string AttributeSource = @"
using System;

namespace Xcsb.Generators
{
    [AttributeUsage(AttributeTargets.Class)]
    public class CheckedImplementationAttribute : Attribute
    {
        public Type Name { get; }
        public CheckedImplementationAttribute(Type name)
        {
            Name = name;
        }
    }
}";

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

                [CheckedImplementation(typeof(IMyService))]
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
        var generatedSource = TestHelper.GenerateSource<CheckedImplementationGenerator>(source,
            AttributeSource,
            "Service.Checked.g.cs");
        Assert.Contains("void DoChecked()", generatedSource);
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

    [CheckedImplementation(typeof(IMyService))]
    public partial class Service
    {{
    }}
}}";

        TestHelper.AssertDiagnostic<CheckedImplementationGenerator>(source, AttributeSource, "XCSBGEN002");
    }
}
