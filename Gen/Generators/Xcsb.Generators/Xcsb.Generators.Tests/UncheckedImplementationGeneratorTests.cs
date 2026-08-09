using Xcsb.Generators.SourceGenerator;
using Xcsb.Generators.Tests.Utils;
using Xunit;

namespace Xcsb.Generators.Tests;

public class UncheckedImplementationGeneratorTests
{
    private const string AttributeSource = @"
using System;

namespace Xcsb.Generators
{
    [AttributeUsage(AttributeTargets.Class)]
    public class UncheckedImplementationAttribute : Attribute
    {
        public Type Name { get; }
        public UncheckedImplementationAttribute(Type name)
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

                [UncheckedImplementation(typeof(IMyService))]
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
        var generatedSource = TestHelper.GenerateSource<UncheckedImplementationGenerator>(source,
            AttributeSource,
            "Service.Unchecked.g.cs");
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

    [UncheckedImplementation(typeof(IMyService))]
    public partial class Service
    {{
    }}
}}";

        TestHelper.AssertDiagnostic<UncheckedImplementationGenerator>(source, AttributeSource, "XCSBGEN002");
    }
}
