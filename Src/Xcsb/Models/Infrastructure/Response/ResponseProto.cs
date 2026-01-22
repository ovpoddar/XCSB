namespace Xcsb.Models.Infrastructure.Response;

public readonly ref struct ResponseProto
{
    public int Id { get; }
    internal bool HasReturn { get; }

    internal ResponseProto(int id = 0, bool hasReturn = false)
    {
        Id = id;
        HasReturn = hasReturn;
    }
}
