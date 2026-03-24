namespace Xcsb.Connection.Models.TypeInfo;

public sealed record UnknownResponse : XEventType
{
    public UnknownResponse(byte value) : base(value, "Unknown")
    {
    }

    public static UnknownResponse Unknown(byte value) => new UnknownResponse(value);
}
