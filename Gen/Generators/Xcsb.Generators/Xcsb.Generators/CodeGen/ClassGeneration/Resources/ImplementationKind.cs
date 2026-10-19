namespace Xcsb.Generators
{
    [global::System.Flags]
    public enum ImplementationKind
    {
        Checked = 1,
        Unchecked = 2,
        Base = 4,
        Return = 8,
        Async = 16,
    }
}