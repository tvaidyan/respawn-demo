using FluentAssertions;
using System.Data.SqlClient;
using System.Text;
using System.Text.Json;
using RespawnDemo.Api.Employee;
using RespawnDemo.IntegrationTests.Shared;
using Xunit;

namespace RespawnDemo.IntegrationTests;
public class AddEmployeeTests : IClassFixture<DbFixture>
{
    private readonly DbFixture dbFixture;

    public AddEmployeeTests(DbFixture dbFixture)
    {
        this.dbFixture = dbFixture;
    }

    [Fact]
    public async Task CanSaveAndRetrieveEmployees()
    {
        var factory = new CustomWebApplicationFactory<Program>(services =>
        {
            services.SetupDatabaseConnection(dbFixture.DatabaseConnectionString);
        });

        CreateRandomSamplingOfEmployees();

        var client = factory.CreateClient();

        var employee = new AddEmployeeRequest
        {
            HireDate = DateTime.UtcNow.AddDays(-90),
            FirstName = "Test",
            LastName = "Person",
            FavoriteColor = "Red"
        };

        HttpContent createEmployeePayload = new StringContent(JsonSerializer.Serialize(employee), Encoding.UTF8, "application/json");
        var createEmployeeResponse = await client.PostAsync("/employees", createEmployeePayload);

        var response = (await client.SendAsync(new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri(createEmployeeResponse.Headers.Location!.ToString().Replace("localhost", "localhost:7050"))
        })).Content.ReadAsStringAsync().Result;

        var getResponse = JsonSerializer.Deserialize<Employee>(response, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        })!;

        //getResponse.WeatherForecasts.Should().HaveCount(1);
        //getResponse.WeatherForecasts.Should().ContainSingle(x =>
        //    x.TemperatureC == 30
        //    && x.Summary == "Sunny");
    }

    private void CreateRandomSamplingOfEmployees()
    {
        var insertSQL = new StringBuilder();
        for (int i = 0; i < 25; i++)
        {
            insertSQL.AppendLine("INSERT INTO Employees ([EmployeeId],[HireDate],[FirstName],[LastName]" +
                ",[FavoriteColor]) " +
                $" VALUES(NEWID(), GETUTCDATE(), 'First-{i}', 'Last-{i}', 'Color-{i}');");
        }

        using (SqlConnection connection = new SqlConnection(
               dbFixture.DatabaseConnectionString))
        {
            SqlCommand command = new SqlCommand(insertSQL.ToString(), connection);
            command.Connection.Open();
            command.ExecuteNonQuery();
        }
    }
}
