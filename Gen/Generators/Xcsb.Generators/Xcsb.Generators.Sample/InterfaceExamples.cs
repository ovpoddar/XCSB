using System;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Xcsb.Generators;

namespace Xcsb.Generators.Sample;

[Declaration(DeclarationKind.Checked | DeclarationKind.Unchecked | DeclarationKind.Async)]
public interface IMyService
{
    int Do();
    int Doing(int a);
}

public interface IService : IMyService, IMyServiceChecked, IMyServiceUnchecked
{
}

[Implementation(ImplementationKind.Checked, typeof(IMyService))]
[Implementation(ImplementationKind.Base, typeof(IMyService))]
[Implementation(ImplementationKind.Unchecked, typeof(IMyService))]
[JsonSerializable(typeof(Service))]
public partial class Service : IService
{
    private readonly TestService _socketAccessor;

    public Task<int> DoAsync(CancellationToken cancellationToken = default)
    {
        throw new System.NotImplementedException();
    }

    public Task<int> DoingAsync(int a, CancellationToken cancellationToken = default)
    {
        throw new System.NotImplementedException();
    }


    private Cookie DoBase()
    {
        return new Cookie();
    }

    private Cookie DoingBase(int a)
    {
        return new Cookie();
    }

    public class Cookie
    {
        public int Id { get; set; }
    }

    public class TestService
    {
        public void SkipErrorForSequence(int sequence, bool isError)
        {
        }
    }
}

[Implementation(ImplementationKind.Checked | ImplementationKind.Unchecked | ImplementationKind.Base,
    typeof(IMyService))]
public partial class MyClass
{
}