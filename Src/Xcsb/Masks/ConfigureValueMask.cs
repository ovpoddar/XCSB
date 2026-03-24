namespace Xcsb.Masks;

[Flags]
public enum ConfigureValueMask : ushort
{
    X = 1,
    Y = 2,
    Width = 4,
    Height = 8,
    BorderWidth = 16,
    Sibling = 32,
    StackMode = 64
}