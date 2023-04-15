using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PropertyManagement.Models
{
    public class Property
    {
        [Key]
        //[Column("Property_PropertyID")]
        public int PropertyID { get; set; }
        public IEnumerable<Lease> Leases { get; set; }
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
    }
}
