using Xcsb.Connection.Response;
using Xcsb.Extension.Damage.Models;
using Xcsb.Extension.Damage.Response.Replies;

namespace Xcsb.Extension.Damage.Infrastructure;

public interface IDamageRequest : IDamage, IDamageChecked, IDamageUnchecked
{
    DamageQueryVersionReply QueryVersion(uint majorVersion, uint minorVersion);
}
