namespace Xcsb.Connection.Response;

public readonly ref struct ResponseProto
{
    public int Id { get; }
    public bool HasReturn { get; }

    internal ResponseProto(int id = 0, bool hasReturn = false)
    {
        Id = id;
        HasReturn = hasReturn;
    }
}
