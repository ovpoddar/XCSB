using Xcsb.Extension.Damage.Models;

namespace Xcsb.Extension.Damage.Infrastructure;

public interface IDamageUnchecked
{
    void CreateUnchecked(uint damage, uint drawable, ReportLevel reportLevel);
    void DestroyUnchecked(uint damage);
    void SubtractUnchecked(uint damage, uint repair, uint parts);
    void AddUnchecked(uint drawable, uint region);
}