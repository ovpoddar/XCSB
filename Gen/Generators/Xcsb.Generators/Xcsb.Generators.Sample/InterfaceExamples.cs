using System;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using Xcsb.Generators;
namespace Xcsb.Generators.Sample;

public struct Cookie
{
    public int Id { get; set; }
    public bool IsChecked { get; set; }
}


[Declaration(DeclarationKind.Checked | DeclarationKind.Unchecked)]
interface IItem1
{
    Cookie GetCookie();
}

[Declaration(DeclarationKind.Async)]
interface IItem2
{
    int GetInt();
    Dispo<foo> GetFoo();
}

[Implementation(ImplementationKind.Checked | ImplementationKind.Unchecked | ImplementationKind.Base, typeof(IItem1))]
[Implementation(ImplementationKind.Async | ImplementationKind.Return, typeof(IItem2))]
partial class Item1 : IItem1
{
    private Requiremnt _socketAccessor;
    
    private Cookie GetCookieBase() => default;
    
    private int GetIntOverride() => default;
    
    private Dispo<foo> GetFooOverride() => default;
    
    private ValueTask<int> GetIntOverrideAsync() => default;

    private ValueTask<Dispo<foo>> GetFooOverrideAsync() => default;
}

class Requiremnt
{
    public void SkipErrorForSequence(int i, bool b)
    {
    }
}

struct foo : ICloneable, IAsyncResult
{
    public object? AsyncState { get; }
    public WaitHandle AsyncWaitHandle { get; }
    public bool CompletedSynchronously { get; }
    public bool IsCompleted { get; }
    public object Clone()
    {
        throw new NotImplementedException();
    }
}

struct Dispo<T>
{
    
}