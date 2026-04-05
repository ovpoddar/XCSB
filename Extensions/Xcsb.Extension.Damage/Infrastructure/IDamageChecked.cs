using Xcsb.Extension.Damage.Models;

namespace Xcsb.Extension.Damage.Infrastructure;

public interface IDamageChecked
{
    void CreateChecked(uint damage, uint drawable, ReportLevel reportLevel);
    void DestroyChecked(uint damage);
    void SubtractChecked(uint damage, uint repair, uint parts);
    void AddChecked(uint drawable, uint region);
}