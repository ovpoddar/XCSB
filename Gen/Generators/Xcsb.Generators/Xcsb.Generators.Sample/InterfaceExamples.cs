using System.Threading;
using System.Threading.Tasks;
using Xcsb.Generators;



namespace Xcsb.Generators.Sample;

[CheckedDeclaration]
[UncheckedDeclaration]
[AsyncDeclaration]
[BufferDeclaration]
public interface IMyService
{
    int Do();
    int Doing(int a);
}

public interface IService : IMyService, IMyServiceChecked, IMyServiceUnchecked, IMyServiceAsync
{
}

public interface IServiceBuffer : IMyServiceBuffer
{
}

public class Sercice : IService
{
    public int Do()
    {
        throw new System.NotImplementedException();
    }

    public int Doing(int a)
    {
        throw new System.NotImplementedException();
    }

    public void DoChecked()
    {
        throw new System.NotImplementedException();
    }

    public void DoingChecked(int a)
    {
        throw new System.NotImplementedException();
    }

    public void DoUnchecked()
    {
        throw new System.NotImplementedException();
    }

    public void DoingUnchecked(int a)
    {
        throw new System.NotImplementedException();
    }

    public Task<int> DoAsync(CancellationToken cancellationToken = default)
    {
        throw new System.NotImplementedException();
    }

    public Task<int> DoingAsync(int a, CancellationToken cancellationToken = default)
    {
        throw new System.NotImplementedException();
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