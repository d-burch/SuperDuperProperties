using PropertyManagement.Models;
using Dapper.FluentMap.Mapping;

namespace PropertyManagement.Data.Mapping
{
    internal class OwnerMap : EntityMap<Owner>
    {
        internal OwnerMap()
        {
            Map(owner => owner.FirstName).ToColumn("Owner_FirstName");
            Map(owner => owner.LastName).ToColumn("Owner_LastName");
            Map(owner => owner.Email).ToColumn("Owner_Email");
            Map(owner => owner.Phone).ToColumn("Owner_Phone");
        }
    }
}
