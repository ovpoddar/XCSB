namespace Xcsb.Extension.Generic.Event.Infrastructure.Exceptions;

public sealed class InsufficientDataException : Exception
{
    public int RequiredCount { get; }
    public int AvailableCount { get; }

    public InsufficientDataException(
        int requiredCount,
        int availableCount,
        string flagField,
        string collectionField)
        : base(FormatMessage(requiredCount, availableCount, flagField, collectionField))
    {
        RequiredCount = requiredCount;
        AvailableCount = availableCount;
    }

    private static string FormatMessage(int required, int available, string flagField, string collectionField) =>
        $"The '{collectionField}' collection requires {required} item(s), but only {available} were supplied. This requirement is determined by the '{flagField}' flag.";
}
