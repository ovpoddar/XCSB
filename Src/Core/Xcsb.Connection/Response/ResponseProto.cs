namespace Xcsb.Connection.Response;

public readonly struct ResponseProto
{
    public uint Id { get; }
    public bool HasReturn { get; }

    internal ResponseProto(uint id = 0, bool hasReturn = false)
    {
        Id = id;
        HasReturn = hasReturn;
    }
}
