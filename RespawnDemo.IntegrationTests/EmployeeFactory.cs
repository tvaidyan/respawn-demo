using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RespawnDemo.IntegrationTests
{
    public class EmployeeFactory
    {
        private string databaseConnectionString;

        public EmployeeFactory(string databaseConnectionString)
        {
            this.databaseConnectionString = databaseConnectionString;
        }
        public void CreateRandomSamplingOfEmployees(int count = 10)
        {
            var insertSQL = new StringBuilder();
            for (int i = 0; i < count; i++)
            {
                insertSQL.AppendLine("INSERT INTO Employees ([EmployeeId],[HireDate],[FirstName],[LastName]" +
                    ",[FavoriteColor]) " +
                    $" VALUES(NEWID(), GETUTCDATE(), 'First-{i}', 'Last-{i}', 'Color-{i}');");
            }

            using (SqlConnection connection = new SqlConnection(
                   databaseConnectionString))
            {
                SqlCommand command = new SqlCommand(insertSQL.ToString(), connection);
                command.Connection.Open();
                command.ExecuteNonQuery();
            }
        }
    }
}
