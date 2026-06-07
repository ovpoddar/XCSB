namespace Xcsb.Extension.XInput.Models;

internal interface IHierarchyChange
{
    HierarchyChange Type { get; }
    ushort Length { get; }
}