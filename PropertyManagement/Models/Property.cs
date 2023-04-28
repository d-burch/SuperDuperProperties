namespace PropertyManagement.Models
{
    public class Property
    {
        public int PropertyID { get; set; }
        public List<Lease> Leases { get; set; } = new List<Lease>();
        public Lease? CurrentLease
        {
            get
            {
                if (Leases == null) return null;

                return Leases.Where(lease => lease.StartDate < DateTime.Now && lease.EndDate > DateTime.Now).FirstOrDefault();
            }
            set { CurrentLease = value; }
        }
        public string AddressLine1 { get; set; }
        public string? AddressLine2 { get; set; }
        public string City { get; set; }
        public string StateCode { get; set; }
        public string ZipCode { get; set; }
        public int Bedrooms { get; set; }
        public int Bathrooms { get; set; }
        public bool? PetsAllowed { get; set; }
        public string? UnitNumber { get; set; }
        public bool? WasherDryer { get; set; }
        public bool? Dishwasher { get; set; }

        public struct SearchCriteria
        {
            public string? City { get; set; }
            public string? StateCode { get; set; }
            public string? ZipCode { get; set; }
            public int? Bedrooms { get; set; }
            public int? Bathrooms { get; set; }
            public bool? PetsAllowed { get; set; }
            public bool? WasherDryer { get; set; }
            public bool? Dishwasher { get; set; }
        }
    }
}
