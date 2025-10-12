namespace Xcsb.Models.Infrastructure.Response;
// todo: rethink about access modifier
public readonly ref struct ResponseProto
{
    public readonly int Id { get; }
    public readonly bool HasReturn { get; }

    public ResponseProto(int id = 0, bool hasReturn = false)
    {
        Id = id;
        HasReturn = hasReturn;
    }
}
