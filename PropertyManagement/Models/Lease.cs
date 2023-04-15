using System.ComponentModel.DataAnnotations;

namespace PropertyManagement.Models
{
    public class Lease
    {
        public int LeaseID { get; set; }
        public IEnumerable<Renter> Renters { get; set; }

        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }
        public decimal MonthlyRent { get; set; }
        public decimal? RentOutstanding { get; set; }
        public decimal? SecurityDepositAmount { get; set; }
        public bool? SecurityDepositPaid { get; set; }
        public decimal? SecurityDepositCharges { get; set; }
        public bool? SecurityDepositReturned { get; set; }

    }
}
