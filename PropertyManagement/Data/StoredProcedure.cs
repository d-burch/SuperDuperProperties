using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace PropertyManagement.Data
{
    public static class StoredProcedure
    {
        /*
        public ActionResult RetrieveAllProperties()
        {
            var connection = new SqlConnection("");

            try
            {
                var command = new SqlCommand("NameOfSproc", connection);
                command.CommandType = System.Data.CommandType.StoredProcedure;

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                //reader.
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                connection.Close();
            }
        }
        */
    }

    // Example
    //command.Parameters.AddWithValue("@AddressLine1", addressLine1);
}
