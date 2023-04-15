using PropertyManagement.Models;
using Dapper;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using PropertyManagement.Data;
using PropertyManagement.Models;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Policy;
using Microsoft.Data.SqlClient;
using System.Reflection.Emit;

namespace PropertyManagement.Controllers
{
    public class PropertiesArchiveController : Controller
    {
        private readonly ILogger<PropertiesArchiveController> _logger;

        public PropertiesArchiveController(ILogger<PropertiesArchiveController> logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var properties = new List<Property>();
            var connectionString = "Server=(localdb)\\mssqllocaldb;Database=ReallyGoodPropertyManagement;Trusted_Connection=True;MultipleActiveResultSets=true";
            var getPropertiesSproc = "GetAllProperties";
            var sqlOwners = "SELECT * FROM Owner;";

            // splitOn won't work if there is a conflict b/w a PK and a FK name
            // {columnName} AS {name} will break dapper mapping b/c the new name won't match the data model
            var sqlOwnersProperties = 
		        "SELECT o.OwnerID, o.FirstName, o.LastName, o.Email, o.Phone," +
                " p.PropertyID, p.Property_OwnerId, AddressLine1, AddressLine2, City, StateCode, ZipCode, Bedrooms, Bathrooms, PetsAllowed, UnitNumber, WasherDryer, Dishwasher" +
                " FROM Owner AS o" +
                " INNER JOIN Property AS p ON o.OwnerID = p.Property_OwnerId;";

            using (var connection = new SqlConnection(connectionString))
            {
                var ownerDictionary = new Dictionary<int, Owner>();

                var allProperties = connection.Query<Owner, Models.Property, Owner>(sqlOwnersProperties,
                    (owner, property) =>
                    {
                        Owner ownerResult;

                        if (!ownerDictionary.TryGetValue(owner.OwnerID, out ownerResult))
                        {
                            ownerResult = owner;
                            ownerResult.Properties = new List<Property>();
                            ownerDictionary.Add(ownerResult.OwnerID, ownerResult);
                        }
                        Debug.WriteLine(owner);
                        Debug.WriteLine(property);

                        if (ownerResult.Properties != null)
                        {
                            ownerResult.Properties.ToList().Add(property);
                        }
                        return ownerResult;
                    },
                    splitOn: "PropertyID") // AS {name} works here if need be
                    .Distinct()
                    .ToList();

                /*
                using (var command = new SqlCommand
                {
                    CommandText = sqlOwnersProperties,
                    CommandType = System.Data.CommandType.Text,
                    Connection = connection
                })
                {
                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var x = reader[0];
                        }
                    }
                    //connection.Close();
                }
                */


                /*
                Type[] types = new Type[] { typeof(Owner), typeof(Property) };

                var allProperties = connection.Query<Owner>(sqlOwnersProperties,
                    types,
                    (typeArr, owner) =>
                    {
                        Owner ownerResult;

                        if (!ownerDictionary.TryGetValue(owner.OwnerID, out ownerResult))
                        {
                            ownerResult = owner;
                            ownerResult.Properties = new List<Property>();
                            ownerDictionary.Add(ownerResult.OwnerID, ownerResult);
                        }
                        Debug.WriteLine(owner);
                        Debug.WriteLine(property);

                        if (ownerResult.Properties != null)
                        {
                            ownerResult.Properties.ToList().Add(property);
                        }
                        return ownerResult;
                    },
                    splitOn: "OwnerID")
                    .Distinct()
                    .ToList();
                */

                System.Diagnostics.Debug.WriteLine(ownerDictionary.Count);
            }

            return View(properties);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}