using System;
using System.Threading;
using System.Threading.Tasks;
using Xcsb.Generators;
namespace Xcsb.Generators.Sample;
[CheckedDeclaration]
[UncheckedDeclaration]
[BufferDeclaration]
public interface IMyService
{
    int Do();
    int Doing(int a);
}

public interface IService : IMyService, IMyServiceChecked, IMyServiceUnchecked
{
}

public interface IServiceBuffer : IMyServiceBuffer
{
}

[CheckedImplementation(typeof(IMyService))]
[UncheckedImplementation(typeof(IMyService))]
[BaseImplementation(typeof(IMyService))]
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
        public int Sequence { get; set; }
    }

    public class TestService
    {
        public void SkipErrorForSequence(int sequence, bool isError)
        {

        }
    }
}

public class ServiceBuffer : IServiceBuffer
{
    public void Do()
    {
        throw new System.NotImplementedException();
    }

    public void Doing(int a)
    {
        throw new System.NotImplementedException();
    }
}
