using PropertyManagement.Models;

namespace PropertyManagement.Data
{
    public static class DataUtility
    {
        private static List<Property> FilterProperties(List<Property> properties)
        {
            return properties.Select(property => property).Distinct().ToList();
        }

        private static List<Lease> FilterLeases(List<Lease> leases)
        {
            return leases.Select(lease => lease).Distinct().ToList();
                //.Where(lease => lease.StartDate < DateTime.Now && lease.EndDate > DateTime.Now).ToList();
        }

        internal static void FilterPropertiesAndLeases(List<Owner> owners)
        {
            foreach (var owner in owners)
            {
                foreach (var property in owner.Properties)
                {
                    property.Leases = FilterLeases(property.Leases);
                }
                owner.Properties = FilterProperties(owner.Properties);
            }
        }
    }
}
