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

                [CheckedImplementation(typeof(IMyService))]
                public partial class Service
                {
                    private readonly TestService _socketAccessor;
                    
                    
                    private Cookie DoStaffBase()
                    {
                        return new Cookie();
                    }
                }
                
                public class Cookie
                {
                    public int Sequence { get; set; }
                }
                
                public class TestService
                {
                    public void SkipErrorForSequence(int sequence, bool isError)
                    {
                            
                    }
                }
            }
            """;
        var generatedSource = TestHelper.GenerateSource<ImplementationGeneratorBase>(source,
            AttributeSource,
            "Service.Checked.g.cs");
        Assert.NotEmpty(generatedSource);
    }
}