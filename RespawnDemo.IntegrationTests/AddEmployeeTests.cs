using FluentAssertions;
using RespawnDemo.Api.Employee;
using RespawnDemo.IntegrationTests.Shared;
using System.Net;
using System.Text;
using System.Text.Json;
using Xunit;

namespace RespawnDemo.IntegrationTests;

[Collection("EmployeeDbCollection")]
public class AddEmployeeTests
{
    private readonly DbFixture dbFixture;

    public AddEmployeeTests(DbFixture dbFixture)
    {
        this.dbFixture = dbFixture;
    }

    [Fact]
    public async Task CanCreateAndRetrieveEmployee()
    {
        var factory = new CustomWebApplicationFactory<Program>(services =>
        {
            services.SetupDatabaseConnection(dbFixture.DatabaseConnectionString);
        });

        var employeeFactory = new EmployeeFactory(dbFixture.DatabaseConnectionString);
        employeeFactory.CreateRandomSamplingOfEmployees();

        var client = factory.CreateClient();

        var employee = new AddEmployeeRequest
        {
            HireDate = DateTime.UtcNow.AddDays(-90),
            FirstName = "Bruce",
            LastName = "Willis",
            FavoriteColor = "Red"
        };

        HttpContent createEmployeePayload = new StringContent(JsonSerializer.Serialize(employee), Encoding.UTF8, "application/json");
        var createEmployeeResponse = await client.PostAsync("/employees", createEmployeePayload);

        var response = (await client.SendAsync(new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri(createEmployeeResponse.Headers.Location!.ToString().Replace("localhost", "localhost:7050"))
        })).Content.ReadAsStringAsync().Result;

        var employeeCreated = JsonSerializer.Deserialize<Employee>(response, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        })!;

        createEmployeeResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        employeeCreated.Should().NotBeNull();
        employeeCreated!.FirstName.Should().Be("Bruce");
        employeeCreated.LastName.Should().Be("Willis");
        employeeCreated.FavoriteColor.Should().Be("Red");
    }
}
