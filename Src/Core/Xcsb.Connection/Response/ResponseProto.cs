namespace Xcsb.Connection.Response;

public readonly ref struct ResponseProto
{
    public ushort Id { get; }
    public bool HasReturn { get; }

    internal ResponseProto(ushort id = 0, bool hasReturn = false)
    {
        Id = id;
        HasReturn = hasReturn;
    }
}
