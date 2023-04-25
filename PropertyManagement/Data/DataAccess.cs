using PropertyManagement.Models;
using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Reflection;
using Microsoft.IdentityModel.Tokens;

namespace PropertyManagement.Data
{
    internal static class DataAccess
    {
        private const string connectionString =
            "Server=(localdb)\\mssqllocaldb;Database=ReallyGoodPropertyManagement;Trusted_Connection=True;MultipleActiveResultSets=true";
        private const string sqlGetAllPropertiesSproc = "GetAllProperties";
        private const string sqlGetSproc = "Get";
        private const string sqlUpdateSproc = "Update";
        private const string sqlInsertSproc = "Insert";
        private const string sqlGetOwnerIdByEmailSproc = "GetOwnerIdByEmail";
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

        internal static int GetOwnerIdByEmail(string email)
        {
            if (email.IsNullOrEmpty()) return 0;

            using (var connection = new SqlConnection(connectionString))
            {
                try
                {
                    var result = connection
                        .Query(sqlGetOwnerIdByEmailSproc, new { @Email = email }, commandType: CommandType.StoredProcedure)
                        .FirstOrDefault();

                    if (result == null)
                    {
                        throw new System.Data.RowNotInTableException(innerException: null,
                            message: $"Owner not found for email {email}");
                    }

                    return result.OwnerID;
                }
                catch (System.Data.RowNotInTableException ex) {
                    System.Console.WriteLine(ex.Message);
                    throw;
                }
            }
        }

        internal static T Get<T>(int id)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                try
                {
                    var typeName = typeof(T).Name;
                    var idFieldName = typeName + "ID";
                    var param = new DynamicParameters();
                    param.Add(idFieldName, id);
                    var sql = sqlGetSproc + typeName;

                    var result = connection
                        .Query<T>(sql, param, commandType: CommandType.StoredProcedure)
                        .FirstOrDefault();

                    if (result == null)
                    {
                        throw new System.Data.RowNotInTableException(innerException: null,
                            message: $"{typeName} not found for id {id}");
                    }

                    return result;
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
                    var parameters = GetParameters<T>(entity, true, (null, null));
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

        internal static bool Insert<T>(T entity, (int?, string?) fk)
        {
            int rowsAffected = 0;

            using (var connection = new SqlConnection(connectionString))
            {
                try
                {
                    var parameters = GetParameters<T>(entity, false, fk);
                    var sql = sqlInsertSproc + typeof(T).Name;

                    rowsAffected = connection
                        .Execute(sql, parameters, commandType: CommandType.StoredProcedure);

                }
                catch (RowNotInTableException ex)
                {
                    System.Console.WriteLine(ex.Message);
                    throw;
                }
                catch (SqlException ex)
                {
                    if (ex.Message.Contains("Cannot insert the value NULL into column"))
                    {
                        System.Console.WriteLine(ex.Message);
                        return false;
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            return rowsAffected > 0;
        }

        // Todo: fix this signature
        internal static Dictionary<string, object> GetParameters<T>(T entity, bool includeId, (int?, string?) fk)
        {
            List<PropertyInfo> typeProperties = typeof(T).GetProperties().ToList();
            var parameters = new Dictionary<string, object>();

            foreach (var property in typeProperties)
            {
                // Dirty hack - don't add Lists and other types
                if ((property.PropertyType.IsValueType || property.PropertyType.Name == "String") &&
                    (property.Name != typeof(T).Name + "ID" || includeId))
                {
                    parameters.Add(property.Name, property.GetValue(entity));
                }
            }

            // Add foreign key if there is one
            if (fk.Item1 != null)
            {
                parameters.Add($"{typeof(T).Name}_{fk.Item2}Id", fk.Item1);
            }

            return parameters;
        }

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
