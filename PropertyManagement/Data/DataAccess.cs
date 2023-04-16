﻿using PropertyManagement.Models;
using Dapper;
using Microsoft.Data.SqlClient;

namespace PropertyManagement.Data
{
    internal static class DataAccess
    {
        private const string connectionString =
            "Server=(localdb)\\mssqllocaldb;Database=ReallyGoodPropertyManagement;Trusted_Connection=True;MultipleActiveResultSets=true";
        private const string sqlGetAllPropertiesSproc = "GetAllProperties";
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
                        Owner ownerResult;
                        Property propertyResult;
                        Lease leaseResult;

                        if (!ownerDictionary.TryGetValue(owner.OwnerID, out ownerResult))
                        {
                            ownerResult = owner;
                            ownerResult.Properties = new List<Property>();
                            ownerDictionary.Add(ownerResult.OwnerID, ownerResult);
                        }
                        if (!propertyDictionary.TryGetValue(property.PropertyID, out propertyResult))
                        {
                            propertyResult = property;
                            propertyResult.Leases = new List<Lease>();
                            propertyDictionary.Add(propertyResult.PropertyID, propertyResult);
                        }
                        if (!leaseDictionary.TryGetValue(lease.LeaseID, out leaseResult))
                        {
                            leaseResult = lease;
                            leaseResult.Renters = new List<Renter>();
                            leaseDictionary.Add(leaseResult.LeaseID, leaseResult);
                        }

                        if (leaseResult.Renters != null && renter != null)
                        {
                            leaseResult.Renters.Add(renter);
                        }
                        if (propertyResult.Leases != null && leaseResult != null)
                        {
                            propertyResult.Leases.Add(leaseResult);
                        }
                        if (ownerResult.Properties != null && propertyResult != null)
                        {
                            ownerResult.Properties.Add(propertyResult);
                        }

                        return ownerResult;
                    },
                    splitOn: "PropertyID, LeaseID, RenterID") // AS {name} works here if need be
                    .Distinct() // Distinct owners - only one owner per property allowed
                    .ToList();

                DataUtility.FilterPropertiesAndLeases(allOwners);

                System.Console.WriteLine();

                return allOwners;
            }
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