using Xcsb.Generators.SourceGenerator;
using Xcsb.Generators.Tests.Utils;
using Xunit;

namespace Xcsb.Generators.Tests;

public class CheckedImplementationTests
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
}