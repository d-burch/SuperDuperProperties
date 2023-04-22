using PropertyManagement.Models;
using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Reflection;

namespace PropertyManagement.Data
{
    internal static class DataAccess
    {
        private const string connectionString =
            "Server=(localdb)\\mssqllocaldb;Database=ReallyGoodPropertyManagement;Trusted_Connection=True;MultipleActiveResultSets=true";
        private const string sqlGetAllPropertiesSproc = "GetAllProperties";
        private const string sqlGetRenterSproc = "GetRenter";
        private const string sqlUpdateSproc = "Update";
        private const string sqlForeignKeys =
            "SELECT o.OwnerID, PropertyID, p.Property_OwnerId, l.LeaseID, l.Lease_PropertyId, RenterID, r.Renter_LeaseId" +
            " FROM Owner AS o" +
            " INNER JOIN Property AS p ON p.Property_OwnerId = o.OwnerID" +
            " LEFT JOIN Lease AS l ON l.Lease_PropertyId = p.PropertyID" +
            " LEFT JOIN Renter AS r ON r.Renter_LeaseId = l.LeaseID;";

        internal static List<Owner> GetFullDataGraph()
        {
            var allOwners = new List<Owner>();

            using (var connection = new SqlConnection(connectionString))
            {
                var ownerDictionary = new Dictionary<int, Owner>();
                var propertyDictionary = new Dictionary<int, Property>();
                var leaseDictionary = new Dictionary<int, Lease>();
                var foreignKeysDictionary = new Dictionary<string, List<Int32?>>();

                allOwners = connection.Query<Owner, Models.Property, Lease, Renter, Owner>(sqlGetAllPropertiesSproc,
                    (owner, property, lease, renter) =>
                    {
                        //if (owner == null) return new Owner();

                        Owner? ownerResult = null;
                        Property? propertyResult = null;
                        Lease? leaseResult = null;

                        if (owner != null && !ownerDictionary.TryGetValue(owner.OwnerID, out ownerResult))
                        {
                            ownerResult = owner;
                            ownerResult.Properties = new List<Property>();
                            ownerDictionary.Add(ownerResult.OwnerID, ownerResult);
                        }
                        if (property != null && !propertyDictionary.TryGetValue(property.PropertyID, out propertyResult))
                        {
                            propertyResult = property;
                            propertyResult.Leases = new List<Lease>();
                            propertyDictionary.Add(propertyResult.PropertyID, propertyResult);
                        }
                        if (lease != null && !leaseDictionary.TryGetValue(lease.LeaseID, out leaseResult))
                        {
                            leaseResult = lease;
                            leaseResult.Renters = new List<Renter>();
                            leaseDictionary.Add(leaseResult.LeaseID, leaseResult);
                        }

                        if (leaseResult?.Renters != null && renter != null)
                        {
                            leaseResult.Renters.Add(renter);
                        }
                        if (propertyResult?.Leases != null && leaseResult != null)
                        {
                            propertyResult.Leases.Add(leaseResult);
                        }
                        if (ownerResult?.Properties != null && propertyResult != null)
                        {
                            ownerResult.Properties.Add(propertyResult);
                        }

                        return ownerResult;
                    },
                    splitOn: "PropertyID, LeaseID, RenterID") // AS {name} works here if need be
                    .Distinct() // Distinct owners - only one owner per property allowed
                    .ToList();

                allOwners.RemoveAll(owner => owner == null);

                DataUtility.FilterPropertiesAndLeases(allOwners);

                System.Console.WriteLine();

                return allOwners;
            }
        }

        internal static Renter GetRenter(int id)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                try
                {
                    var param = new { RenterId = id };
                    var renter = connection
                        .Query<Renter>(sqlGetRenterSproc, param, commandType: CommandType.StoredProcedure)
                        .FirstOrDefault();

                    if (renter == null)
                    {
                        throw new System.Data.RowNotInTableException(innerException: null, message: $"Renter not found for id {id}");
                    }

                    return renter;
                }
                catch (System.Data.RowNotInTableException ex) {
                    System.Console.WriteLine(ex.Message);
                    throw;
                }
            }
        }

        internal static bool Update<T>(T entity)
        {
            int rowsAffected = 0;

            using (var connection = new SqlConnection(connectionString))
            {
                try
                {
                    var parameters = GetParameters<T>(entity);
                    var sql = sqlUpdateSproc + typeof(T).Name;

                    rowsAffected = connection
                        .Execute(sql, parameters, commandType: CommandType.StoredProcedure);
                }
                catch (RowNotInTableException ex)
                {
                    System.Console.WriteLine(ex.Message);
                    throw;
                }
            }

            return rowsAffected > 0;
        }

        internal static Dictionary<string, object> GetParameters<T>(T entity)
        {
            List<PropertyInfo> typeProperties = typeof(T).GetProperties().ToList();
            var parameters = new Dictionary<string, object>();

            foreach (var property in typeProperties)
            {
                parameters.Add(property.Name, property.GetValue(entity));
            }

            return parameters;
        }

        /*
        internal static bool UpdateRenter(Renter renter)
        {
            int rowsAffected = 0;

            using (var connection = new SqlConnection(connectionString))
            {
                try
                {
                    var parameters = new {
                        RenterId = renter.RenterID,
                        FirstName = renter.FirstName,
                        LastName = renter.LastName,
                        Email = renter.Email,
                        Phone = renter.Phone
                    };
                    rowsAffected = connection
                        .Execute(sqlUpdateRenterSproc, parameters, commandType: CommandType.StoredProcedure);
                }
                catch (RowNotInTableException ex)
                {
                    System.Console.WriteLine(ex.Message);
                    throw;
                }
            }

            return rowsAffected > 0;
        }
        */

        internal static Dictionary<string, List<Int32?>> GetAllForeignKeys()
        {
            var foreignKeysDictionary = new Dictionary<string, List<Int32?>>();

            using (var connection = new SqlConnection(connectionString))
            {
                var sqlCommand = new SqlCommand(sqlForeignKeys, connection);
                try
                {
                    connection.Open();
                    var reader = sqlCommand.ExecuteReader();

                    while (reader.Read())
                    {
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            var columnName = reader.GetName(i);

                            if (!reader.IsDBNull(i))
                            {
                                var id = reader.GetInt32(i);

                                if (foreignKeysDictionary.TryGetValue(columnName, out List<int?> ids))
                                {
                                    if (!ids.Contains(id))
                                    {
                                        ids.Add(id);
                                    }
                                }
                                else
                                {
                                    foreignKeysDictionary.Add(columnName, new List<int?> { id });
                                }
                            }
                        }
                    }
                }
                catch (SqlException ex)
                {
                    System.Console.WriteLine(ex);
                    throw;
                }
            }

            return foreignKeysDictionary;
        }

    }

    // Example
    //command.Parameters.AddWithValue("@AddressLine1", addressLine1);
}
