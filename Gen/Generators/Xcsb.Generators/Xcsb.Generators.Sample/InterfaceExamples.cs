using Xcsb.Generators;

namespace Xcsb.Generators.Sample;

[CheckedDeclaration]
[UncheckedDeclaration]
public interface IMyService
{
    int Do();
}

public interface IService : IMyService, IMyServiceChecked, IMyServiceUnchecked
{
    
}

public class Sercice : IService
{
    public int Do()
    {
        throw new System.NotImplementedException();
    }

    public void DoChecked()
    {
        throw new System.NotImplementedException();
    }

    public void DoUnchecked()
    {
        throw new System.NotImplementedException();
    }
}