using Xcsb.Connection.Response;
using Xcsb.Extension.Damage.Models;

namespace Xcsb.Extension.Damage.Infrastructure;

public interface IDamage
{
    ResponseProto Create(uint damage, uint drawable, ReportLevel reportLevel);
    ResponseProto Destroy(uint damage);
    ResponseProto Subtract(uint damage, uint repair, uint parts);
    ResponseProto Add(uint drawable, uint region);
}