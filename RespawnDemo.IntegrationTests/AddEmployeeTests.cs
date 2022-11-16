using FluentAssertions;
using Microsoft.Data.SqlClient;
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

        var client = factory.CreateClient();

        CreateRandomSamplingOfWeatherForecastsInTheDatabase();

        var forecast = new AddEmployeeRequest
        {
            HireDate = DateTime.UtcNow.AddDays(-90),
            FirstName = "Test",
            LastName = "Person",
            FavoriteColor = "Red"
        };

        HttpContent c = new StringContent(JsonSerializer.Serialize(forecast), Encoding.UTF8, "application/json");
        await client.PostAsync("/employee", c);

        var response = (await client.SendAsync(new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri("https://localhost:7050/employee"),
            Content = new StringContent(JsonSerializer.Serialize(new Employee { EmployeeId = "TEST1234" }), Encoding.UTF8, "application/json")
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

    private void CreateRandomSamplingOfWeatherForecastsInTheDatabase()
    {
        var insertSQL = new StringBuilder();
        for (int i = 0; i < 25; i++)
        {
            insertSQL.AppendLine("INSERT INTO Employees ([EmployeeId],[HireDate],[FirstName],[LastName]" +
                ",[FavoriteColor]) " +
                $" VALUES('Emp{i}', GETUTCDATE(), 'First-{i}', 'Last-{i}', 'Color-{i}');");
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
