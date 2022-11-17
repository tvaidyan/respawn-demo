using RespawnDemo.Api.Employee;
using System.Data.SqlClient;
using System.Text;

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

        public List<Employee> GetAllEmployees()
        {
            var sql = "SELECT * FROM Employees";

            using SqlConnection connection = new SqlConnection(
                   databaseConnectionString);
            SqlCommand command = new(sql, connection);
            command.Connection.Open();
            using SqlDataReader reader = command.ExecuteReader();

            var employees = new List<Employee>();
            while (reader.Read())
            {
                employees.Add(new Employee
                {
                    EmployeeId = (Guid)reader["EmployeeId"],
                    FavoriteColor = (string)reader["FavoriteColor"],
                    FirstName = (string)reader["FirstName"],
                    LastName = (string)reader["LastName"]
                });
            }

            return employees;
        }
    }
}
