using FluentAssertions;
using RespawnDemo.Api.Employee;
using RespawnDemo.IntegrationTests.Shared;
using System.Net;
using System.Text;
using System.Text.Json;
using Xunit;

namespace RespawnDemo.IntegrationTests;

[Collection("EmployeeDbCollection")]
public class UpdateEmployeeTests
{
    private readonly DbFixture dbFixture;

    public UpdateEmployeeTests(DbFixture dbFixture)
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
        employeeFactory.CreateRandomSamplingOfEmployees(10);
        var createdEmployees = employeeFactory.GetAllEmployees();
        var employeeToUpdate = createdEmployees[new Random().Next(0, 9)];

        var client = factory.CreateClient();

        var employee = new UpdateEmployeeRequest
        {
            EmployeeId = employeeToUpdate.EmployeeId,
            HireDate = DateTime.UtcNow.AddDays(-90),
            FirstName = "Tom",
            LastName = "Hanks",
            FavoriteColor = "Blue"
        };

        HttpContent updateEmployeePayload = new StringContent(JsonSerializer.Serialize(employee), Encoding.UTF8, "application/json");
        var updateEmployeeResponse = await client.PutAsync("/employees", updateEmployeePayload);

        var response = (await client.SendAsync(new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri($@"http://localhost:7050/employees/{employeeToUpdate.EmployeeId}")
        })).Content.ReadAsStringAsync().Result;

        var updatedEmployee = JsonSerializer.Deserialize<Employee>(response, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        })!;

        updateEmployeeResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        updatedEmployee.Should().NotBeNull();
        updatedEmployee!.FirstName.Should().Be("Tom");
        updatedEmployee.LastName.Should().Be("Hanks");
        updatedEmployee.FavoriteColor.Should().Be("Blue");
    }
}
