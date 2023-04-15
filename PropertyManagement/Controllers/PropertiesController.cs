using PropertyManagement.Models;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using PropertyManagement.Data;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Data.SqlClient;

namespace PropertyManagement.Controllers
{
    public class PropertiesController : Controller
    {
        private readonly ILogger<PropertiesController> _logger;

        public PropertiesController(ILogger<PropertiesController> logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var allOwners = new List<Owner>();
            var connectionString = "Server=(localdb)\\mssqllocaldb;Database=ReallyGoodPropertyManagement;Trusted_Connection=True;MultipleActiveResultSets=true";
            var getPropertiesSproc = "GetAllProperties";
            var sqlOwnersProperties = 
		        "SELECT o.OwnerID, o.FirstName AS Owner_FirstName, o.LastName AS Owner_LastName, o.Email AS Owner_Email, o.Phone AS Owner_Phone," +
                " p.PropertyID, p.Property_OwnerId, AddressLine1, AddressLine2, City, StateCode, ZipCode, Bedrooms, Bathrooms, PetsAllowed, UnitNumber, WasherDryer, Dishwasher" +
                " FROM Owner AS o" +
                " INNER JOIN Property AS p ON o.OwnerID = p.Property_OwnerId;";

            using (var connection = new SqlConnection(connectionString))
            {
                var ownerDictionary = new Dictionary<int, Owner>();
                var propertyDictionary = new Dictionary<int, Property>();
                var leaseDictionary = new Dictionary<int, Lease>();

                // Owners are top-level, use each owner to get list of all properties in the View
                allOwners = connection.Query<Owner, Models.Property, Lease, Renter, Owner>(getPropertiesSproc,
                    (owner, property, lease, renter) =>
                    {
                        Owner ownerResult;

                        if (!ownerDictionary.TryGetValue(owner.OwnerID, out ownerResult))
                        {
                            ownerResult = owner;
                            ownerResult.Properties = new List<Property>();
                            ownerDictionary.Add(ownerResult.OwnerID, ownerResult);
                        }

                        if (ownerResult.Properties != null)
                        {
                            ownerResult.Properties.Add(property);
                        }
                        return ownerResult;
                    },
                    splitOn: "PropertyID, LeaseID, RenterID") // AS {name} works here if need be
                    .Distinct() // Distinct owners - only one owner per property allowed
                    .ToList();
            }

            return View(allOwners);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}