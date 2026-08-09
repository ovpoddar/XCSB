using Xcsb.Generators.SourceGenerator;
using Xcsb.Generators.Tests.Utils;
using Xunit;

namespace Xcsb.Generators.Tests;

public class BaseImplementationGeneratorTests
{
    private const string AttributeSource = @"
using System;

namespace Xcsb.Generators
{
    [AttributeUsage(AttributeTargets.Class)]
    public class BaseImplementationAttribute : Attribute
    {
        public Type Name { get; }
        public BaseImplementationAttribute(Type name)
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
                    int DoStaff();
                }

                [BaseImplementation(typeof(IMyService))]
                public partial class Service
                {

                }

            }
            """;
        var generatedSource = TestHelper.GenerateSource<BaseImplementationGenerator>(source,
            AttributeSource,
            "Service.Base.g.cs");
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

    [BaseImplementation(typeof(IMyService))]
    public partial class Service
    {{
    }}
}}";

        TestHelper.AssertDiagnostic<BaseImplementationGenerator>(source, AttributeSource, "XCSBGEN002");
    }
}
