namespace Xcsb;

public class XcbClientConfiguration
{
    public static XcbClientConfiguration Default => new XcbClientConfiguration();
    public const int StackAllocThreshold = 1024;
}